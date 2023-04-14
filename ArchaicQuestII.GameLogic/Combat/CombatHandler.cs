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

        /// <summary>
        /// Target Killed
        /// </summary>
        /// <param name="killer">The one who did the killing</param>
        /// <param name="victim">The one who died</param>
        public static void TargetKilled(Combatant combatant, Room room)
        {
            Services.Instance.Writer.WriteLine(
                "<p class='dead'>You are dead. R.I.P.</p>",
                combatant.target
            );

            if (combatant.target.Combat != null)
                combatant.target.Combat.RemoveFromCombat(combatant.target);

            combatant.target.Target = null;
            combatant.target.Status = CharacterStatus.Status.Ghost;
            combatant.target.DeathCry(room);

            if (combatant.player.Grouped)
            {
                // other group members to drop from combat if they're fighting the same target
                // other group members status set to standing

                var isGroupLeader = string.IsNullOrEmpty(combatant.player.Following);

                var groupLeader = combatant.player;

                if (!isGroupLeader)
                {
                    groupLeader = Services.Instance.Cache
                        .GetPlayerCache()
                        .FirstOrDefault(x => x.Value.Name.Equals(combatant.player.Following))
                        .Value;
                }

                var exp = combatant.target.GetExpWorth() / (groupLeader.Followers.Count + 1);
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
                combatant.player.GainExperiencePoints(combatant.target, true);
            }

            Services.Instance.Quest.IsQuestMob(combatant.player, combatant.target.Name);

            if (combatant.target.ConnectionId != "mob")
            {
                Helpers.PostToDiscord(
                    $"{combatant.target.Name} got killed by {combatant.player.Name}!",
                    "event"
                );

                if (combatant.player.ConnectionId != "mob")
                {
                    combatant.target.PlayerDeaths += 1;
                    combatant.player.PlayerKills += 1;
                }
                else
                {
                    combatant.target.MobDeaths += 1;
                }
            }

            var targetName = combatant.target.Name.ToLower(CultureInfo.CurrentCulture);

            var corpse = new Item.Item
            {
                Name = $"The corpse of {targetName}",
                Description = new Description
                {
                    Room = $"The corpse of {targetName} is laying here.",
                    Exam = combatant.target.Description,
                    Look = combatant.target.Description,
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
                Decay = combatant.target.ConnectionId.Equals(
                    "mob",
                    StringComparison.OrdinalIgnoreCase
                )
                    ? 10
                    : 20,
                DecayTimer = 300 // 5 minutes,
            };

            foreach (var item in combatant.target.Inventory)
            {
                item.Equipped = false;
                corpse.Container.Items.Add(item);
            }

            // clear list
            combatant.target.Inventory = new ItemList();
            // clear equipped
            combatant.target.Equipped = new Equipment();

            var mount = combatant.target.Pets.FirstOrDefault(
                x => x.Name.Equals(combatant.target.Mounted.Name)
            );
            if (mount != null)
            {
                combatant.target.Pets.Remove(mount);
                combatant.target.Mounted.Name = string.Empty;
            }

            // add corpse to room
            room.Items.Add(corpse);

            room.Clean = false;

            if (
                combatant.target.ConnectionId.Equals(
                    "mob",
                    StringComparison.CurrentCultureIgnoreCase
                )
            )
            {
                combatant.player.MobKills += 1;

                var randomItem = Services.Instance.RandomItem.WeaponDrop(combatant.player);

                if (randomItem != null)
                {
                    corpse.Container.Items.Add(randomItem);
                }

                var command = Services.Instance.Cache.GetCommand("get");
                if (combatant.player.Config.AutoLoot && command != null)
                {
                    var corpseIndex = room.Items.IndexOf(corpse) + 1;
                    command.Execute(
                        combatant.player,
                        room,
                        new[] { "get", "all", $"{corpseIndex}.corpse" }
                    );
                }

                command = Services.Instance.Cache.GetCommand("sacrifice");
                if (combatant.player.Config.AutoSacrifice && command != null)
                {
                    command.Execute(combatant.player, room, new[] { "sacrifice", corpse.Name });
                }

                room.Mobs.Remove(combatant.target);

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
                room.Players.Remove(combatant.target);
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

            Services.Instance.UpdateClient.UpdateScore(combatant.player);

            // take player to Temple / recall area
            if (combatant.target.ConnectionId != "mob")
            {
                combatant.target.Status = CharacterStatus.Status.Resting;
                var newRoom = Services.Instance.Cache.GetRoom(combatant.target.RecallId);
                combatant.target.RoomId = Helpers.ReturnRoomId(newRoom);
                newRoom.Players.Add(combatant.target);
                combatant.target.Attributes.Attribute[EffectLocation.Hitpoints] = 1;
                combatant.target.UpdateClientUI();
                Services.Instance.UpdateClient.UpdateInventory(combatant.target);
                Services.Instance.UpdateClient.UpdateEquipment(combatant.target);
                combatant.target.Buffer = new Queue<string>();
                combatant.target.Buffer.Enqueue("look");
            }
        }
    }
}
