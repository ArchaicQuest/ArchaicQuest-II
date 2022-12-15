using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Class;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Info;

public class ListSkillsCmd : ICommand
{
    public ListSkillsCmd(ICore core)
    {
        Aliases = new[] {"skills"};
        Description = "Shows available sk";
        Usages = new[] {"Type: north"};
        UserRole = UserRole.Player;
        Core = core;
    }
    
    public string[] Aliases { get; }
    public string Description { get; }
    public string[] Usages { get; }
    public UserRole UserRole { get; }
    public ICore Core { get; }

    public void Execute(Player player, Room room, string[] input)
    {
        var target = input.ElementAtOrDefault(1);
        
        if (string.IsNullOrEmpty(target))
        {
            var skills = player.Skills.Where(x => x.IsSpell == false && x.Level <= player.Level).ToList();
            
            if (skills.Any())
            {
                ReturnSkillList(player.Skills.Where(x => x.IsSpell == false && x.Level <= player.Level).ToList(), player, "Skills:");
                return;
            }
            else
            {
                Core.Writer.WriteLine("You have no skills, try spells instead.", player.ConnectionId);
                return;
            }
        }

        if (target.Equals("all", StringComparison.CurrentCultureIgnoreCase))
        {
            var spells = player.Skills.Where(x => x.IsSpell).ToList();

            if (spells.Any())
            {
                ReturnSkillList(player.Skills.Where(x => x.IsSpell == false).ToList(), player, "Skills:");
                return;
            }
            else
            {
                Core.Writer.WriteLine("You have no skills, try spells instead.", player.ConnectionId);
                return;
            }
        }

        ReturnSkillList(player.Skills.ToList(), player, "Skills:");
    }
    
    private void ReturnSkillList(List<SkillList> skillList, Player player, string skillTitle)
    {

        Core.Writer.WriteLine(skillTitle, player.ConnectionId);

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

        Core.Writer.WriteLine(sb.ToString(), player.ConnectionId);
    }
}