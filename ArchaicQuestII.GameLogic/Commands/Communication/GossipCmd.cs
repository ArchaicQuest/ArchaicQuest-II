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
    public GossipCmd()
    {
        Aliases = new[] {"gossip", "goss"};
        Description = "Talk on the IC gossip channel.";
        Usages = new[] {"Type: gossip some message"};
        DeniedStatus = null;
        Title = String.Empty;
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
        if (string.IsNullOrEmpty(input.ElementAtOrDefault(1)))
        {
            CoreHandler.Instance.Writer.WriteLine("Gossip what?", player.ConnectionId);
            return;
        }
        
        var text = string.Join(" ", input.Skip(1));
        
        
        CoreHandler.Instance.Writer.WriteLine($"<p class='gossip'>[<span>Gossip</span>] You: {text}</p>", player.ConnectionId);
        CoreHandler.Instance.Writer.WriteToOthersInGame($"<p class='gossip'>[<span>Gossip</span>] {player.Name}: {text}</p>", player);
        
        foreach (var pc in CoreHandler.Instance.Cache.GetAllPlayers().Where(pc => pc.Config.OocChannel))
        {
            CoreHandler.Instance.UpdateClient.UpdateCommunication(pc, $"<p class='gossip'>[<span>Gossip</span>] {(player.Name == pc.Name ? "You" : player.Name)}: {text}</p>", "ooc");
        }

        Helpers.PostToDiscordBot($"{player.Name}: {text}",1092857545183473694,  CoreHandler.Instance.Cache.GetConfig().ChannelDiscordWebHookURL);

    }
}