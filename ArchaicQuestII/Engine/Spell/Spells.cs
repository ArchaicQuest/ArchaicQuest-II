using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.Engine.Character.Model;
using ArchaicQuestII.Engine.Item;
using System.Data;
using System.Reflection;
using ArchaicQuestII.Engine.Core.Events;
using ArchaicQuestII.Engine.Effect;
using ArchaicQuestII.Engine.Skill;
using ArchaicQuestII.Engine.Skill.Model;
using ArchaicQuestII.Engine.World.Room.Model;
using ArchaicQuestII.Engine.Spell.Interface;

namespace ArchaicQuestII.Engine.Spell
{
    public class Spells: ISpells
    {
        private readonly IWriteToClient _writer;
       
        public Spells(IWriteToClient writer)
        {
            _writer = writer;
           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spell"></param>
        /// <param name="origin"></param>
        /// <param name="target"></param>
        /// <param name="room"></param>
        public void DoSpell(Model.Spell spell, Player origin, Player target, Room room = null)
        {
            // you're asleep
            // your deead
            // all those checks to go here
            // mana check
            // target check (shrugs)


            var formula = spell.Damage.Roll(spell.Damage.DiceRoll, spell.Damage.DiceMinSize,
                              spell.Damage.DiceMaxSize) + (origin.Level + 1) / 2; //+ mod

            //Fire skill start message to player, room, target

            //deduct mana


            if (spell.Rounds > 1)
            {
                // prob needs to be on player
                spell.Rounds -= 1;
                return;
            }

            _writer.WriteLine(spell.SkillStart.ToPlayer);

            var skillTarget = new SkillTarget()
            {
                Origin = origin,
                Target = target,
                Skill = spell,
                Room = null
            };

            new SpellEffect(_writer, skillTarget, formula).Type[spell.Type].Invoke();
 
        }

    }
 
}

