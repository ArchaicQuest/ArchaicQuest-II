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
        
        
        Core.Writer.WriteLine($"<p class='gossip'>[<span>Gossip</span>] You: {text}</p>", player.ConnectionId);
      //  Core.Writer.WriteToOthersInGame($"<p class='gossip'>[<span>Gossip</span>] {player.Name}: {text}</p>", player);
        
        foreach (var pc in Core.Cache.GetAllPlayers().Where(pc => pc.Config.OocChannel))
        {
            Core.UpdateClient.UpdateCommunication(pc, $"<p class='gossip'>[<span>Gossip</span>] {(player.Name == pc.Name ? "You" : player.Name)}: {text}</p>", "ooc");
        }

        Helpers.PostToDiscordBot($"{player.Name}: {text}",1092857545183473694,  Core.Cache.GetConfig().ChannelDiscordWebHookURL);

    }
}