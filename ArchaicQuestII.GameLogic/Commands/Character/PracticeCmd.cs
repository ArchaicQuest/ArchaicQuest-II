using System;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Character
{
    public class PracticeCmd : ICommand
    {
        public PracticeCmd(ICore core)
        {
            Aliases = new[] {"practice", "prac"};
            Description = "Practice only works at a guild trainer to practice your skills or spells. Your learning " +
            "percentage varies from 1% to a maximum of 75%. <br />The higher your intelligence, the more you will learn at each practice " +
            "session.  <br />The higher your wisdom, the more practice sessions you will " +
            "have each time you gain a level. <br />To view your skills just enter skills";
            Usages = new[] {"Type: practice, practice <skill>"};
            Title = "";
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
        public string Title { get; }
        public CharacterStatus.Status[] DeniedStatus { get; }
        public UserRole UserRole { get; }
        public ICore Core { get; }

        public void Execute(Player player, Room room, string[] input)
        {
            var target = input.ElementAtOrDefault(1);

            if (string.IsNullOrEmpty(target))
            {
                Core.Writer.WriteLine("<p>Practice what?</p>", player.ConnectionId);
                return;
            }

            if (room.Mobs.Find(x => x.Trainer) == null)
            {
                Core.Writer.WriteLine("<p>You can't do that here.</p>", player.ConnectionId);
                return;
            }

            var trainerName = room.Mobs.Find(x => x.Trainer).Name;

            var skillName = target == "prac" || target == "practice" ? "" : target;

            if (string.IsNullOrEmpty(skillName))
            {
                Core.Writer.WriteLine($"<p>You have {player.Practices} practice{(player.Practices <= 1 ? "" : "s")} left.</p>", player.ConnectionId);

                var sb = new StringBuilder();

                sb.Append("<table class='simple'><thead><tr><th></th><th></th><th colspan='2'>Skills</th><th></th><th></th></tr></thead><tbody>");

                var i = 0;
                foreach (var skill in player.Skills.OrderBy(x => x.Name))
                {
                    if (i == 0)
                    {
                        sb.Append("<tr>");
                    }

                    if (i <= 2)
                    {
                        sb.Append($"<td>{skill.Name}</td><td>{skill.Proficiency}%</td>");
                    }

                    if (i == 2)
                    {
                        sb.Append("</tr>");
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

            var foundSkill = player.GetSkill(skillName);

            if (foundSkill == null)
            {
                Core.Writer.WriteLineMobSay(trainerName, "<p>You don't have that skill to practice.</p>", player.ConnectionId);
                return;
            }

            if (player.Practices == 0)
            {
                Core.Writer.WriteLineMobSay(trainerName, "<p>You have no practices left.</p>", player.ConnectionId);
                return;
            }

            if (foundSkill.Proficiency == 100)
            {
                Core.Writer.WriteLineMobSay(trainerName, $"<p>You have already mastered {foundSkill.Name}.</p>", player.ConnectionId);
                return;
            }

            if (foundSkill.Proficiency >= 75)
            {
                Core.Writer.WriteLineMobSay(trainerName, $"<p>I've taught you everything I can about {foundSkill.Name}.</p>", player.ConnectionId);
                return;
            }

            var maxGain = player.Attributes.Attribute[EffectLocation.Intelligence];
            var minGain = player.Attributes.Attribute[EffectLocation.Intelligence] / 2;
            var gain = DiceBag.Roll(1, minGain, maxGain);

            foundSkill.Proficiency += gain;
            player.Practices -= 1;

            if (foundSkill.Proficiency >= 75)
            {
                foundSkill.Proficiency = 75;
                Core.Writer.WriteLine($"<p>You practice for some time. Your proficiency with {foundSkill.Name} is now {foundSkill.Proficiency}%.</p>", player.ConnectionId);
                Core.Writer.WriteLineMobSay(trainerName, "<p>You'll have to practice it on your own now...</p>", player.ConnectionId);
                return;
            }

            Core.Writer.WriteLine($"<p>You practice for some time. Your proficiency with {foundSkill.Name} is now {foundSkill.Proficiency}%.</p>", player.ConnectionId);
        }
    }
}