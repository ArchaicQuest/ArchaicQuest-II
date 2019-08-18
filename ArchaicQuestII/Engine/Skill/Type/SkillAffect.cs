using ArchaicQuestII.Engine.Core.Events;
using ArchaicQuestII.Engine.Skill.Model;

namespace ArchaicQuestII.Engine.Skill.Type
{
    /// <summary>
    /// Handles effects on player attributes which is defined in Attributes.cs
    /// Effects can be negative or positive, and can stack.
    /// </summary>
    public class SkillAffect
    {
        private readonly IWriteToClient _writer;
        private static SkillTarget _skillTarget;
        private readonly int _value;

        public SkillAffect(IWriteToClient writer, SkillTarget skillTarget, int value)
        {
            _writer = writer;
            _skillTarget = skillTarget;
  
            _value = value;
        }
        public void CauseAffect()
        {
            var action = new SkillMessage(_writer);
              action.DisplayActionToUser(_skillTarget.Skill.LevelBasedMessages, _skillTarget.Skill.SkillAction, _skillTarget.Origin.Level);

            if (_skillTarget.Skill.Effect.Modifier.PositiveEffect)
            {
                _skillTarget.Target.Attributes.Attribute[_skillTarget.Skill.Effect.Location] += _value;
            }
            else
            {
                _skillTarget.Target.Attributes.Attribute[_skillTarget.Skill.Effect.Location] -= _value;
            }
        }
    }
}
