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
    public GossipCmd(ICoreHandler coreHandler)
    {
        Aliases = new[] {"gossip", "goss"};
        Description = "Talk on the IC gossip channel.";
        Usages = new[] {"Type: gossip some message"};
        DeniedStatus = null;
        Title = String.Empty;
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
            Handler.Client.WriteLine("Gossip what?", player.ConnectionId);
            return;
        }
        
        var text = string.Join(" ", input.Skip(1));
        
        Handler.Client.WriteLine($"<p class='gossip'>[<span>Gossip</span>]: {text}</p>", player.ConnectionId);
        Handler.Client.UpdateCommunication(player, $"<p class='gossip'>[<span>Gossip</span>]: {text}</p>", "gossip");
        Handler.Client.WriteToOthersInRoom($"<p class='gossip'>[<span>Gossip</span>] {player.Name}: {text}</p>", room, player);
        
        foreach (var pc in room.Players.Where(pc => !pc.Name.Equals(player.Name, StringComparison.CurrentCultureIgnoreCase) && pc.Config.GossipChannel))
        {
            Handler.Client.UpdateCommunication(pc, $"<p class='gossip'>[<span>Gossip</span>] {player.Name}: {text}</p>", "gossip");
        }

        if(Handler.Config.PostToDiscord)
            Helpers.PostToDiscord($"<p>[Gossip] {player.Name} {text}</p>", Handler.Config.ChannelDiscordWebHookURL);
    }
}