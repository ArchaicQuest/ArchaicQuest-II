using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Combat;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.Skill.Enum;
using ArchaicQuestII.GameLogic.Skill.Model;
using ArchaicQuestII.GameLogic.Spell.Interface;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Spell.Spells.DamageSpells
{
   public class DamageSpells: IDamageSpells
    {
        private readonly IWriteToClient _writer;
        private readonly IUpdateClientUI _updateClientUi;
        private readonly IDice _dice;
        private readonly IDamage _damage;
        private readonly ICombat _fight;



        public DamageSpells(IWriteToClient writer, IUpdateClientUI updateClientUi, IDice dice, IDamage damage, ICombat fight)
        {
            _writer = writer;
            _updateClientUi = updateClientUi;
            _dice = dice;
            _damage = damage;
            _fight = fight;

        }

        public Player GetValidTarget(Player player, Player target, ValidTargets validTargets)
        {

            var setTarget = target;
            if (validTargets.HasFlag(ValidTargets.TargetFightSelf) && player.Status == CharacterStatus.Status.Fighting)
            {
                setTarget = player;
            }

            if (validTargets.HasFlag(ValidTargets.TargetFightVictim) && player.Status == CharacterStatus.Status.Fighting)
            {
                setTarget = target;
            }

            if (validTargets == ValidTargets.TargetIgnore)
            {
                setTarget = player;
            }

            return setTarget;
        }

        public void updateCombat(Player player, Player target)
        {
            if (string.IsNullOrEmpty(target.Target))
            {
                target.Target = player.Name;
                target.Status = CharacterStatus.Status.Fighting;
            }

            if (string.IsNullOrEmpty(player.Target))
            {
                player.Target = target.Name;
                player.Status = CharacterStatus.Status.Fighting;
            }

        }

        public string ReplacePlaceholders(string str, Player player, bool isTarget)
        {
            var newString = String.Empty;
            if (isTarget)
            {
                newString = str.Replace("#target#", "You");

                return newString;
            }

            newString = str.Replace("#target#", player.Name);

            return newString;

        }

        public void DamagePlayer(string spellName, int damage, Player player, Player target, Room room)
        {
            _writer.WriteLine(
                $"<p>Your {spellName} {_damage.DamageText(damage).Value} {target.Name} ({damage})</p>",
                player.ConnectionId);
            _writer.WriteLine(
                $"<p>{player.Name}'s {spellName} {_damage.DamageText(damage).Value} you! ({damage})</p>",
                target.ConnectionId);

            foreach (var pc in room.Players)
            {
                if (pc.ConnectionId.Equals(player.ConnectionId) ||
                    pc.ConnectionId.Equals(target.ConnectionId))
                {
                    continue;
                }

                _writer.WriteLine($"<p>{ player.Name}'s {spellName} {_damage.DamageText(damage).Value} {target.Name} ({damage}))</p>",
                    pc.ConnectionId);

            }

            target.Attributes.Attribute[EffectLocation.Hitpoints] -= damage;

            if (!_fight.IsTargetAlive(target))
            {
                _fight.DeathCry(room, target);
                //TODO: create corpse, refactor fight method from combat.cs
            }

            //update UI
            _updateClientUi.UpdateHP(target);

            _fight.AddCharToCombat(target);
            _fight.AddCharToCombat(player);
        }

        /*
         * Message for when attribute is full
         * message for player
         * message for target
         * message for room
         *
         */

        public bool AffectPlayerAttributes(string spellName, EffectLocation attribute, int value, Player player, Player target, Room room, string noAffect)
        {

            if ((attribute == EffectLocation.Hitpoints || attribute == EffectLocation.Mana || attribute == EffectLocation.Moves) && target.Attributes.Attribute[attribute] == target.MaxAttributes.Attribute[attribute])
            {
               _writer.WriteLine(ReplacePlaceholders(noAffect, target, false), player.ConnectionId);
               return false;
            }

            target.Attributes.Attribute[attribute] += value;

            if ((attribute == EffectLocation.Hitpoints || attribute ==  EffectLocation.Mana || attribute == EffectLocation.Moves) && target.Attributes.Attribute[attribute] > target.MaxAttributes.Attribute[attribute])
            {
                target.Attributes.Attribute[attribute] = target.MaxAttributes.Attribute[attribute];
            }

            return true;

        }

        /// <summary>
        /// Adds affects to player
        /// Bless
        /// HitRoll +10
        /// DamRoll + 5
        /// </summary>
        /// <param name="spellAffects"></param>
        /// <param name="player"></param>
        /// <param name="target"></param>
        /// <param name="room"></param>
        public void AddAffectToPlayer(List<Affect> spellAffects, Player player, Player target, Room room)
        {
            foreach (var affects in spellAffects)
            {
                var hasEffect = target.Affects.Custom.FirstOrDefault(x => x.Name.Equals(affects.Name));
                if (hasEffect != null)
                {
                    hasEffect.Duration = affects.Duration;
                }
                else
                {
                    target.Affects.Custom.Add(new Affect()
                    {
                        Modifier = affects.Modifier,
                        Benefits = affects.Benefits,
                        Affects = affects.Affects,
                        Duration = player.Level + player.Attributes.Attribute[EffectLocation.Intelligence] / 2,
                        Name = affects.Name
                    });

                    if (affects.Affects == DefineSpell.SpellAffect.Blind)
                    {
                        target.Affects.Blind = true;
                    }

                    //apply affects to target
                    if (affects.Modifier.Strength != 0)
                    {
                        target.Attributes.Attribute[EffectLocation.Strength] += affects.Modifier.Strength;
                    }


                    if (affects.Modifier.Dexterity != 0)
                    {
                        target.Attributes.Attribute[EffectLocation.Dexterity] += affects.Modifier.Dexterity;
                    }

                    if (affects.Modifier.Charisma != 0)
                    {
                        target.Attributes.Attribute[EffectLocation.Charisma] += affects.Modifier.Charisma;
                    }

                    if (affects.Modifier.Constitution != 0)
                    {
                        target.Attributes.Attribute[EffectLocation.Constitution] += affects.Modifier.Constitution;
                    }

                    if (affects.Modifier.Intelligence != 0)
                    {
                        target.Attributes.Attribute[EffectLocation.Intelligence] += affects.Modifier.Intelligence;
                    }

                    if (affects.Modifier.Wisdom != 0)
                    {
                        target.Attributes.Attribute[EffectLocation.Wisdom] += affects.Modifier.Wisdom;
                    }

                    if (affects.Modifier.DamRoll != 0)
                    {
                        target.Attributes.Attribute[EffectLocation.DamageRoll] += affects.Modifier.DamRoll;
                    }

                    if (affects.Modifier.HitRoll != 0)
                    {
                        target.Attributes.Attribute[EffectLocation.HitRoll] += affects.Modifier.HitRoll;
                    }

                    if (affects.Modifier.HP != 0)
                    {
                        target.Attributes.Attribute[EffectLocation.Hitpoints] += affects.Modifier.HP;

                        if (target.Attributes.Attribute[EffectLocation.Hitpoints] >
                            target.MaxAttributes.Attribute[EffectLocation.Hitpoints])
                        {
                            target.Attributes.Attribute[EffectLocation.Hitpoints] =
                                target.MaxAttributes.Attribute[EffectLocation.Hitpoints];
                        }
                    }

                    if (affects.Modifier.Mana != 0)
                    {
                        target.Attributes.Attribute[EffectLocation.Mana] += affects.Modifier.Mana;

                        if (target.Attributes.Attribute[EffectLocation.Mana] >
                            target.MaxAttributes.Attribute[EffectLocation.Mana])
                        {
                            target.Attributes.Attribute[EffectLocation.Mana] =
                                target.MaxAttributes.Attribute[EffectLocation.Mana];
                        }
                    }

                    if (affects.Modifier.Moves != 0)
                    {
                        target.Attributes.Attribute[EffectLocation.Moves] += affects.Modifier.Moves;

                        if (target.Attributes.Attribute[EffectLocation.Moves] >
                            target.MaxAttributes.Attribute[EffectLocation.Moves])
                        {
                            target.Attributes.Attribute[EffectLocation.Moves] =
                                target.MaxAttributes.Attribute[EffectLocation.Moves];
                        }
                    }

                }
            }

            _updateClientUi.UpdateAffects(target);
            _updateClientUi.UpdateScore(target);
        }

        public void UpdateClientUI(Player player)
        {
            //update UI
            _updateClientUi.UpdateHP(player);
            _updateClientUi.UpdateMana(player);
            _updateClientUi.UpdateMoves(player);
            _updateClientUi.UpdateScore(player);
        }

        public void EmoteAction(Player player, Player target, Room room, SkillMessage emote)
        {
            if (target.ConnectionId == player.ConnectionId)
            {
                _writer.WriteLine(
                    $"<p>{ReplacePlaceholders(emote.Hit.ToTarget, target, true)}</p>",
                    target.ConnectionId);

                return;
            }

            _writer.WriteLine(
                $"<p>{ReplacePlaceholders(emote.Hit.ToPlayer, target, false)}</p>",
                player.ConnectionId);
            _writer.WriteLine(
                $"<p>{emote.Hit.ToTarget}</p>",
                target.ConnectionId);

            foreach (var pc in room.Players)
            {
                if (pc.ConnectionId.Equals(player.ConnectionId) ||
                    pc.ConnectionId.Equals(target.ConnectionId))
                {
                    continue;
                }

                _writer.WriteLine($"<p>{ReplacePlaceholders(emote.Hit.ToRoom, target, false)}</p>",
                    pc.ConnectionId);

            }
        }

        public void EmoteEffectWearOffAction(Player player, Room room, SkillMessage emote)
        {
            
            foreach (var pc in room.Players)
            {
                if (pc.ConnectionId.Equals(player.ConnectionId))
                {
                    _writer.WriteLine($"<p>{emote.EffectWearOff.ToPlayer}</p>",
                        pc.ConnectionId);
                    continue;
                }

                _writer.WriteLine($"<p>{ReplacePlaceholders(emote.EffectWearOff.ToRoom, player, false)}</p>",
                    pc.ConnectionId);

            }
        }


        public int MagicMissile(Player player, Player target, Room room)
        {
            var damage = _dice.Roll(1, 1, 4) + 1;

            DamagePlayer("magic missile", damage, player, target, room );

            return damage;
        }

        public int CauseLightWounds(Player player, Player target, Room room)
        {
            var casterLevel = player.Level > 10 ? 5 : player.Level;
            var damage = _dice.Roll(1, 1, 8) + casterLevel;

            DamagePlayer("Cause light wounds", damage, player, target, room);

            return damage;
        }

        public void Armor(Player player, Player target, Room room, bool wearOff)
        {

            var skillMessage = new SkillMessage()
            {
                NoEffect = new Messages()
                {
                    ToPlayer = "A protective white light engulfs #target#."
                },
                Hit = new Messages()
                {
                    ToPlayer = "#target# is engulfed by a protective white light.",
                    ToTarget = "You feel protected.",
                    ToRoom = "A protective white light engulfs #target#."
                },
                Death = new Messages(),
                Miss = new Messages(),
                EffectWearOff = new Messages()
                {
                    ToPlayer = "Your protective white light fades away.",
                    ToRoom = "#target#'s protective white light fades away."
                }
            };

            var affect = target.Affects.Custom.FirstOrDefault(x =>
                x.Name.Equals("Armour", StringComparison.CurrentCultureIgnoreCase));

            if (wearOff)
            {
                player.ArmorRating.Armour -= 20;


              target.Affects.Custom.Remove(affect);

              EmoteEffectWearOffAction(player, room, skillMessage);

                _updateClientUi.UpdateAffects(player);
                _updateClientUi.UpdateScore(player);
                return;
            }

            var skill = new AllSpells().Armour();
            target = GetValidTarget(player, target, skill.ValidTargets);
          
            //create emote effectWear off message
            EmoteAction(player,target,room, skillMessage);

            if (affect == null)
            {

                target.Affects.Custom.Add(new Affect()
                {
                    Modifier = new Modifier(),
                    Benefits = "Affects armour by 20",
                    Affects = DefineSpell.SpellAffect.ArmorClass,
                    Duration = 2, /* player.Level + player.Attributes.Attribute[EffectLocation.Intelligence] / 2,*/
                    Name = "Armour"
                });
            }
            else
            {
                affect.Duration = player.Level + player.Attributes.Attribute[EffectLocation.Intelligence] / 2;
            }

            target.ArmorRating.Armour += 20;

            _updateClientUi.UpdateAffects(target);
            _updateClientUi.UpdateScore(target);
            _updateClientUi.UpdateScore(player);
     

        }

        public void CureLightWounds(Player player, Player target, Room room)
        {
            var casterLevel = player.Level > 10 ? 5 : player.Level;
            var value = _dice.Roll(1, 1, 4) + 1 + casterLevel / 4;

            var skillMessage = new SkillMessage()
            {
                NoEffect = new Messages()
                {
                    ToPlayer = "#target# is at full health."
                },
                Hit = new Messages()
                {
                    ToPlayer = "#target# looks better!",
                    ToTarget = "You feel better.",
                    ToRoom = "#target# looks better!"
                },
                Death = new Messages(),
                Miss = new Messages()
            };

         var hasAffcted = AffectPlayerAttributes("Cure light wounds", EffectLocation.Hitpoints, value, player, target, room, skillMessage.NoEffect.ToPlayer);

         if (hasAffcted)
         {
             EmoteAction(player, target, room, skillMessage);
             UpdateClientUI(target);
            }
         
            UpdateClientUI(player);
           
        }



        public void CastSpell(string key, string obj, Player target, string fullCommand, Player player, Room room, bool wearOff)
        {
            switch (key.ToLower())
            {
                case "magic missle":
                    MagicMissile(player, target,room);
                    break;
                case "cause light wounds":
                    CauseLightWounds(player, target, room);
                    break;
                case "cure light wounds":
                    CureLightWounds(player, target, room);
                    break;
                case "armour":
                case "armor":
                    Armor(player, target, room, wearOff);
                    break;
            }
        }
    }
}
