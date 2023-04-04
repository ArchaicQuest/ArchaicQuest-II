using System;
using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Communication;

public class NewbieCmd : ICommand
{
    public NewbieCmd(ICore core)
    {
        Aliases = new[] {"newbie"};
        Description = "Sends a message to newbie channel";
        Usages = new[] {"Type: newbie i need help"};
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
        if (string.IsNullOrEmpty(input.ElementAtOrDefault(1)))
        {
            Core.Writer.WriteLine("<p>Newbie what?</p>", player.ConnectionId);
            return;
        }
        
        var text = string.Join(" ", input.Skip(1));
        
        Core.Writer.WriteLine($"<p class='newbie'>[<span>Newbie</span>] You: {text}</p>", player.ConnectionId);
      //  Core.Writer.WriteToOthersInGame($"<p class='newbie'>[<span>Newbie</span>] {player.Name}: {text}</p>", player);
        
        foreach (var pc in Core.Cache.GetAllPlayers().Where(pc => pc.Config.NewbieChannel))
        {
            Core.UpdateClient.UpdateCommunication(pc, $"<p class='newbie'>[<span>Newbie</span>] {(player.Name == pc.Name ? "You" : player.Name)}: {text}</p>", "newbie");
        }

        Helpers.PostToDiscordBot($"{player.Name}: {text}",1091818289249923162,  Core.Cache.GetConfig().ChannelDiscordWebHookURL);
     
    }
}