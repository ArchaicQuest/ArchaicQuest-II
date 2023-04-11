using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.Skill.Core;
using ArchaicQuestII.GameLogic.Spell;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Skill.Skills
{
    public interface IDamageSkills
    {
        int Kick(Player player, Player target, Room room);
        int Elbow(Player player, Player target, Room room);
        int Trip(Player player, Player target, Room room);
        int HeadButt(Player player, Player target, Room room);
        int Charge(Player player, Player target, Room room, string obj);
        int Stab(Player player, Player target, Room room, string obj);
        int OverheadCrush(Player player, Player target, Room room, string obj);
        int Cleave(Player player, Player target, Room room, string obj);
        int Impale(Player player, Player target, Room room, string obj);
        int Slash(Player player, Player target, Room room, string obj);
        int UpperCut(Player player, Player target, Room room, string obj);
        int DirtKick(Player player, Player target, Room room, string obj);
        int Lunge(Player player, Player target, Room room, string obj);
        int ShieldBash(Player player, Player target, Room room, string obj);
        int HamString(Player player, Player target, Room room, string obj);
    }

    public class DamageSkills : IDamageSkills
    {
        private readonly ISkillManager _skillManager;

        public DamageSkills(ISkillManager skillManager)
        {
            _skillManager = skillManager;
        }

        public int Kick(Player player, Player target, Room room)
        {
            return 0;
        }

        public int Elbow(Player player, Player target, Room room)
        {
            return 0;
        }

        // TODO skill success check
        public int HeadButt(Player player, Player target, Room room)
        {
            return 0;
        }

        public int Charge(Player player, Player target, Room room, string obj)
        {
            return 0;
        }

        public int Stab(Player player, Player target, Room room, string obj)
        {
            return 0;
        }

        public int OverheadCrush(Player player, Player target, Room room, string obj)
        {
            return 0;
        }

        public int Cleave(Player player, Player target, Room room, string obj)
        {
            return 0;
        }

        public int Impale(Player player, Player target, Room room, string obj)
        {
            return 0;
        }

        public int Slash(Player player, Player target, Room room, string obj)
        {
            return 0;
        }

        public int Trip(Player player, Player target, Room room)
        {
            return 0;
        }

        public int UpperCut(Player player, Player target, Room room, string obj)
        {
            var str = player.Attributes.Attribute[EffectLocation.Strength];
            var damage = DiceBag.Roll(1, 1, 6) + str / 5;

            _skillManager.DamagePlayer("uppercut", damage, player, target, room);

            var helmet = target.Equipped.Head;
            var chance = DiceBag.Roll(1, 1, 100);
            if (helmet != null)
            {
                if (chance <= 15)
                {
                    room.Items.Add(helmet);
                    target.Equipped.Head = null;

                    var skillMessage = new SkillMessage()
                    {
                        Hit =
                        {
                            ToPlayer =
                                $"Your uppercut knocks{helmet.Name.ToLower()} off {target.Name}'s head.",
                            ToRoom =
                                $"{player.Name} knocks {helmet.Name.ToLower()} off {target.Name}'s head.",
                            ToTarget =
                                $"{player.Name} knocks {helmet.Name.ToLower()} off your head."
                        }
                    };

                    _skillManager.EmoteAction(player, target, room, skillMessage);
                }
            }
            else
            {
                if (chance <= 15)
                {
                    var skillMessage = new SkillMessage()
                    {
                        Hit =
                        {
                            ToPlayer = $"Your uppercut stuns {target.Name}.",
                            ToRoom = $"{player.Name}'s uppercut stuns {target.Name}.",
                            ToTarget = $"{player.Name}'s uppercut stuns you!"
                        }
                    };

                    _skillManager.EmoteAction(player, target, room, skillMessage);

                    target.Lag += 2;
                }
            }

            player.Lag += 1;

            _skillManager.updateCombat(player, target, room);

            return damage;
        }

        public int DirtKick(Player player, Player target, Room room, string obj)
        {
            if (target.Affects.Blind)
            {
                Services.Instance.Writer.WriteLine(
                    $"{target.Name} has already been blinded.",
                    player
                );
            }

            /*dexterity check */
            var chance = 50;
            chance += player.Attributes.Attribute[EffectLocation.Dexterity];
            chance -= target.Attributes.Attribute[EffectLocation.Dexterity] / 2;

            if (player.Affects.Haste)
            {
                chance += 10;
            }

            if (target.Affects.Haste)
            {
                chance -= 25;
            }

            /* level check */
            chance += player.Level - target.Level;

            /* TODO: terrain check, can't dirt kick underwater *taps head* */
            /* Check if player is flying/floating then fail dirt kick */

            if (DiceBag.Roll(1, 1, 100) < chance)
            {
                var skillMessage = new SkillMessage()
                {
                    Hit =
                    {
                        ToPlayer = $"You kick dirt in {target.Name}'s eyes!",
                        ToRoom = $"{player.Name} kicks dirt into {target.Name}'s eyes!",
                        ToTarget = $"{player.Name} kicks dirt into your eyes!"
                    }
                };

                _skillManager.EmoteAction(player, target, room, skillMessage);

                Services.Instance.Writer.WriteLine("You can't see a thing!", target);

                target.Affects.Blind = true;

                var affect = new Affect()
                {
                    Duration = 2,
                    Modifier = new Modifier() { Dexterity = -4, HitRoll = -4 },
                    Affects = DefineSpell.SpellAffect.Blind,
                    Name = "Blindness from dirt kick"
                };

                target.Affects.Custom.Add(affect);

                target.ApplyAffects(affect);
            }
            else
            {
                var skillMessage = new SkillMessage()
                {
                    Hit =
                    {
                        ToPlayer = $"You kick dirt but {target.Name} shuts his eyes in time.",
                        ToRoom = $"{player.Name} tries to kick dirt in {target.Name} eyes.",
                        ToTarget = $"{player.Name} tries to kick dirt into your eyes."
                    }
                };

                _skillManager.EmoteAction(player, target, room, skillMessage);
            }

            player.Lag += 1;

            _skillManager.updateCombat(player, target, room);

            Services.Instance.UpdateClient.UpdateScore(player);
            Services.Instance.UpdateClient.UpdateMoves(player);
            Services.Instance.UpdateClient.UpdateHP(player);
            Services.Instance.UpdateClient.UpdateAffects(target);
            Services.Instance.UpdateClient.UpdateExp(player);

            return 0;
        }

        public int Lunge(Player player, Player target, Room room, string obj)
        {
            /*dexterity check */
            var chance = 50;
            chance += player.Attributes.Attribute[EffectLocation.Strength];
            chance -= target.Attributes.Attribute[EffectLocation.Dexterity];

            if (player.Affects.Haste)
            {
                chance += 10;
            }

            if (target.Affects.Haste)
            {
                chance -= 25;
            }

            /* level check */
            chance += player.Level - target.Level;

            var weaponDam =
                player.Equipped.Wielded != null ? player.Equipped.Wielded.Damage.Maximum : 1 * 2;
            var str = player.Attributes.Attribute[EffectLocation.Strength];
            var damage = DiceBag.Roll(3, 1, 6) + str / 5 + weaponDam;

            if (DiceBag.Roll(1, 1, 100) < chance)
            {
                var skillMessage = new SkillMessage()
                {
                    Hit =
                    {
                        ToPlayer = $"You lunge at {target.Name}",
                        ToRoom = $"{player.Name} lunges at {target.Name}!",
                        ToTarget = $"{player.Name} lunges at you!"
                    }
                };
                player.Lag += 1;

                _skillManager.EmoteAction(player, target, room, skillMessage);
                _skillManager.DamagePlayer("lunge", damage, player, target, room);
            }
            else
            {
                var skillMessage = new SkillMessage()
                {
                    Hit =
                    {
                        ToPlayer = $"You lunge at {target.Name} but miss.",
                        ToRoom = $"{player.Name} lunges at {target.Name} but misses.",
                        ToTarget = $"{player.Name}lunges at you but you avoid it easily."
                    }
                };

                _skillManager.EmoteAction(player, target, room, skillMessage);

                player.Lag += 2;
            }

            _skillManager.updateCombat(player, target, room);

            return 0;
        }

        public int ShieldBash(Player player, Player target, Room room, string obj)
        {
            if (player.Equipped.Shield == null)
            {
                Services.Instance.Writer.WriteLine("You need a shield before you can bash", player);
                return 0;
            }

            /*dexterity check */
            var chance = 50;
            chance += player.Attributes.Attribute[EffectLocation.Strength];
            chance -= target.Attributes.Attribute[EffectLocation.Dexterity];

            if (player.Affects.Haste)
            {
                chance += 10;
            }

            if (target.Affects.Haste)
            {
                chance -= 25;
            }

            if (target.Weight > player.Weight)
            {
                chance -= 10;
            }
            else
            {
                chance += 5;
            }

            if (target.Equipped.Shield != null)
            {
                chance += 15;
            }

            /* level check */
            chance += player.Level - target.Level;
            var weaponDam =
                player.Equipped.Shield != null ? player.Equipped.Shield.ArmourRating.Armour : 1 * 2;
            var str = player.Attributes.Attribute[EffectLocation.Strength];
            var damage = DiceBag.Roll(3, 1, 6) + str / 5 + weaponDam;

            if (DiceBag.Roll(1, 1, 100) < chance)
            {
                var skillMessage = new SkillMessage()
                {
                    Hit =
                    {
                        ToPlayer = $"You lift your shield and smash it at {target.Name}",
                        ToRoom =
                            $"{player.Name} lifts {player.ReturnPronoun()} shield and smashes it at {target.Name}!",
                        ToTarget =
                            $"{player.Name} lifts {player.ReturnPronoun()} shield and smashes it at you!"
                    }
                };

                _skillManager.EmoteAction(player, target, room, skillMessage);
                _skillManager.DamagePlayer("shield bash", damage, player, target, room);
                target.Lag += 3;
            }
            else
            {
                var skillMessage = new SkillMessage()
                {
                    Hit =
                    {
                        ToPlayer = $"You lift your shield and swing it at {target.Name} but miss.",
                        ToRoom =
                            $"{player.Name} lifts {player.ReturnPronoun()} shield and swings it at {target.Name} but misses.",
                        ToTarget =
                            $"{player.Name} lifts {player.ReturnPronoun()} shield and swings it at you but you avoid it easily."
                    }
                };

                _skillManager.EmoteAction(player, target, room, skillMessage);

                player.Lag += 2;
            }

            player.Lag += 1;

            _skillManager.updateCombat(player, target, room);

            return 0;
        }

        public int HamString(Player player, Player target, Room room, string obj)
        {
            /*dexterity check */
            var chance = 50;
            chance += player.Attributes.Attribute[EffectLocation.Strength];
            chance -= target.Attributes.Attribute[EffectLocation.Dexterity];

            if (player.Affects.Haste)
            {
                chance += 10;
            }

            if (target.Affects.Haste)
            {
                chance -= 25;
            }

            /* level check */
            chance += player.Level - target.Level;

            var weaponDam =
                player.Equipped.Wielded != null ? player.Equipped.Wielded.Damage.Maximum : 1 * 2;
            var str = player.Attributes.Attribute[EffectLocation.Strength];
            var damage = DiceBag.Roll(2, 1, 6) + str / 5 + weaponDam;

            if (DiceBag.Roll(1, 1, 100) < chance)
            {
                var skillMessage = new SkillMessage()
                {
                    Hit =
                    {
                        ToPlayer = $"You slash the back of {target.Name}'s legs.",
                        ToRoom = $"{player.Name} slashes the back of {target.Name}'s legs!",
                        ToTarget = $"{player.Name} slashes the back of your legs!"
                    }
                };

                _skillManager.EmoteAction(player, target, room, skillMessage);
                _skillManager.DamagePlayer("hamstring slash", damage, player, target, room);

                target.Attributes.Attribute[EffectLocation.Moves] -=
                    target.Attributes.Attribute[EffectLocation.Moves] / 2;

                if (target.Attributes.Attribute[EffectLocation.Moves] < 0)
                {
                    target.Attributes.Attribute[EffectLocation.Moves] = 0;
                }
            }
            else
            {
                var skillMessage = new SkillMessage()
                {
                    Hit =
                    {
                        ToPlayer = $"You try to slash the back of {target.Name}'s legs but miss.",
                        ToRoom =
                            $"{player.Name} tries to slash the back of {target.Name}'s legs. but misses",
                        ToTarget = $"{player.Name} tries to slash the back of your legs but misses."
                    }
                };

                _skillManager.EmoteAction(player, target, room, skillMessage);

                player.Lag += 3;
            }

            player.Lag += 2;

            _skillManager.updateCombat(player, target, room);

            return 0;
        }
    }
}
