using System.Linq;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.Skill.Core;
using ArchaicQuestII.GameLogic.Spell;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Skill.Skills
{
    public interface IUtilSkills
    {
        int Disarm(Player player, Player target, Room room, string obj);
        int Rescue(Player player, Player target, Room room, string obj);
        int Mount(Player player, Player target, Room room);
        int Berserk(Player player, Player target, Room room);
        int WarCry(Player player, Player target, Room room);
    }

    public class UtilSkills : IUtilSkills
    {
        private readonly ISkillManager _skillManager;

        public UtilSkills(ISkillManager skillManager)
        {
            _skillManager = skillManager;
        }

        public int Disarm(Player player, Player target, Room room, string obj)
        {
            if (string.IsNullOrEmpty(player.Target))
            {
                Services.Instance.Writer.WriteLine(
                    "You are not fighting anyone.",
                    player.ConnectionId
                );

                return 0;
            }

            if (player.Equipped.Wielded == null)
            {
                Services.Instance.Writer.WriteLine(
                    "You must wield a weapon to disarm.",
                    player.ConnectionId
                );

                return 0;
            }

            if (target.Equipped.Wielded == null)
            {
                Services.Instance.Writer.WriteLine(
                    "Your opponent is not wielding a weapon.",
                    player.ConnectionId
                );

                return 0;
            }

            var playerWeaponSkill = player.GetWeaponSkill(player.Equipped.Wielded);

            var targetWeaponSkill = target.GetWeaponSkill(target.Equipped.Wielded);

            var playerSkillOfTargetsWeapon = player.GetWeaponSkill(target.Equipped.Wielded);

            var chance = playerWeaponSkill;

            chance += (playerSkillOfTargetsWeapon - targetWeaponSkill) / 2;

            /* dex vs. strength */
            chance += player.Attributes.Attribute[EffectLocation.Dexterity];
            chance -= target.Attributes.Attribute[EffectLocation.Strength];

            chance += (player.Level - target.Level) * 2;
            if (player.Affects.Haste)
            {
                chance += 10;
            }

            if (target.Affects.Haste)
            {
                chance -= 25;
            }

            var hasGrip = target.Skills.FirstOrDefault(x => x.Name == SkillName.Grip);

            var gripChance = DiceBag.Roll(1, 1, 100);

            if (hasGrip != null)
            {
                if (gripChance <= hasGrip.Proficiency)
                {
                    chance -= 20;
                }
                else
                {
                    target.FailedSkill(SkillName.Grip, false);
                }
            }

            if (DiceBag.Roll(1, 1, 100) < chance)
            {
                if ((target.Equipped.Wielded.ItemFlag & Item.Item.ItemFlags.Noremove) != 0)
                {
                    var skillMessageNoRemove = new SkillMessage()
                    {
                        Hit =
                        {
                            ToPlayer = $"{target.Name}'s weapon won't budge!",
                            ToRoom = $"{player.Name} tries to disarm {target.Name}, but fails.",
                            ToTarget =
                                $"{player.Name} tries to disarm you, but your weapon won't budge!"
                        }
                    };

                    _skillManager.EmoteAction(player, target, room, skillMessageNoRemove);
                    return 0;
                }

                var skillMessage = new SkillMessage()
                {
                    Hit =
                    {
                        ToPlayer = $"You disarm {target.Name}!",
                        ToRoom = $"{player.Name} disarms {target.Name}!",
                        ToTarget = $"{player.Name} DISARMS you and sends your weapon flying!"
                    }
                };

                _skillManager.EmoteAction(player, target, room, skillMessage);

                room.Items.Add(target.Equipped.Wielded);
                target.Equipped.Wielded = null;
            }
            else
            {
                var skillMessage = new SkillMessage()
                {
                    Hit =
                    {
                        ToPlayer = $"You fail to disarm {target.Name}.",
                        ToRoom = $"{player.Name} tries to disarm {target.Name}, but fails.",
                        ToTarget = $"{player.Name} tries to disarm you, but fails."
                    }
                };

                _skillManager.EmoteAction(player, target, room, skillMessage);
            }

            player.Lag += 1;

            return 0;
        }

        public int Rescue(Player player, Player target, Room room, string obj)
        {
            if (target == null)
            {
                Services.Instance.Writer.WriteLine("Rescue whom?", player.ConnectionId);
                return 0;
            }

            if (player.Followers.FirstOrDefault(x => x.Name.Equals(target.Name)) == null)
            {
                Services.Instance.Writer.WriteLine(
                    "You can only rescue those in your group.",
                    player.ConnectionId
                );
                return 0;
            }

            if ((target.Status & CharacterStatus.Status.Fighting) == 0)
            {
                Services.Instance.Writer.WriteLine(
                    $"{target.Name} is not fighting right now.",
                    player.ConnectionId
                );

                return 0;
            }

            if (target == player)
            {
                Services.Instance.Writer.WriteLine(
                    "What about fleeing instead?",
                    player.ConnectionId
                );

                return 0;
            }

            var foundSkill = player.Skills.FirstOrDefault(x => x.Name == SkillName.Rescue);

            var chance = foundSkill.Proficiency;

            if (DiceBag.Roll(1, 1, 100) < chance)
            {
                player.Target = target.Target;
                target.Target = string.Empty;
                target.Status = CharacterStatus.Status.Standing;

                var findTarget =
                    Helpers.FindMob(Helpers.findNth($"{player.Target}"), room)
                    ?? Helpers.FindPlayer(Helpers.findNth($"{player.Target}"), room);

                findTarget.Target = player.Name;

                var skillMessage = new SkillMessage()
                {
                    Hit =
                    {
                        ToPlayer = $"You rescue {target.Name}!",
                        ToRoom = $"{player.Name} rescues {target.Name}!",
                        ToTarget = $"{player.Name} rescues you!"
                    }
                };

                _skillManager.EmoteAction(player, target, room, skillMessage);

                _skillManager.updateCombat(player, findTarget, room);
                return 0;
            }

            player.FailedSkill(SkillName.Rescue, true);

            return 0;
        }

        public int Berserk(Player player, Player target, Room room)
        {
            if (player.Affects.Berserk)
            {
                Services.Instance.Writer.WriteLine("You get a little madder", player.ConnectionId);
                return 0;
            }

            if (player.Attributes.Attribute[EffectLocation.Moves] < 50)
            {
                Services.Instance.Writer.WriteLine(
                    "You can't get up enough energy.",
                    player.ConnectionId
                );
                return 0;
            }

            /* below 50% of hp helps, above hurts */

            var foundSkill = player.Skills.FirstOrDefault(x => x.Name == SkillName.Berserk);

            if (foundSkill != null)
            {
                var chance = foundSkill.Proficiency;

                chance += (player.Status & CharacterStatus.Status.Fighting) != 0 ? 10 : 1;
                var hpPercent =
                    100
                    * player.Attributes.Attribute[EffectLocation.Hitpoints]
                    / player.MaxAttributes.Attribute[EffectLocation.Hitpoints];
                chance += 25 - hpPercent / 2;

                if (DiceBag.Roll(1, 1, 100) < chance)
                {
                    target.Affects.Berserk = true;
                    var affect = new Affect()
                    {
                        Duration = 4 + player.Level / 5,
                        Modifier = new Modifier()
                        {
                            DamRoll = 4 + player.Level / 5,
                            HitRoll = 4 + player.Level / 8,
                            Armour = 4 + player.Level / 5
                        },
                        Affects = DefineSpell.SpellAffect.Berserk,
                        Name = SkillName.Berserk.ToString()
                    };
                    target.Affects.Custom.Add(affect);

                    player.ApplyAffects(affect);

                    player.Attributes.Attribute[EffectLocation.Moves] = player.Attributes.Attribute[
                        EffectLocation.Moves
                    ] /= 2;
                    player.Attributes.Attribute[EffectLocation.Hitpoints] += player.Level * 2;

                    var skillMessage = new SkillMessage()
                    {
                        Hit =
                        {
                            ToPlayer = "Your pulse races as you are consumed by rage!",
                            ToRoom =
                                $"{player.Name} gets a wild look in {player.ReturnPronoun()} eyes.",
                            ToTarget = ""
                        }
                    };

                    _skillManager.EmoteAction(player, target, room, skillMessage);
                }
                else
                {
                    Services.Instance.Writer.WriteLine(
                        "Your pulse speeds up, but nothing happens",
                        target.ConnectionId
                    );

                    player.Attributes.Attribute[EffectLocation.Moves] = player.Attributes.Attribute[
                        EffectLocation.Moves
                    ] /= 4;

                    player.FailedSkill(SkillName.Berserk, true);
                }
            }

            player.Lag += 1;
            Services.Instance.UpdateClient.UpdateScore(player);
            Services.Instance.UpdateClient.UpdateMoves(player);
            Services.Instance.UpdateClient.UpdateHP(player);
            Services.Instance.UpdateClient.UpdateAffects(player);
            Services.Instance.UpdateClient.UpdateExp(player);

            return 0;
        }

        public int Mount(Player player, Player target, Room room)
        {
            if (!string.IsNullOrEmpty(player.Mounted.Name))
            {
                Services.Instance.Writer.WriteLine(
                    $"You are already riding a mount called {player.Mounted.Name}",
                    player.ConnectionId
                );
                return 0;
            }

            if (
                !string.IsNullOrEmpty(target.Mounted.MountedBy)
                && target.Mounted.MountedBy != player.Name
            )
            {
                Services.Instance.Writer.WriteLine(
                    $"This mount is ridden by {target.Mounted.MountedBy}",
                    player.ConnectionId
                );
                return 0;
            }

            if (!string.IsNullOrEmpty(player.Mounted.Name) && target.Name != player.Mounted.Name)
            {
                Services.Instance.Writer.WriteLine(
                    $"You must dismount {player.Mounted.Name} first.",
                    player.ConnectionId
                );
                return 0;
            }

            if (!target.Mounted.IsMount)
            {
                Services.Instance.Writer.WriteLine(
                    $"{target.Name} is not a mount.",
                    player.ConnectionId
                );
                return 0;
            }

            var skillMessage = new SkillMessage()
            {
                Hit =
                {
                    ToPlayer = $"You jump upon {target.Name}.",
                    ToRoom = $"{player.Name} jumps upon {target.Name}.",
                    ToTarget = $""
                }
            };

            target.Mounted.MountedBy = player.Name;
            player.Mounted.Name = target.Name;
            player.Pets.Add(target);

            _skillManager.EmoteAction(player, target, room, skillMessage);

            Services.Instance.UpdateClient.UpdateScore(player);

            return 0;
        }

        public int WarCry(Player player, Player target, Room room)
        {
            if (player.Affects.Custom.FirstOrDefault(x => x.Name.Equals("War Cry")) != null)
            {
                Services.Instance.Writer.WriteLine(
                    "You are already affected by War Cry.",
                    player.ConnectionId
                );
                return 0;
            }

            var skillMessage = new SkillMessage()
            {
                Hit =
                {
                    ToPlayer = $"You scream a loud war cry.",
                    ToRoom = $"{player.Name} screams a loud war cry.",
                    ToTarget = $""
                }
            };

            _skillManager.EmoteAction(player, target, room, skillMessage);

            var affect = new Affect()
            {
                Duration = player.Level + player.Level / 5,
                Modifier = new Modifier() { DamRoll = 3, Armour = -2 },
                Affects = DefineSpell.SpellAffect.Berserk,
                Name = "War Cry"
            };

            player.Affects.Custom.Add(affect);

            player.ApplyAffects(affect);
            Services.Instance.UpdateClient.UpdateScore(player);
            Services.Instance.UpdateClient.UpdateMoves(player);
            Services.Instance.UpdateClient.UpdateHP(player);
            Services.Instance.UpdateClient.UpdateAffects(player);
            Services.Instance.UpdateClient.UpdateExp(player);

            return 0;
        }
    }
}
