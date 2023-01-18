﻿using System;
using System.Linq;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Client;
using ArchaicQuestII.GameLogic.Combat;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.Spell;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Skill.Skills
{
    public class UtilSkills
    {
        private readonly IClientHandler _clientHandler;
        private readonly ICharacterHandler _characterHandler;
        private readonly ICombatHandler _combatHandler;

        public UtilSkills(
            IClientHandler clientHandler,
            ICombatHandler combatHandler,
            ICharacterHandler characterHandler)
        {
            _clientHandler = clientHandler;
            _combatHandler = combatHandler;
            _characterHandler = characterHandler;
        }
        
        public int Disarm(Player player, Player target, Room room, string obj)
        {
            if (string.IsNullOrEmpty(player.Target))
            {
                _clientHandler.WriteLine("You are not fighting anyone.", player.ConnectionId);

                return 0;
            }

            if (player.Equipped.Wielded == null)
            {
                _clientHandler.WriteLine("You must wield a weapon to disarm.", player.ConnectionId);

                return 0;
            }

            if (target.Equipped.Wielded == null)
            {
                _clientHandler.WriteLine("Your opponent is not wielding a weapon.", player.ConnectionId);

                return 0;
            }

            var playerWeaponSkill = Helpers.GetWeaponSkill(player.Equipped.Wielded, player);

            var targetWeaponSkill = Helpers.GetWeaponSkill(target.Equipped.Wielded, target);

            var playerSkillOfTargetsWeapon = Helpers.GetWeaponSkill(target.Equipped.Wielded, player);

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

            var hasGrip = target.Skills.FirstOrDefault(x =>
                x.SkillName.Equals("grip", StringComparison.CurrentCultureIgnoreCase));

            var gripChance = DiceBag.Roll(1, 1, 100);

            if (hasGrip != null)
            {
                if (gripChance <= hasGrip.Proficiency)
                {
                    chance -= 20;
                }
                else
                {
                    _characterHandler.GainSkillExperience(target, hasGrip.Level * 100, hasGrip, DiceBag.Roll(1, 1, 5));
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
                            ToTarget = $"{player.Name} tries to disarm you, but your weapon won't budge!"
                        }
                    };

                    _characterHandler.EmoteAction(player, target, room, skillMessageNoRemove);
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

                _characterHandler.EmoteAction(player, target, room, skillMessage);

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

                _characterHandler.EmoteAction(player, target, room, skillMessage);

            }

            player.Lag += 1;

            return 0;
        }

        public int Rescue(Player player, Player target, Room room, string obj)
        {
            if (target == null)
            {
                _clientHandler.WriteLine("Rescue whom?", player.ConnectionId);
                return 0;
            }

            if (player.Followers.FirstOrDefault(x => x.Name.Equals(target.Name)) == null)
            {
                _clientHandler.WriteLine("You can only rescue those in your group.", player.ConnectionId);
                return 0;
            }


            if ((target.Status & CharacterStatus.Status.Fighting) == 0)
            {
                _clientHandler.WriteLine($"{target.Name} is not fighting right now.", player.ConnectionId);

                return 0;
            }

            if (target == player)
            {
                _clientHandler.WriteLine("What about fleeing instead?", player.ConnectionId);

                return 0;
            }

            var foundSkill = player.Skills
                .FirstOrDefault(x => x.SkillName.Equals("Rescue", StringComparison.CurrentCultureIgnoreCase));

            var chance = foundSkill
                .Proficiency;

            if (DiceBag.Roll(1, 1, 100) < chance)
            {
                player.Target = target.Target;
                target.Target = string.Empty;
                target.Status = CharacterStatus.Status.Standing;

                var findTarget = Helpers.FindMob(Helpers.findNth($"{player.Target}"), room) ?? Helpers.FindPlayer(Helpers.findNth($"{player.Target}"), room);

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

                _characterHandler.EmoteAction(player, target, room, skillMessage);

                _combatHandler.UpdateCombat(player, findTarget);
                return 0;
            }
            var increase = DiceBag.Roll(1, 1, 5);
            _characterHandler.GainExperiencePoints(player, 100 * foundSkill.Level / 4, false);

            _clientHandler.UpdateExp(player);

            _clientHandler.WriteLine(
                $"<p class='improve'>You learn from your mistakes and gain {100 * foundSkill.Level / 4} experience points.</p>" +
                $"<p class='improve'>Your knowledge of {foundSkill.SkillName} increases by {increase}%.</p>",
                player.ConnectionId, 0);

            return 0;
        }

        public int Berserk(Player player, Player target, Room room)
        {

            if (player.Affects.Berserk)
            {
                _clientHandler.WriteLine("You get a little madder", player.ConnectionId);
                return 0;
            }

            if (player.Attributes.Attribute[EffectLocation.Moves] < 50)
            {
                _clientHandler.WriteLine("You can't get up enough energy.", player.ConnectionId);
                return 0;
            }

            /* below 50% of hp helps, above hurts */

            var foundSkill = player.Skills
                .FirstOrDefault(x => x.SkillName.Equals("Berserk", StringComparison.CurrentCultureIgnoreCase));

            if (foundSkill != null)
            {
                var chance = foundSkill
                    .Proficiency;

                chance += (player.Status & CharacterStatus.Status.Fighting) != 0 ? 10 : 1;
                var hpPercent = 100 * player.Attributes.Attribute[EffectLocation.Hitpoints] /
                                player.MaxAttributes.Attribute[EffectLocation.Hitpoints];
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
                        Name = "Berserk"
                    };
                    target.Affects.Custom.Add(affect);

                    Helpers.ApplyAffects(affect, player);

                    player.Attributes.Attribute[EffectLocation.Moves] = player.Attributes.Attribute[EffectLocation.Moves] /= 2;
                    player.Attributes.Attribute[EffectLocation.Hitpoints] += player.Level * 2;

                    var skillMessage = new SkillMessage()
                    {
                        Hit =
                        {
                            ToPlayer = "Your pulse races as you are consumed by rage!",
                            ToRoom = $"{player.Name} gets a wild look in {Helpers.GetPronoun(player.Gender)} eyes.",
                            ToTarget = ""
                        }
                    };

                    _characterHandler.EmoteAction(player, target, room, skillMessage);


                }
                else
                {
                    _clientHandler.WriteLine("Your pulse speeds up, but nothing happens", target.ConnectionId);

                    player.Attributes.Attribute[EffectLocation.Moves] = player.Attributes.Attribute[EffectLocation.Moves] /= 4;

                    var increase = DiceBag.Roll(1, 1, 5);

                    foundSkill.Proficiency += increase;

                    _characterHandler.GainExperiencePoints(player, 100 * foundSkill.Level / 4, false);

                    _clientHandler.UpdateExp(player);

                    _clientHandler.WriteLine(
                        $"<p class='improve'>You learn from your mistakes and gain {100 * foundSkill.Level / 4} experience points.</p>" +
                        $"<p class='improve'>Your knowledge of {foundSkill.SkillName} increases by {increase}%.</p>",
                        player.ConnectionId, 0);


                }
            }

            player.Lag += 1;
            _clientHandler.UpdateScore(player);
            _clientHandler.UpdateMoves(player);
            _clientHandler.UpdateHP(player);
            _clientHandler.UpdateAffects(player);
            _clientHandler.UpdateExp(player);

            return 0;
        }

        public int Mount(Player player, Player target, Room room)
        {

            if (!string.IsNullOrEmpty(player.Mounted.Name))
            {
                _clientHandler.WriteLine($"You are already riding a mount called {player.Mounted.Name}", player.ConnectionId);
                return 0;
            }

            if (!string.IsNullOrEmpty(target.Mounted.MountedBy) && target.Mounted.MountedBy != player.Name)
            {
                _clientHandler.WriteLine($"This mount is ridden by {target.Mounted.MountedBy}", player.ConnectionId);
                return 0;
            }

            if (!string.IsNullOrEmpty(player.Mounted.Name) && target.Name != player.Mounted.Name)
            {
                _clientHandler.WriteLine($"You must dismount {player.Mounted.Name} first.", player.ConnectionId);
                return 0;
            }

            if (!target.Mounted.IsMount)
            {
                _clientHandler.WriteLine($"{target.Name} is not a mount.", player.ConnectionId);
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

            _characterHandler.EmoteAction(player, target, room, skillMessage);


            _clientHandler.UpdateScore(player);



            return 0;
        }

        public int WarCry(Player player, Player target, Room room)
        {

            if (player.Affects.Custom.FirstOrDefault(x => x.Name.Equals("War Cry")) != null)
            {
                _clientHandler.WriteLine("You are already affected by War Cry.", player.ConnectionId);
                return 0;
            }


            var skillMessage = new SkillMessage()
            {
                Hit =
                {
                    ToPlayer = "You scream a loud war cry.",
                    ToRoom = $"{player.Name} screams a loud war cry.",
                    ToTarget = ""
                }
            };


            _characterHandler.EmoteAction(player, target, room, skillMessage);

            var affect = new Affect()
            {
                Duration = player.Level + player.Level / 5,
                Modifier = new Modifier()
                {
                    DamRoll = 3,
                    Armour = -2

                },
                Affects = DefineSpell.SpellAffect.Berserk,
                Name = "War Cry"
            };

            player.Affects.Custom.Add(affect);

            Helpers.ApplyAffects(affect, player);
            _clientHandler.UpdateScore(player);
            _clientHandler.UpdateMoves(player);
            _clientHandler.UpdateHP(player);
            _clientHandler.UpdateAffects(player);
            _clientHandler.UpdateExp(player);


            return 0;
        }
    }
}
