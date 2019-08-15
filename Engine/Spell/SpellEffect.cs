using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.Engine.Character.Model;
using ArchaicQuestII.Engine.Core.Events;
using ArchaicQuestII.Engine.Spell.Type;

namespace ArchaicQuestII.Engine.Spell
{
    public class SpellEffect
    {
        private static IWriteToClient _writer;
        private static Model.Spell _spell;
        private static Player _origin;
        private static Player _target;
        private static int _value;

        public SpellEffect(IWriteToClient writer, Model.Spell spell, Player origin, Player target, int value)
        {
            _writer = writer;
            _spell = spell;
            _origin = origin;
            _target = target;
            _value = value;
        }

        public Dictionary<SpellType, Action> Type { get; set; } = new Dictionary<SpellType, Action>
        {
            {SpellType.Affect, () => new SpellAffect(_writer, _spell, _origin, _target, _value).CauseAffect()}
        };

    }
}
