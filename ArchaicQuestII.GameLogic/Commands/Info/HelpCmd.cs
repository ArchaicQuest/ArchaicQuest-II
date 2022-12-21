using System;
using System.Linq;
using System.Net;
using System.Text;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Info
{
    public class HelpCmd : ICommand
    {
        public HelpCmd(ICore core)
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
southwest exits recall
sleep wake rest stand</td>
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
                target = "help";
            }

            var command = Core.Cache.GetCommand(target) ?? Core.Cache.GetCommands().Values
                .FirstOrDefault(x => x.Title.StartsWith(target, StringComparison.CurrentCultureIgnoreCase));

            if (command == null)
            {
                Core.Writer.WriteLine($"<p>No help found for {target}.", player.ConnectionId);
                return;
            }

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
                sb.Append($"<td>{usage}</td>");
            }

            sb.Append("</tr></table>");

            sb.Append($"<pre>{command.Description}</pre>");

            Core.Writer.WriteLine(sb.ToString(), player.ConnectionId);
        }
    }
}