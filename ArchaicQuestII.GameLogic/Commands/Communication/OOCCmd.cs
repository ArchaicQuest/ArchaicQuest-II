using System;
using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Communication;

public class OOCCmd : ICommand
{
    public OOCCmd(IWriteToClient writeToClient, ICache cache, IUpdateClientUI updateClient, IRoomActions roomActions)
    {
        Aliases = new[] {"ooc"};
        Description = "Sends a message to out of character channel";
        Usages = new[] {"Type: ooc Did anyone see the game last night?"};
        UserRole = UserRole.Player;
        Writer = writeToClient;
        Cache = cache;
        UpdateClient = updateClient;
        RoomActions = roomActions;
    }
    
    public string[] Aliases { get; }
    public string Description { get; }
    public string[] Usages { get; }
    public UserRole UserRole { get; }
    public IWriteToClient Writer { get; }
    public ICache Cache { get; }
    public IUpdateClientUI UpdateClient { get; }
    public IRoomActions RoomActions { get; }

    public void Execute(Player player, Room room, string[] input)
    {
        if (string.IsNullOrEmpty(input.ElementAtOrDefault(1)))
        {
            Writer.WriteLine("ooc what?", player.ConnectionId);
            return;
        }
        
        var text = string.Join(" ", input.Skip(1));
        
        Writer.WriteLine($"<p class='ooc'>[<span>OOC</span>]: {text}</p>", player.ConnectionId);
        UpdateClient.UpdateCommunication(player, $"<p class='ooc'>[<span>OOC</span>]: {text}</p>", "ooc");
        
        foreach (var pc in room.Players.Where(pc => !pc.Name.Equals(player.Name, StringComparison.CurrentCultureIgnoreCase) && pc.Config.OocChannel))
        {
            Writer.WriteLine($"<p class='ooc'>[<span>OOC</span>] {player.Name}: {text}</p>", pc.ConnectionId);
            UpdateClient.UpdateCommunication(pc, $"<p class='ooc'>[<span>OOC</span>] {player.Name}: {text}</p>", "ooc");
        }

        Helpers.PostToDiscord($"[OOC] {player.Name} {text}", "channels", Cache.GetConfig());
    }
}