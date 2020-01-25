using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Skill.Model;
using System.Collections.Generic;

namespace ArchaicQuestII.GameLogic.Skill
{
    public class SkillMessage
    {
        private readonly IWriteToClient _writer;

        public SkillMessage(IWriteToClient writer)
        {
            _writer = writer;
        }

        public void DisplayActionToUser(LevelBasedMessages levelBasedActions, List<Messages> Actions, int level)
        {
       
            if (levelBasedActions.HasLevelBasedMessages)
            {
                switch (level)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                        //send level spec message
                        _writer.WriteLine(levelBasedActions.Ten.ToPlayer);
                        _writer.WriteLine(levelBasedActions.Ten.ToTarget);
                        _writer.WriteLine(levelBasedActions.Ten.ToRoom);
                        break;
                    case 11:
                    case 12:
                    case 13:
                    case 14:
                    case 15:
                    case 16:
                    case 17:
                    case 18:
                    case 19:
                    case 20:
                        //send level spec message
                        _writer.WriteLine(levelBasedActions.Twenty.ToPlayer);
                        _writer.WriteLine(levelBasedActions.Twenty.ToTarget);
                        _writer.WriteLine(levelBasedActions.Twenty.ToRoom);

                        break;
                    case 21:
                    case 22:
                    case 23:
                    case 24:
                    case 25:
                    case 26:
                    case 27:
                    case 28:
                    case 29:
                    case 30:
                        //send level spec message
                        _writer.WriteLine(levelBasedActions.Thirty.ToPlayer);
                        _writer.WriteLine(levelBasedActions.Thirty.ToTarget);
                        _writer.WriteLine(levelBasedActions.Thirty.ToRoom);
                        break;
                    case 31:
                    case 32:
                    case 33:
                    case 34:
                    case 35:
                    case 36:
                    case 37:
                    case 38:
                    case 39:
                    case 40:
                        //send level spec message
                        _writer.WriteLine(levelBasedActions.Forty.ToPlayer);
                        _writer.WriteLine(levelBasedActions.Forty.ToTarget);
                        _writer.WriteLine(levelBasedActions.Forty.ToRoom);
                        break;
                    case 41:
                    case 42:
                    case 43:
                    case 44:
                    case 45:
                    case 46:
                    case 47:
                    case 48:
                    case 49:
                    case 50:
                    case 51:
                        //send level spec message
                        _writer.WriteLine(levelBasedActions.Fifty.ToPlayer);
                        _writer.WriteLine(levelBasedActions.Fifty.ToTarget);
                        _writer.WriteLine(levelBasedActions.Fifty.ToRoom);
                        break;
                    default:

                        foreach (var message in Actions)
                        {
                            _writer.WriteLine(message.ToPlayer);
                            _writer.WriteLine(message.ToTarget);
                            _writer.WriteLine(message.ToRoom);
                        }

                        break;
                }
            }
            else
            {
                foreach (var message in Actions)
                {
                    _writer.WriteLine(message.ToPlayer);
                    _writer.WriteLine(message.ToTarget);
                    _writer.WriteLine(message.ToRoom);
                }

            }
        }
    }
}
