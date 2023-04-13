using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Combat
{
    public static class CombatHandler
    {
        public static Player FindTarget(Player attacker, string target, Room room, bool isMurder)
        {
            if (string.IsNullOrEmpty(target))
            {
                return null;
            }
            // If mob
            if (isMurder && attacker.ConnectionId != "mob")
            {
                return room.Players.FirstOrDefault(
                    x => x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase)
                );
            }

            if (attacker.ConnectionId == "mob")
            {
                return room.Players.FirstOrDefault(
                    x => x.Name.Equals(target, StringComparison.CurrentCultureIgnoreCase)
                );
            }

            return room.Mobs.FirstOrDefault(
                x =>
                    x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase)
                    && x.IsHiddenScriptMob == false
            );
        }

        public static Item.Item GetWeapon(Player player, bool dualWield = false)
        {
            return dualWield ? player.Equipped.Secondary : player.Equipped.Wielded;
        }

        public static void DisplayDamage(
            Player player,
            Player target,
            Room room,
            Item.Item weapon,
            int damage
        )
        {
            CultureInfo cc = CultureInfo.CurrentCulture;
            var damText = Services.Instance.Damage.DamageText(damage);
            var attackType = "";
            var damageType = "";
            if (weapon == null)
            {
                attackType = player.ConnectionId.Equals(
                    "mob",
                    StringComparison.CurrentCultureIgnoreCase
                )
                    ? player.DefaultAttack?.ToLower(cc)
                    : "punch";
            }
            else
            {
                attackType = Enum.GetName(typeof(Item.Item.AttackTypes), weapon.AttackType)
                    ?.ToLower(cc);
                damageType = Enum.GetName(typeof(Item.Item.DamageTypes), weapon.DamageType)
                    ?.ToLower(cc);
            }

            Services.Instance.Writer.WriteLine(
                $"<p class='combat'>Your {(damageType != "none" ? damageType : "")} {attackType} {damText.Value} {target.Name.ToLower(cc)}. <span class='damage'>[{damage}]</span></p>",
                player
            );
            Services.Instance.Writer.WriteLine(
                $"<p class='combat'>{target.Name} {Services.Instance.Formulas.TargetHealth(player, target)}.</p>",
                player
            );

            Services.Instance.Writer.WriteLine(
                $"<p>{player.Name}'s {(damageType != "none" ? damageType : "")} {attackType} {damText.Value} you. <span class='damage'>[{damage}]</span></p></p>",
                target
            );

            foreach (var pc in room.Players)
            {
                if (pc.Name == player.Name || pc.Name == target.Name)
                {
                    continue;
                }

                Services.Instance.Writer.WriteLine(
                    $"<p>{player.Name}'s {attackType} {damText.Value} {target.Name.ToLower(cc)}.</p>",
                    pc
                );
            }
        }

        public static void DisplayMiss(Player player, Player target, Room room, Item.Item weapon)
        {
            CultureInfo cc = CultureInfo.CurrentCulture;
            var attackType = "";
            if (weapon == null)
            {
                attackType = player.ConnectionId.Equals(
                    "mob",
                    StringComparison.CurrentCultureIgnoreCase
                )
                    ? player.DefaultAttack?.ToLower(cc)
                    : "punch";
            }
            else
            {
                attackType = Enum.GetName(typeof(Item.Item.AttackTypes), weapon.AttackType)
                    ?.ToLower(cc);
            }

            Services.Instance.Writer.WriteLine(
                $"<p class='combat'>Your {attackType} misses {target.Name.ToLower(cc)}.</p>",
                player
            );
            Services.Instance.Writer.WriteLine(
                $"<p class='combat'>{player.Name}'s {attackType} misses you.</p>",
                target
            );

            foreach (var pc in room.Players)
            {
                if (pc.Name == player.Name || pc.Name == target.Name)
                {
                    continue;
                }

                Services.Instance.Writer.WriteLine(
                    $"<p>{player.Name}'s {attackType} misses {target.Name.ToLower(cc)}.</p>",
                    pc
                );
            }
        }

        public static int DamageReduction(Player defender, int damage)
        {
            var ArRating = defender.ArmorRating.Armour + 1;

            decimal damageAfterReduction =
                damage
                * (
                    damage
                    / (
                        damage
                        + ArRating
                        + (defender.Attributes.Attribute[EffectLocation.Dexterity] / 4)
                    )
                );

            return (int)Math.Round(damageAfterReduction, MidpointRounding.ToEven);
        }

        public static int CalculateSkillDamage(Player player, Player target, int damage)
        {
            var strengthMod = Math.Round(
                (decimal)(player.Attributes.Attribute[EffectLocation.Strength] - 20) / 2,
                MidpointRounding.ToEven
            );
            var levelDif = target.Level - player.Level <= 0 ? 0 : target.Level - player.Level;

            var totalDamage =
                damage
                + strengthMod
                + levelDif
                + player.Attributes.Attribute[EffectLocation.DamageRoll];

            var criticalHit = 1;

            if (
                target.Status == CharacterStatus.Status.Sleeping
                || target.Status == CharacterStatus.Status.Stunned
                || target.Status == CharacterStatus.Status.Resting
            )
            {
                criticalHit = 2;
            }

            totalDamage *= criticalHit;

            // calculate damage reduction based on target armour
            var DamageAfterArmourReduction = DamageReduction(target, (int)totalDamage);

            return DamageAfterArmourReduction;
        }

        public static void TargetKilled(Player player, Player target, Room room)
        {
            player.Target = null;
            target.Status = CharacterStatus.Status.Ghost;

            target.DeathCry(room);

            if (player.Grouped)
            {
                // other group members to drop from combat if they're fighting the same target
                // other group members status set to standing

                var isGroupLeader = string.IsNullOrEmpty(player.Following);

                var groupLeader = player;

                if (!isGroupLeader)
                {
                    groupLeader = Services.Instance.Cache
                        .GetPlayerCache()
                        .FirstOrDefault(x => x.Value.Name.Equals(player.Following))
                        .Value;
                }

                var exp = target.GetExpWorth() / (groupLeader.Followers.Count + 1);
                groupLeader.GainExperiencePoints(exp, true);

                foreach (
                    var follower in groupLeader.Followers.Where(
                        follower => follower.Grouped && follower.Following == groupLeader.Name
                    )
                )
                {
                    follower.GainExperiencePoints(exp, true);
                    follower.Target = null;
                }
            }
            else
            {
                player.GainExperiencePoints(target, true);
            }

            Services.Instance.Quest.IsQuestMob(player, target.Name);

            if (target.ConnectionId != "mob")
            {
                Helpers.PostToDiscord($"{target.Name} got killed by {player.Name}!", "event");

                if (player.ConnectionId != "mob")
                {
                    target.PlayerDeaths += 1;
                    player.PlayerKills += 1;
                }
                else
                {
                    target.MobDeaths += 1;
                }
            }

            Services.Instance.Writer.WriteLine("<p class='dead'>You are dead. R.I.P.</p>", target);

            var targetName = target.Name.ToLower(CultureInfo.CurrentCulture);
            var corpse = new Item.Item
            {
                Name = $"The corpse of {targetName}",
                Description = new Description
                {
                    Room = $"The corpse of {targetName} is laying here.",
                    Exam = target.Description,
                    Look = target.Description,
                },
                Slot = EquipmentSlot.Held,
                Level = 1,
                Stuck = true,
                Container = new Container
                {
                    Items = new ItemList(),
                    CanLock = false,
                    IsOpen = true,
                    CanOpen = false,
                },
                ItemType = Item.Item.ItemTypes.Container,
                Decay = target.ConnectionId.Equals("mob", StringComparison.OrdinalIgnoreCase)
                    ? 10
                    : 20,
                DecayTimer = 300 // 5 minutes,
            };

            foreach (var item in target.Inventory)
            {
                item.Equipped = false;
                corpse.Container.Items.Add(item);
            }

            // clear list
            target.Inventory = new ItemList();
            // clear equipped
            target.Equipped = new Equipment();

            var mount = target.Pets.FirstOrDefault(x => x.Name.Equals(target.Mounted.Name));
            if (mount != null)
            {
                target.Pets.Remove(mount);
                target.Mounted.Name = string.Empty;
            }

            // add corpse to room
            room.Items.Add(corpse);
            Services.Instance.UpdateClient.UpdateInventory(target);
            Services.Instance.UpdateClient.UpdateEquipment(target);
            Services.Instance.UpdateClient.UpdateScore(target);
            Services.Instance.UpdateClient.UpdateScore(player);

            if (target.ConnectionId.Equals("mob", StringComparison.CurrentCultureIgnoreCase))
            {
                player.MobKills += 1;

                var randomItem = Services.Instance.RandomItem.WeaponDrop(player);

                if (randomItem != null)
                {
                    corpse.Container.Items.Add(randomItem);
                }

                var command = Services.Instance.Cache.GetCommand("get");
                if (player.Config.AutoLoot && command != null)
                {
                    var corpseIndex = room.Items.IndexOf(corpse) + 1;
                    command.Execute(player, room, new[] { "get", "all", $"{corpseIndex}.corpse" });
                }
            }

            room.Clean = false;

            if (target.ConnectionId.Equals("mob", StringComparison.CurrentCultureIgnoreCase))
            {
                var command = Services.Instance.Cache.GetCommand("sacrifice");

                if (player.Config.AutoSacrifice && command != null)
                {
                    command.Execute(player, room, new[] { "sacrifice", corpse.Name });
                }

                room.Mobs.Remove(target);
                var getTodayMobStats = Services.Instance.PlayerDataBase
                    .GetList<MobStats>(PlayerDataBase.Collections.MobStats)
                    .FirstOrDefault(x => x.Date.Date.Equals(DateTime.Today));

                if (getTodayMobStats != null)
                {
                    getTodayMobStats.MobKills += 1;
                }
                else
                {
                    getTodayMobStats = new MobStats()
                    {
                        MobKills = 1,
                        PlayerDeaths = 0,
                        Date = DateTime.Now,
                    };
                }
                Services.Instance.PlayerDataBase.Save(
                    getTodayMobStats,
                    PlayerDataBase.Collections.MobStats
                );
            }
            else
            {
                room.Players.Remove(target);
                var getTodayMobStats = Services.Instance.PlayerDataBase
                    .GetList<MobStats>(PlayerDataBase.Collections.MobStats)
                    .FirstOrDefault(x => x.Date.Date.Equals(DateTime.Today));

                if (getTodayMobStats != null)
                {
                    getTodayMobStats.PlayerDeaths += 1;
                }
                else
                {
                    getTodayMobStats = new MobStats()
                    {
                        MobKills = 0,
                        PlayerDeaths = 1,
                        Date = DateTime.Now,
                    };
                }
                Services.Instance.PlayerDataBase.Save(
                    getTodayMobStats,
                    PlayerDataBase.Collections.MobStats
                );
            }

            // take player to Temple / recall area
            if (target.ConnectionId != "mob")
            {
                target.Status = CharacterStatus.Status.Resting;
                var newRoom = Services.Instance.Cache.GetRoom(target.RecallId);
                target.Target = null;
                target.Buffer = new Queue<string>();
                target.RoomId = Helpers.ReturnRoomId(newRoom);
                newRoom.Players.Add(target);
                target.Buffer.Enqueue("look");
            }
        }

        /// <summary>
        /// 40% chance to hit reduce if no blind fighting
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static bool BlindFighting(Player player)
        {
            var foundSkill = player.Skills.FirstOrDefault(x => x.Name == SkillName.BlindFighting);

            if (foundSkill == null)
            {
                return false;
            }

            var getSkill = Services.Instance.Cache.GetSkill(foundSkill.Id);

            if (getSkill == null)
            {
                var skill = Services.Instance.Cache
                    .GetAllSkills()
                    .FirstOrDefault(
                        x =>
                            x.Name.Equals(
                                "blind fighting",
                                StringComparison.CurrentCultureIgnoreCase
                            )
                    );
                foundSkill.Id = skill.Id;
                getSkill = skill;
            }

            var proficiency = foundSkill.Proficiency;
            var success = DiceBag.Roll(1, 1, 100);

            if (success == 1 || success == 101)
            {
                return false;
            }

            //TODO Charisma Check
            if (proficiency >= success)
            {
                return true;
            }

            if (foundSkill.Proficiency == 100)
            {
                return false;
            }

            var increase = DiceBag.Roll(1, 1, 5);

            foundSkill.Proficiency += increase;

            player.GainExperiencePoints(100 * foundSkill.Level / 4, true);

            Services.Instance.UpdateClient.UpdateExp(player);

            return false;
        }
    }
}
