using System;
using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Communication;

public class OOCCmd : ICommand
{
    public OOCCmd(ICoreHandler coreHandler)
    {
        Aliases = new[] {"ooc"};
        Description = "Sends a message to out of character channel";
        Usages = new[] {"Type: ooc Did anyone see the game last night?"};
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
        if (string.IsNullOrEmpty(input.ElementAtOrDefault(1)))
        {
            Handler.Client.WriteLine("<p>ooc what?</p>", player.ConnectionId);
            return;
        }
        
        var text = string.Join(" ", input.Skip(1));
        
        Handler.Client.WriteLine($"<p class='ooc'>[<span>OOC</span>]: {text}</p>", player.ConnectionId);
        Handler.Client.UpdateCommunication(player, $"<p class='ooc'>[<span>OOC</span>]: {text}</p>", "ooc");
        Handler.Client.WriteToOthersInRoom($"<p class='ooc'>[<span>OOC</span>] {player.Name}: {text}</p>", room, player);
        
        foreach (var pc in room.Players.Where(pc => !pc.Name.Equals(player.Name, StringComparison.CurrentCultureIgnoreCase) && pc.Config.OocChannel))
        {
            Handler.Client.UpdateCommunication(pc, $"<p class='ooc'>[<span>OOC</span>] {player.Name}: {text}</p>", "ooc");
        }

        if(Handler.Config.PostToDiscord)
            Helpers.PostToDiscord($"<p>[OOC] {player.Name} {text}</p>", Handler.Config.ChannelDiscordWebHookURL);
    }
}