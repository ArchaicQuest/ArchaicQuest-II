using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Skill.Enum;
using ArchaicQuestII.GameLogic.Skill.Model;
using ArchaicQuestII.GameLogic.Skill.Type;
using System;
using System.Collections.Generic;

namespace ArchaicQuestII.GameLogic.Spell
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
