using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Info;

public class HelpCmd : ICommand
{
    public HelpCmd(ICoreHandler coreHandler)
    {
        Aliases = new[] {"help"};
        Description = @"
<table class='simple heading'>
<tbody>
<tr>
<td>Movement</td>
<td>Objects</td>
</tr><tr><td>north south east west up down
northeast northwest southeast
southwest recall
sleep wake rest stand enter</td>
<td>get put drop give sacrifice          
wear wield hold                     
recite quaff zap brandish            
lock unlock open close pick          
inventory equipment look compare    
eat drink fill                      
list buy sell value</td>
</tr>
<tr>
<td>Combat</td>
<td>Group</td>
</tr>
<tr>
<td>kill cast skills spells
dodge parry wimpy flee  
wands scrolls staves
damage death healers
nosummon PK</td>
<td> group follow nofollow gtell</td>
</tr>  <tr>
<td>Character</td>
<td>Communication</td>
</tr>
<tr>
<td>description title 
score report practice train stats
commands socials pose emote RP </td>
<td>ic ooc newbie gossip yell shout   
note idea history change  
say tell reply who    
</td>
</tr>
</tbody>
                </table>";
        Usages = new[] {$"help {WebUtility.HtmlEncode("<keyword>")} <br /> for example, help quaff"};
        Title = "";
        DeniedStatus = null;
        UserRole = UserRole.Player;

        Handler = coreHandler;
    }
        
    public string[] Aliases { get; }
    public string Description { get; }
    public string[] Usages { get; }
    public string Title { get; }
    public CharacterStatus.Status[] DeniedStatus { get; }
    public UserRole UserRole { get; }
    public ICoreHandler Handler { get; }

    public void Execute(Player player, Room room, string[] input)
    {
        var target = input.ElementAtOrDefault(1);

        if (string.IsNullOrEmpty(target))
        {
            target = "help";
        }
            
        // TODO: move to startup to build and cache 
        // Proof of concept, have help file for anything that's not a command
        // example help newbie could explain about the game
        var nonCommandHelpFiles = new Dictionary<string,HelpFileContent>
        {
            {
                "test", new HelpFileContent
                {
                    Title = "test",
                    Description = "it works",
                    Aliases = new[] { "" },
                    Usages = new[] { "" }
                }
            }
        };

        var command = Handler.Command.GetCommand(target) ?? Handler.Command.GetCommands().Values
            .FirstOrDefault(x => x.Title.StartsWith(target, StringComparison.CurrentCultureIgnoreCase));

        HelpFileContent help = null;
        if (string.IsNullOrEmpty(command?.Title))
        {
            nonCommandHelpFiles.TryGetValue(target, out help);
        }

        if (command == null && help == null)
        {
            Handler.Client.WriteLine($"<p>No help found for {target}.", player.ConnectionId);
            return;
        }

        var helpText = new HelpFileContent
        {
            Aliases = command?.Aliases ?? help?.Aliases,
            Description = command?.Description ?? help?.Description,
            Title = command?.Title ?? help?.Title,
            Usages = command?.Usages ?? help?.Usages
        };

        var helpString = HelpHtml(helpText, target);
        Handler.Client.WriteLine(helpString, player.ConnectionId);
    }

    
    private static string HelpHtml(HelpFileContent command, string target)
    {
        var sb = new StringBuilder();

        sb.Append("<div class='help-section'><table>");
        sb.Append($"<tr><td>Help Title</td><td>{(string.IsNullOrEmpty(command.Title) ? target : command.Title)}</td></tr>");
                
        sb.Append("<tr><td>Aliases:</td><td>");
                
        var index = command.Aliases.Length;
                
        foreach (var alias in command.Aliases)
        {
            sb.Append($"{alias}");
            index--;
            if (index > 0)
                sb.Append(", ");
        }

        sb.Append("</td></tr><tr><td>Usages:</td>");

        foreach (var usage in command.Usages)
        {
            sb.Append($"<td>{WebUtility.HtmlEncode(usage)}</td>");
        }

        sb.Append("</tr></table>");

        sb.Append($"<pre>{command.Description}</pre>");

        return sb.ToString();
    }
}