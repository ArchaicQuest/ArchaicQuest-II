using System;
using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Communication;

public class OOCCmd : ICommand
{
    public OOCCmd(ICore core)
    {
        Aliases = new[] {"ooc"};
        Description = "Sends a message to out of character channel";
        Usages = new[] {"Type: ooc Did anyone see the game last night?"};
        DeniedStatus = null;
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
        if (string.IsNullOrEmpty(input.ElementAtOrDefault(1)))
        {
            Core.Writer.WriteLine("<p>ooc what?</p>", player.ConnectionId);
            return;
        }
        
        var text = string.Join(" ", input.Skip(1));
        
        Core.Writer.WriteLine($"<p class='ooc'>[<span>OOC</span>]: {text}</p>", player.ConnectionId);
        Core.UpdateClient.UpdateCommunication(player, $"<p class='ooc'>[<span>OOC</span>]: {text}</p>", "ooc");
        
        foreach (var pc in room.Players.Where(pc => !pc.Name.Equals(player.Name, StringComparison.CurrentCultureIgnoreCase) && pc.Config.OocChannel))
        {
            Core.Writer.WriteLine($"<p class='ooc'>[<span>OOC</span>] {player.Name}: {text}</p>", pc.ConnectionId);
            Core.UpdateClient.UpdateCommunication(pc, $"<p class='ooc'>[<span>OOC</span>] {player.Name}: {text}</p>", "ooc");
        }

        Helpers.PostToDiscord($"<p>[OOC] {player.Name} {text}</p>", "channels", Core.Cache.GetConfig());
    }
}