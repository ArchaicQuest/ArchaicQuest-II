using System;
using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Communication;

public class NewbieCmd : ICommand
{
    public NewbieCmd(IWriteToClient writeToClient, ICache cache, IUpdateClientUI updateClient, IRoomActions roomActions)
    {
        Aliases = new[] {"newbie"};
        Description = "Sends a message to newbie channel";
        Usages = new[] {"Type: newbie i need help"};
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
            Writer.WriteLine("Newbie what?", player.ConnectionId);
            return;
        }
        
        var text = string.Join(" ", input.Skip(1));
        
        Writer.WriteLine($"<p class='newbie'>[<span>Newbie</span>]: {text}</p>", player.ConnectionId);
        UpdateClient.UpdateCommunication(player, $"<p class='newbie'>[<span>Newbie</span>]: {text}</p>", "newbie");
        
        foreach (var pc in room.Players.Where(pc => !pc.Name.Equals(player.Name, StringComparison.CurrentCultureIgnoreCase) && pc.Config.NewbieChannel))
        {
            Writer.WriteLine($"<p class='newbie'>[<span>Newbie</span>] {player.Name}: {text}</p>", pc.ConnectionId);
            UpdateClient.UpdateCommunication(pc, $"<p class='newbie'>[<span>Newbie</span>] {player.Name}: {text}</p>", "newbie");
        }

        Helpers.PostToDiscord($"[Newbie] {player.Name} {text}", "channels", Cache.GetConfig());
    }
}