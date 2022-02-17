using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Class;
using ArchaicQuestII.GameLogic.Character.Gain;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Item;

namespace ArchaicQuestII.GameLogic.Commands.Skills
{
    public class Skills : ISkills
    {

        private readonly IWriteToClient _writeToClient;
        private readonly IUpdateClientUI _clientUi;
        private readonly ICache _cache;
        private readonly IGain _gain;

        public Skills(IWriteToClient writeToClient, ICache cache, IUpdateClientUI clientUi, IGain gain)
        {
            _writeToClient = writeToClient;
            _cache = cache;
            _clientUi = clientUi;
            _gain = gain;
        }
        public void ShowSkills(Player player, string fullCommand)
        {
              
            if(fullCommand.Equals("skills all", StringComparison.CurrentCultureIgnoreCase))
            {
                ReturnSkillList(player.Skills.Where(x => x.IsSpell == false).ToList(), player, "Skills:");
                return;
            }

            if (fullCommand.Equals("skills", StringComparison.CurrentCultureIgnoreCase) || fullCommand.Equals("skill", StringComparison.CurrentCultureIgnoreCase))
            {
                ReturnSkillList(player.Skills.Where(x => x.IsSpell == false && x.Level <= player.Level).ToList(), player, "Skills:");
                return;
            }

            if (fullCommand.Equals("spells all", StringComparison.CurrentCultureIgnoreCase))
            {
                var spells = player.Skills.Where(x => x.IsSpell == true).ToList();

                if (spells.Any())
                {
                    ReturnSkillList(player.Skills.Where(x => x.IsSpell == true).ToList(), player, "Spells:");
                    return;
                }
                else
                {
                    _writeToClient.WriteLine("You have no spells, try skills instead.", player.ConnectionId);
                    return;
                }
            }

            if (fullCommand.Equals("spells", StringComparison.CurrentCultureIgnoreCase))
            {
                var spells = player.Skills.Where(x => x.IsSpell == true && x.Level <= player.Level).ToList();
                if (spells.Any())
                {
                    ReturnSkillList(player.Skills.Where(x => x.IsSpell == true && x.Level <= player.Level).ToList(), player, "Spells:");
                    return;
                }
                else
                {
                    _writeToClient.WriteLine("You have no spells, try skills instead.", player.ConnectionId);
                    return;
                }
            }

        
                ReturnSkillList(player.Skills.ToList(), player, "Skills &amp; Spells:");
            


        }

        private void ReturnSkillList(List<SkillList> skillList, Player player, string skillTitle)
        {

            _writeToClient.WriteLine(skillTitle, player.ConnectionId);

            var sb = new StringBuilder();
            sb.Append("<table>");
            var currentLevel = 1;
            var currentLevelInteration = 0;
            var i = 1;

            foreach (var skill in skillList.OrderBy(x => x.Level))
            {
                if (skill.Level != currentLevel)
                {
                    currentLevel = skill.Level;
                    currentLevelInteration = 0;
                    i = 1;
                }

                if (i == 1)
                {
                    sb.Append($"<tr><td>{ (currentLevelInteration == 0 ? $"Level   {currentLevel}:" : "&nbsp;")}</td><td>{skill.SkillName}</td><td>{skill.Proficiency}%</td>");
                    i++;
                }
                else
                {
                    sb.Append($"<td>&nbsp;</td><td>{skill.SkillName}</td><td>{skill.Proficiency}%</td>");
                    if (i == 2)
                    {
                        i = 1;
                    }
                    sb.Append("</tr>");
                }

                currentLevelInteration++;
            }

            sb.Append("</table>");

            _writeToClient.WriteLine(sb.ToString(), player.ConnectionId);
        }

        public bool SuccessCheck(Player player, string skillName)
        {
            var skill = player.Skills.FirstOrDefault(x =>
                x.SkillName.Equals(skillName, StringComparison.CurrentCultureIgnoreCase));

            var chance = new Dice().Roll(1, 1, 100);

            if (skill != null && skill.Proficiency <= chance || chance == 1)
            {
                return false;
            }

            return true;
        }


        public void LearnMistakes(Player player, string skillName, int delay = 0)
        {
            var skill = player.Skills.FirstOrDefault(x => x.SkillName.Equals(skillName, StringComparison.CurrentCultureIgnoreCase));

            if (skill == null)
            {
                return;
            }

            if (skill.Proficiency == 100)
            {
                return;
            }

            var increase = new Dice().Roll(1, 1, 5);

            skill.Proficiency += increase;

            _gain.GainExperiencePoints(player, 100 * skill.Level / 4, false);

            _clientUi.UpdateExp(player);

            _writeToClient.WriteLine(
                $"<p class='improve'>You learn from your mistakes and gain {100 * skill.Level / 4} experience points.</p>" +
                $"<p class='improve'>Your knowledge of {skill.SkillName} increases by {increase}%.</p>",
                player.ConnectionId, delay);
        }
    }
}
