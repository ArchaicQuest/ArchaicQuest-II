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
    public class DamageSkills
    {
        private readonly IClientHandler _clientHandler;
        private readonly ICharacterHandler _characterHandler;
        private readonly ICombatHandler _combatHandler;

        public DamageSkills(
            IClientHandler clientHandler,
            ICombatHandler combatHandler,
            ICharacterHandler characterHandler)
        {
            _clientHandler = clientHandler;
            _characterHandler = characterHandler;
            _combatHandler = combatHandler;
        }

        public int Kick(Player player, Player target, Room room)
        {
            var str = player.Attributes.Attribute[EffectLocation.Strength];
            var damage = DiceBag.Roll(1, 1, 8) + str / 4;

            _characterHandler.DamagePlayer("Kick", damage, player, target, room);

            player.Lag += 1;


            _combatHandler.UpdateCombat(player, target);

            return damage;
        }

        public int Elbow(Player player, Player target, Room room)
        {
            var str = player.Attributes.Attribute[EffectLocation.Strength];
            var damage = DiceBag.Roll(1, 1, 6) + str / 5;

            _characterHandler.DamagePlayer("elbow", damage, player, target, room);

            player.Lag += 1;


            _combatHandler.UpdateCombat(player, target);

            return damage;
        }

        // TODO skill success check
        public int HeadButt(Player player, Player target, Room room)
        {
            var str = player.Attributes.Attribute[EffectLocation.Strength];
            var damage = DiceBag.Roll(1, 1, 12) + str / 5;

            if (player.Equipped.Head == null)
            {
                damage /= 2;
            }

            _characterHandler.DamagePlayer("headbutt", damage, player, target, room);

            player.Lag += 1;


            _combatHandler.UpdateCombat(player, target);

            return damage;
        }

        public int Charge(Player player, Player target, Room room, string obj)
        {
            if (player.Status == CharacterStatus.Status.Fighting)
            {
                _clientHandler.WriteLine("You are already in combat, Charge can only be used to start a combat.");
                return 0;
            }

            var nthTarget = Helpers.findNth(obj);

            var character = Helpers.FindMob(nthTarget, room) ?? Helpers.FindPlayer(nthTarget, room);


            var weaponDam = player.Equipped.Wielded != null ? player.Equipped.Wielded.Damage.Maximum : 1 * 2;
            var str = player.Attributes.Attribute[EffectLocation.Strength];
            var damage = DiceBag.Roll(1, 1, weaponDam) + str / 5;


            _characterHandler.DamagePlayer("charge", damage, player, target, room);

            player.Lag += 2;

            _combatHandler.UpdateCombat(player, target);

            return damage;
        }

        public int Stab(Player player, Player target, Room room, string obj)
        {
            if (player.Equipped.Wielded == null)
            {
                _clientHandler.WriteLine("Stab with what?", player.ConnectionId);
                return 0;
            }

            //var nthTarget = Helpers.findNth(obj);

            //var character = Helpers.FindMob(nthTarget, room) ?? Helpers.FindPlayer(nthTarget, room);


            var weaponDam = (player.Equipped.Wielded.Damage.Maximum + player.Equipped.Wielded.Damage.Minimum) / 2;
            var str = player.Attributes.Attribute[EffectLocation.Strength];
            var damage = (weaponDam + DiceBag.Roll(1, 1, 6)) + str / 5;


            _characterHandler.DamagePlayer("stab", damage, player, target, room);

            player.Lag += 1;

            _combatHandler.UpdateCombat(player, target);

            return damage;
        }

        public int OverheadCrush(Player player, Player target, Room room, string obj)
        {

            if (player.Equipped.Wielded == null)
            {
                _clientHandler.WriteLine("Overhead crush with what?", player.ConnectionId);
            }

            if (!player.Affects.Stunned || (player.Status & CharacterStatus.Status.Sleeping) == 0 ||
                (player.Status & CharacterStatus.Status.Resting) == 0)
            {
                _clientHandler.WriteLine("You can only use this on stunned or targets that are not prepared.", player.ConnectionId);
                return 0;
            }


            var weaponDam = player.Equipped.Wielded?.Damage.Maximum ?? 1 * 2;
            var str = player.Attributes.Attribute[EffectLocation.Strength];
            var damage = weaponDam + DiceBag.Roll(1, 3, 10) + str / 5;


            _characterHandler.DamagePlayer("overhead crush", damage, player, target, room);

            player.Lag += 1;

            _combatHandler.UpdateCombat(player, target);

            return damage;
        }

        public int Cleave(Player player, Player target, Room room, string obj)
        {
            if (player.Equipped.Wielded == null)
            {
                _clientHandler.WriteLine("Cleave with what?", player.ConnectionId);
                return 0;
            }

            var weaponDam = player.Equipped.Wielded?.Damage.Maximum ?? 1 * 2;
            var str = player.Attributes.Attribute[EffectLocation.Strength];
            var damage = weaponDam + DiceBag.Roll(1, 3, 10) + str / 5;


            _characterHandler.DamagePlayer("cleave", damage, player, target, room);

            player.Lag += 1;

            _combatHandler.UpdateCombat(player, target);

            return 0;

        }

        public int Impale(Player player, Player target, Room room, string obj)
        {
            if (player.Equipped.Wielded == null)
            {
                _clientHandler.WriteLine("Impale with what?", player.ConnectionId);
            }


            var weaponDam = player.Equipped.Wielded?.Damage.Maximum ?? 1 * 2;
            var str = player.Attributes.Attribute[EffectLocation.Strength];
            var damage = weaponDam + DiceBag.Roll(1, 2, 10) + str / 5;

            /*dexterity check */
            var chance = 50;
            chance += player.Attributes.Attribute[EffectLocation.Dexterity];
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

            if (DiceBag.Roll(1, 1, 100) < chance)
            {

                _characterHandler.DamagePlayer("impale", damage, player, target, room);

                player.Lag += 1;

                _combatHandler.UpdateCombat(player, target);

            }
            else
            {
                var skillMessageMiss = new SkillMessage()
                {
                    Hit =
                    {
                        ToPlayer = $"You try to impale {target.Name} and miss.",
                        ToRoom = $"{player.Name} tries to impale {target.Name} but {target.Name} easily avoids it.",
                        ToTarget = $"{player.Name} tries to impale you but misses."
                    }
                };

                _characterHandler.EmoteAction(player, target, room, skillMessageMiss);
            }

            return damage;
        }

        public int Slash(Player player, Player target, Room room, string obj)
        {
            if (player.Equipped.Wielded == null)
            {
                _clientHandler.WriteLine("Slash with what?", player.ConnectionId);
            }


            var weaponDam = player.Equipped.Wielded?.Damage.Maximum ?? 1 * 2;
            var str = player.Attributes.Attribute[EffectLocation.Strength];
            var damage = weaponDam + DiceBag.Roll(1, 2, 10) + str / 5;

            /*dexterity check */
            var chance = 50;
            chance += player.Attributes.Attribute[EffectLocation.Dexterity];
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

            /* TODO: terrain check, can't dirt kick underwater *taps head* */
            /* Check if player is flying/floating then fail dirt kick */

            if (DiceBag.Roll(1, 1, 100) < chance)
            {

                _characterHandler.DamagePlayer("slash", damage, player, target, room);

                player.Lag += 1;

                _combatHandler.UpdateCombat(player, target);

            }
            else
            {
                var skillMessageMiss = new SkillMessage()
                {
                    Hit =
                    {
                        ToPlayer = $"You try to slash {target.Name} and miss.",
                        ToRoom = $"{player.Name} tries to slash {target.Name} but {target.Name} easily avoids it.",
                        ToTarget = $"{player.Name} tries to slash you but misses."
                    }
                };

                _characterHandler.EmoteAction(player, target, room, skillMessageMiss);
            }

            return damage;
        }

        public int Trip(Player player, Player target, Room room)
        {
            var str = player.Attributes.Attribute[EffectLocation.Strength];
            var damage = DiceBag.Roll(1, 1, 4) + str / 5;

            var skillMessage = new SkillMessage()
            {
                Hit =
                {
                    ToPlayer = $"You trip {target.Name} and {target.Name} goes down!",
                    ToRoom = $"{player.Name} trips {target.Name} and {target.Name} goes down!",
                    ToTarget = $"{player.Name} trips you and you go down!"
                },
                Miss =
                {
                    ToPlayer = $"You trip {target.Name} and {target.Name} goes down!",
                    ToRoom = $"{player.Name} trips {target.Name} and {target.Name} goes down!",
                    ToTarget = $"{player.Name} trips you and you go down!"
                }
            };

            if (target.Lag == 0)
            {

                _characterHandler.EmoteAction(player, target, room, skillMessage);

                _characterHandler.DamagePlayer("trip", damage, player, target, room);

                player.Lag += 1;
                target.Lag += 2;

                target.Status = CharacterStatus.Status.Stunned;

                _combatHandler.UpdateCombat(player, target);
            }
            else
            {
                player.Lag += 1;

                var skillMessageMiss = new SkillMessage()
                {
                    Hit =
                    {
                        ToPlayer = $"You try to trip {target.Name} and miss.",
                        ToRoom = $"{player.Name} tries to trip {target.Name} but {target.Name} easily avoids it.",
                        ToTarget = $"{player.Name} tries to trip you but fails."
                    },
                    Miss =
                    {
                        ToPlayer = $"You try to trip {target.Name} and miss.",
                        ToRoom = $"{player.Name} tries to trip {target.Name} but {target.Name} easily avoids it.",
                        ToTarget = $"{player.Name} tries to trip you but fails."
                    }
                };

                _characterHandler.EmoteAction(player, target, room, skillMessageMiss);
            }

            return damage;
        }

        public int UpperCut(Player player, Player target, Room room, string obj)
        {


            var str = player.Attributes.Attribute[EffectLocation.Strength];
            var damage = DiceBag.Roll(1, 1, 6) + str / 5;


            _characterHandler.DamagePlayer("uppercut", damage, player, target, room);

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
                            ToPlayer = $"Your uppercut knocks{helmet.Name.ToLower()} off {target.Name}'s head.",
                            ToRoom = $"{player.Name} knocks {helmet.Name.ToLower()} off {target.Name}'s head.",
                            ToTarget = $"{player.Name} knocks {helmet.Name.ToLower()} off your head."
                        }
                    };

                    _characterHandler.EmoteAction(player, target, room, skillMessage);
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

                    _characterHandler.EmoteAction(player, target, room, skillMessage);

                    target.Lag += 2;
                }
            }

            player.Lag += 1;

            _combatHandler.UpdateCombat(player, target);

            return damage;
        }

        public int DirtKick(Player player, Player target, Room room, string obj)
        {

            if (target.Affects.Blind)
            {
                _clientHandler.WriteLine($"{target.Name} has already been blinded.", player.ConnectionId);
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

                _characterHandler.EmoteAction(player, target, room, skillMessage);

                _clientHandler.WriteLine("You can't see a thing!", target.ConnectionId);

                target.Affects.Blind = true;

                var affect = new Affect()
                {
                    Duration = 2,
                    Modifier = new Modifier()
                    {
                        Dexterity = -4,
                        HitRoll = -4
                    },
                    Affects = DefineSpell.SpellAffect.Blind,
                    Name = "Blindness from dirt kick"
                };

                target.Affects.Custom.Add(affect);

                Helpers.ApplyAffects(affect, target);
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

                _characterHandler.EmoteAction(player, target, room, skillMessage);

            }

            player.Lag += 1;

            _combatHandler.UpdateCombat(player, target);

            _clientHandler.UpdateScore(player);
            _clientHandler.UpdateMoves(player);
            _clientHandler.UpdateHP(player);
            _clientHandler.UpdateAffects(target);
            _clientHandler.UpdateExp(player);


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

            var weaponDam = player.Equipped.Wielded != null ? player.Equipped.Wielded.Damage.Maximum : 1 * 2;
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


                _characterHandler.EmoteAction(player, target, room, skillMessage);
                _characterHandler.DamagePlayer("lunge", damage, player, target, room);
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

                _characterHandler.EmoteAction(player, target, room, skillMessage);


                player.Lag += 2;
            }


            _combatHandler.UpdateCombat(player, target);

            return 0;
        }

        public int ShieldBash(Player player, Player target, Room room, string obj)
        {

            if (player.Equipped.Shield == null)
            {
                _clientHandler.WriteLine("You need a shield before you can bash", player.ConnectionId);
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
            var weaponDam = player.Equipped.Shield != null ? player.Equipped.Shield.ArmourRating.Armour : 1 * 2;
            var str = player.Attributes.Attribute[EffectLocation.Strength];
            var damage = DiceBag.Roll(3, 1, 6) + str / 5 + weaponDam;

            if (DiceBag.Roll(1, 1, 100) < chance)
            {
                var skillMessage = new SkillMessage()
                {
                    Hit =
                    {
                        ToPlayer = $"You lift your shield and smash it at {target.Name}",
                        ToRoom = $"{player.Name} lifts {Helpers.GetPronoun(player.Gender)} shield and smashes it at {target.Name}!",
                        ToTarget = $"{player.Name} lifts {Helpers.GetPronoun(player.Gender)} shield and smashes it at you!"
                    }
                };

                _characterHandler.EmoteAction(player, target, room, skillMessage);
                _characterHandler.DamagePlayer("shield bash", damage, player, target, room);
                target.Lag += 3;
            }
            else
            {
                var skillMessage = new SkillMessage()
                {
                    Hit =
                    {
                        ToPlayer = $"You lift your shield and swing it at {target.Name} but miss.",
                        ToRoom = $"{player.Name} lifts {Helpers.GetPronoun(player.Gender)} shield and swings it at {target.Name} but misses.",
                        ToTarget = $"{player.Name} lifts {Helpers.GetPronoun(player.Gender)} shield and swings it at you but you avoid it easily."
                    }
                };

                _characterHandler.EmoteAction(player, target, room, skillMessage);


                player.Lag += 2;
            }

            player.Lag += 1;


            _combatHandler.UpdateCombat(player, target);

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

            var weaponDam = player.Equipped.Wielded != null ? player.Equipped.Wielded.Damage.Maximum : 1 * 2;
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

                _characterHandler.EmoteAction(player, target, room, skillMessage);
                _characterHandler.DamagePlayer("hamstring slash", damage, player, target, room);

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
                        ToRoom = $"{player.Name} tries to slash the back of {target.Name}'s legs. but misses",
                        ToTarget = $"{player.Name} tries to slash the back of your legs but misses."
                    }
                };

                _characterHandler.EmoteAction(player, target, room, skillMessage);


                player.Lag += 3;
            }

            player.Lag += 2;

            _combatHandler.UpdateCombat(player, target);

            return 0;
        }

    }
}
