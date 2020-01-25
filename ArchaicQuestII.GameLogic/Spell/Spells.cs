
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Skill.Enum;
using ArchaicQuestII.GameLogic.Skill.Model;
using ArchaicQuestII.GameLogic.Spell.Interface;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Spell
{
    public class Spells : ISpells
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
                case CharacterStatus.Status.Sleeping:
                    _writer.WriteLine("You can't do this while asleep.");
                    return;
                case CharacterStatus.Status.Stunned:
                    _writer.WriteLine("You are stunned.");
                    return;
                case CharacterStatus.Status.Dead:
                case CharacterStatus.Status.Ghost:
                case CharacterStatus.Status.Incapitated:
                    _writer.WriteLine("You can't do this while dead.");
                    return;
                case CharacterStatus.Status.Resting:
                case CharacterStatus.Status.Sitting:
                    _writer.WriteLine("You need to stand up before you do that.");
                    return;
                case CharacterStatus.Status.Busy:
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

