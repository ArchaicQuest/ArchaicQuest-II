using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.Engine.Character.Model;
using ArchaicQuestII.Engine.Core.Events;
using ArchaicQuestII.Engine.Skill.Enum;
using ArchaicQuestII.Engine.Skill.Model;
using ArchaicQuestII.Engine.Skill.Type;

namespace ArchaicQuestII.Engine.Spell
{
    public class SpellEffect
    {
        private static IWriteToClient _writer;
        private static SkillTarget _skillTarget;
        private static int _value;

        public SpellEffect(IWriteToClient writer, SkillTarget skillTarget, int value)
        {
            _writer = writer;
            _skillTarget = skillTarget;
            _value = value;
        }

        public Dictionary<SkillType, Action> Type { get; set; } = new Dictionary<SkillType, Action>
        {
            {SkillType.Affect, () => new SkillAffect(_writer, _skillTarget, _value).CauseAffect()}
        };

    }
}
