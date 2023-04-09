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
    public NewbieCmd()
    {
        Aliases = new[] {"newbie"};
        Description = "Sends a message to newbie channel";
        Usages = new[] {"Type: newbie i need help"};
        Title = "";
        DeniedStatus = null;
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
            CoreHandler.Instance.Writer.WriteLine("<p>Newbie what?</p>", player.ConnectionId);
            return;
        }
        
        var text = string.Join(" ", input.Skip(1));
        
        CoreHandler.Instance.Writer.WriteLine($"<p class='newbie'>[<span>Newbie</span>] You: {text}</p>", player.ConnectionId);
        CoreHandler.Instance.Writer.WriteToOthersInGame($"<p class='newbie'>[<span>Newbie</span>] {player.Name}: {text}</p>", player);
        
        foreach (var pc in CoreHandler.Instance.Cache.GetAllPlayers().Where(pc => pc.Config.NewbieChannel))
        {
            CoreHandler.Instance.UpdateClient.UpdateCommunication(pc, $"<p class='newbie'>[<span>Newbie</span>] {(player.Name == pc.Name ? "You" : player.Name)}: {text}</p>", "newbie");
        }

        Helpers.PostToDiscordBot($"{player.Name}: {text}",1091818289249923162,  CoreHandler.Instance.Cache.GetConfig().ChannelDiscordWebHookURL);
     
    }
}