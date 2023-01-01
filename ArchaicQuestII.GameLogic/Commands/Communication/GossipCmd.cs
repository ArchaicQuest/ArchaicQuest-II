using System;
using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Communication;

public class GossipCmd : ICommand
{
    public GossipCmd(ICore core)
    {
        Aliases = new[] {"gossip", "goss"};
        Description = "Talk on the IC gossip channel.";
        Usages = new[] {"Type: gossip some message"};
        DeniedStatus = null;
        Title = String.Empty;
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
        if (string.IsNullOrEmpty(input.ElementAtOrDefault(1)))
        {
            Core.Writer.WriteLine("Gossip what?", player.ConnectionId);
            return;
        }
        
        var text = string.Join(" ", input.Skip(1));
        
        Core.Writer.WriteLine($"<p class='gossip'>[<span>Gossip</span>]: {text}</p>", player.ConnectionId);
        Core.UpdateClient.UpdateCommunication(player, $"<p class='gossip'>[<span>Gossip</span>]: {text}</p>", "gossip");
        Core.Writer.WriteToOthersInRoom($"<p class='gossip'>[<span>Gossip</span>] {player.Name}: {text}</p>", room, player);
        
        foreach (var pc in room.Players.Where(pc => !pc.Name.Equals(player.Name, StringComparison.CurrentCultureIgnoreCase) && pc.Config.GossipChannel))
        {
            Core.UpdateClient.UpdateCommunication(pc, $"<p class='gossip'>[<span>Gossip</span>] {player.Name}: {text}</p>", "gossip");
        }

        Helpers.PostToDiscord($"<p>[Gossip] {player.Name} {text}</p>", "channels", Core.Cache.GetConfig());
    }
}