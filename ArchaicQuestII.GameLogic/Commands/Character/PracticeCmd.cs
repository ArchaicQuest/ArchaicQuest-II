using System;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Character
{
    public class PracticeCmd : ICommand
    {
        public PracticeCmd(ICore core)
        {
            Aliases = new[] {"practice", "prac"};
            Description = "Practice skills at a trainer.";
            Usages = new[] {"Type: "};
            DeniedStatus = new[]
            {
                CharacterStatus.Status.Busy,
                CharacterStatus.Status.Dead,
                CharacterStatus.Status.Fighting,
                CharacterStatus.Status.Ghost,
                CharacterStatus.Status.Fleeing,
                CharacterStatus.Status.Incapacitated,
                CharacterStatus.Status.Sleeping,
                CharacterStatus.Status.Stunned
            };
            UserRole = UserRole.Player;
            Core = core;
        }
        
        public string[] Aliases { get; }
        public string Description { get; }
        public string[] Usages { get; }
        public CharacterStatus.Status[] DeniedStatus { get; }
        public UserRole UserRole { get; }
        public ICore Core { get; }

        public void Execute(Player player, Room room, string[] input)
        {
            var target = input.ElementAtOrDefault(1);

            if (string.IsNullOrEmpty(target))
            {
                Core.Writer.WriteLine("Practice what?", player.ConnectionId);
                return;
            }

            if (room.Mobs.Find(x => x.Trainer) == null)
            {
                Core.Writer.WriteLine("You can't do that here.", player.ConnectionId);
                return;
            }

            var trainerName = room.Mobs.Find(x => x.Trainer).Name;

            var skillName = target == "prac" || target == "practice" ? "" : target;

            if (string.IsNullOrEmpty(skillName))
            {
                Core.Writer.WriteLine($"You have {player.Practices} practice{(player.Practices <= 1 ? "" : "s")} left.", player.ConnectionId);

                var sb = new StringBuilder();

                sb.Append("<table class='simple'><thead><tr><th></th><th></th><th colspan='2'>Skills</th><th></th><th></th></tr></thead><tbody>");

                var i = 0;
                foreach (var skill in player.Skills.OrderBy(x => x.SkillName))
                {
                    if (i == 0)
                    {
                        sb.Append("<tr>");
                    }

                    if (i <= 2)
                    {
                        sb.Append($"<td>{skill.SkillName}</td><td>{skill.Proficiency}%</td>");
                    }

                    if (i == 2)
                    {
                        sb.Append($"</tr>");
                        i = 0;
                        continue;
                    }
                    i++;

                };

                sb.Append("</tbody></table>");

                //if (player.Skills.Where(x => x.IsSpell == true).Any())
                //{

                //    sb.Append("<table class='simple'><thead><tr><th></th><th></th><th colspan='2'>Spells</th><th></th><th></th></tr></thead><tbody>");

                //    var j = 0;
                //    foreach (var skill in player.Skills.Where(x => x.IsSpell == true).OrderBy(x => x.SkillName))
                //    {
                //        if (j == 0)
                //        {
                //            sb.Append("<tr>");
                //        }

                //        if (j <= 2)
                //        {
                //            sb.Append($"<td>{skill.SkillName}</td><td>{skill.Proficiency}%</td>");
                //        }

                //        if (j == 2)
                //        {
                //            sb.Append($"</tr>");
                //            i = 0;
                //            continue;
                //        }


                //        j++;

                //        if (player.Skills.Where(x => x.IsSpell == true).Count() == 2 && j == player.Skills.Where(x => x.IsSpell == true).Count())
                //        {
                //            if (j == 2)
                //            {
                //                sb.Append($"<td>&nbsp;</td>&nbsp;<td></td>");
                //                sb.Append($"<td>&nbsp;</td><td>&nbsp;</td>");
                //            }
                //        }
                //    };

                //    sb.Append("</tbody></table>");

                //}
                Core.Writer.WriteLine(sb.ToString(), player.ConnectionId);
                return;
            }

            var foundSkill = player.Skills.Find(x => x.SkillName.StartsWith(skillName, StringComparison.OrdinalIgnoreCase));

            if (foundSkill == null)
            {
                Core.Writer.WriteLineMobSay(trainerName, $"You don't have that skill to practice.", player.ConnectionId);
                return;
            }

            if (player.Practices == 0)
            {
                Core.Writer.WriteLineMobSay(trainerName, $"You have no practices left.", player.ConnectionId);
                return;
            }

            if (foundSkill.Proficiency == 100)
            {
                Core.Writer.WriteLineMobSay(trainerName, $"You have already mastered {foundSkill.SkillName}.", player.ConnectionId);
                return;
            }

            if (foundSkill.Proficiency >= 75)
            {
                Core.Writer.WriteLineMobSay(trainerName, $"I've taught you everything I can about {foundSkill.SkillName}.", player.ConnectionId);
                return;
            }

            var maxGain = player.Attributes.Attribute[EffectLocation.Intelligence];
            var minGain = player.Attributes.Attribute[EffectLocation.Intelligence] / 2;
            var gain = Core.Dice.Roll(1, minGain, maxGain);

            foundSkill.Proficiency += gain;
            player.Practices -= 1;

            if (foundSkill.Proficiency >= 75)
            {
                foundSkill.Proficiency = 75;
                Core.Writer.WriteLine($"You practice for some time. Your proficiency with {foundSkill.SkillName} is now {foundSkill.Proficiency}%", player.ConnectionId);
                Core.Writer.WriteLineMobSay(trainerName, $"You'll have to practice it on your own now...", player.ConnectionId);
                return;
            }

            Core.Writer.WriteLine($"You practice for some time. Your proficiency with {foundSkill.SkillName} is now {foundSkill.Proficiency}%", player.ConnectionId);
        }
    }
}