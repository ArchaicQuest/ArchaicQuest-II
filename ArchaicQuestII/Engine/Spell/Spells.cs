using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.Engine.Character.Model;
using ArchaicQuestII.Engine.Item;
using System.Data;
using System.Reflection;
using ArchaicQuestII.Engine.Character.Status;
using ArchaicQuestII.Engine.Core.Events;
using ArchaicQuestII.Engine.Effect;
using ArchaicQuestII.Engine.Skill;
using ArchaicQuestII.Engine.Skill.Enum;
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
    
            switch (origin.Status)
            {
                case Status.Sleeping:
                    _writer.WriteLine("You can't do this while asleep.");
                    return;
                case Status.Stunned:
                    _writer.WriteLine("You are stunned.");
                    return;
                case Status.Dead:
                case Status.Ghost:
                case Status.Incapitated:
                    _writer.WriteLine("You can't do this while dead.");
                    return;
                case Status.Resting:
                case Status.Sitting:
                    _writer.WriteLine("You need to stand up before you do that.");
                    return;
                case Status.Busy:
                    _writer.WriteLine("You can't do that right now.");
                    return;
            }


            if (origin.Attributes.Attribute[EffectLocation.Mana] < spell.Cost.Table[Cost.Mana])
            {
                _writer.WriteLine("You don't have enough mana.");
                return;
            }

            // target check (shrugs)


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

            _writer.WriteLine(spell.SkillStart.ToPlayer);

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

