using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.Engine.Character.Model;
using ArchaicQuestII.Engine.Core.Events;
using ArchaicQuestII.Engine.Skills;

namespace ArchaicQuestII.Engine.Spell.Type
{
    public class SpellAffect
    {
        private readonly IWriteToClient _writer;
        private readonly Model.Spell _spell;
        private readonly Player _origin;
        private readonly Player _target;
        private readonly int _value;

        public SpellAffect(IWriteToClient writer, Model.Spell spell, Player origin, Player target, int value)
        {
            _writer = writer;
            _spell = spell;
            _origin = origin;
            _target = target;
            _value = value;
        }
        public void CauseAffect()
        {
            var action = new SkillMessage(_writer);
            action.DisplayActionToUser(_spell.LevelBasedMessages, _spell.SkillAction, _origin.Level);

            if (_spell.Effect.Modifier.PositiveEffect)
            {
                _target.Attributes.Attribute[_spell.Effect.Location] += _value;
            }
            else
            {
                _target.Attributes.Attribute[_spell.Effect.Location] -= _value;
            }
        }
    }
}
