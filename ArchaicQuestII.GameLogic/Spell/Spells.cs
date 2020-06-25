
using System;
using System.Linq;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Skill.Enum;
using ArchaicQuestII.GameLogic.Skill.Model;
using ArchaicQuestII.GameLogic.Spell.Interface;
using ArchaicQuestII.GameLogic.World.Room;
using LiteDB;

namespace ArchaicQuestII.GameLogic.Spell
{
    public class Spells : ISpells
    {
        private readonly IWriteToClient _writer;

        public Spells(IWriteToClient writer)
        {
            _writer = writer;

        }

        public bool ValidStatus(Player player)
        {
            switch (player.Status)
            {
                case CharacterStatus.Status.Sleeping:
                    _writer.WriteLine("You can't do this while asleep.");
                    return false;
                case CharacterStatus.Status.Stunned:
                    _writer.WriteLine("You are stunned.");
                    return false;
                case CharacterStatus.Status.Dead:
                case CharacterStatus.Status.Ghost:
                case CharacterStatus.Status.Incapacitated:
                    _writer.WriteLine("You can't do this while dead.");
                    return false;
                case CharacterStatus.Status.Resting:
                case CharacterStatus.Status.Sitting:
                    _writer.WriteLine("You need to stand up before you do that.");
                    return false;
                case CharacterStatus.Status.Busy:
                    _writer.WriteLine("You can't do that right now.");
                    return false;
                default:
                    return true;
            }
        }

        public Model.Spell FindSpell(string spell, Player player)
        {
             var foundSpell = player.Spells.FirstOrDefault(x => x.Name.StartsWith(spell, StringComparison.CurrentCultureIgnoreCase));

             if (foundSpell == null)
             {
                 _writer.WriteLine($"You don't know a spell that begins with {spell}");
             }

             return foundSpell;
        }

        public bool ManaCheck(Model.Spell spell, Player player)
        {
            if (player.Attributes.Attribute[EffectLocation.Mana] < spell.Cost.Table[Cost.Mana])
            {
                _writer.WriteLine("You don't have enough mana.");
                return false;
            }

            return true;
        }

        public bool SpellAffectsCharacter(Model.Spell spell)
        {
            
            return (spell.ValidTargets & ValidTargets.TargetPlayerWorld) != 0 || 
                   (spell.ValidTargets & ValidTargets.TargetFightVictim) != 0 || 
                   (spell.ValidTargets & ValidTargets.TargetSelfOnly)    != 0 || 
                   (spell.ValidTargets & ValidTargets.TargetPlayerRoom)  != 0 || 
                   (spell.ValidTargets & ValidTargets.TargetFightSelf)   != 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spell"></param>
        /// <param name="origin"></param>
        /// <param name="target"></param>
        /// <param name="room"></param>
        public void DoSpell(string spellName, Player origin, string targetName, Room room = null)
        {

            if (!ValidStatus(origin))
            {
                return;
            }

            var spell = FindSpell(spellName, origin);

            if (spell == null)
            {
                return;
            }

            if (!ManaCheck(spell, origin))
            {
                return;
            }

            if(spe)

            // target check (shrugs)
            Player target = null;

          

            if (origin.Status == CharacterStatus.Status.Fighting && spell.StartsCombat && string.IsNullOrEmpty(targetName))
            {
                // target equals the target of the player
            }

            if((spell.ValidTargets & ValidTargets.TargetObjectRoom) != 0)



            // not correct need to send to room 
            if (origin == target)
            {
                _writer.WriteLine($"{origin.Name} closes {Helpers.GetPronoun(origin.Gender)} eyes and utters the words, '{spell.Name}'.");
            }
            else if(origin != target)
            {
                _writer.WriteLine($"{origin.Name} stares at {target.Name} and utters the words, '{spell.Name}'.");
            }
            else
            {
                _writer.WriteLine($"{origin.Name} utters the words, '{spell.Name}'.");
            }


            var formula = spell.Damage.Roll(spell.Damage.DiceRoll, spell.Damage.DiceMinSize,
                              spell.Damage.DiceMaxSize) + (origin.Level + 1) / 2; //+ mod

            //Fire skill start message to player, room, target

            //deduct mana
            origin.Attributes.Attribute[EffectLocation.Mana] -= spell.Cost.Table[Cost.Mana];

            if (spell.Rounds > 1)
            {
                // prob needs to be on player
                spell.Rounds -= 1;
                return;
            }

            // hit / miss messages

          //  _writer.WriteLine(spell.SkillStart.ToPlayer);

            var skillTarget = new SkillTarget
            {
                Origin = origin,
                Target = target,
                Room = room,
                Skill = spell
            };

            new SpellEffect(_writer, skillTarget, formula).Type[skillTarget.Skill.Type].Invoke();

        }

    }

}

