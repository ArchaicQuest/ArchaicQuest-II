using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Info;

public class ListSpellsCmd : ICommand
{
    public ListSpellsCmd()
    {
        Aliases = new[] { "spells" };
        Description =
            "The skills and spells commands are used to display your character's list "
            + "of available skills (or spells, as the case may be).  They are listed in "
            + "order of level, with mana cost (for spells) or percentage (for skills) "
            + "listed where applicable. Typing skills or spells alone will list only the "
            + "skills/spells you have currently achieved usage of. To list all skills and "
            + "spells you have, use skills/spells all.";
        Usages = new[] { "Type: spells", "spells all" };
        Title = "";
        DeniedStatus = null;
        UserRole = UserRole.Player;
    }

    public string[] Aliases { get; }
    public string Description { get; }
    public string[] Usages { get; }
    public string Title { get; }
    public CharacterStatus.Status[] DeniedStatus { get; }
    public UserRole UserRole { get; }

    public void Execute(Player player, Room room, string[] input)
    {
        var target = input.ElementAtOrDefault(1);

        if (string.IsNullOrEmpty(target))
        {
            var spells = player.Skills.Where(x => x.IsSpell && x.Level <= player.Level).ToList();

            if (spells.Any())
            {
                ReturnSkillList(
                    player.Skills.Where(x => x.IsSpell && x.Level <= player.Level).ToList(),
                    player,
                    "Spells:"
                );
                return;
            }
            else
            {
                Services.Instance.Writer.WriteLine(
                    "<p>You have no spells, try skills instead.</p>",
                    player
                );
                return;
            }
        }

        if (target.Equals("all", StringComparison.CurrentCultureIgnoreCase))
        {
            var spells = player.Skills.Where(x => x.IsSpell).ToList();

            if (spells.Any())
            {
                ReturnSkillList(player.Skills.Where(x => x.IsSpell).ToList(), player, "Spells:");
                return;
            }
            else
            {
                Services.Instance.Writer.WriteLine(
                    "<p>You have no spells, try skills instead.</p>",
                    player
                );
                return;
            }
        }

        ReturnSkillList(player.Skills.ToList(), player, "Spells:");
    }

    // TODO: show mana
    private void ReturnSkillList(List<SkillList> skillList, Player player, string skillTitle)
    {
        Services.Instance.Writer.WriteLine(skillTitle, player);

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
                sb.Append(
                    $"<tr><td>{(currentLevelInteration == 0 ? $"Level   {currentLevel}:" : "&nbsp;")}</td><td>{skill.Name}</td><td>{skill.Proficiency}%</td>"
                );
                i++;
            }
            else
            {
                sb.Append($"<td>&nbsp;</td><td>{skill.Name}</td><td>{skill.Proficiency}%</td>");
                if (i == 2)
                {
                    i = 1;
                }
                sb.Append("</tr>");
            }

            currentLevelInteration++;
        }

        sb.Append("</table>");

        Services.Instance.Writer.WriteLine(sb.ToString(), player);
    }
}
