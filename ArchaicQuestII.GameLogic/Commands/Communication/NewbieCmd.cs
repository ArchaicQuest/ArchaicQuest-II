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
    public NewbieCmd(ICoreHandler coreHandler)
    {
        Aliases = new[] {"newbie"};
        Description = "Sends a message to newbie channel";
        Usages = new[] {"Type: newbie i need help"};
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
            Handler.Client.WriteLine("<p>Newbie what?</p>", player.ConnectionId);
            return;
        }
        
        var text = string.Join(" ", input.Skip(1));
        
        Handler.Client.WriteLine($"<p class='newbie'>[<span>Newbie</span>]: {text}</p>", player.ConnectionId);
        Handler.Client.UpdateCommunication(player, $"<p class='newbie'>[<span>Newbie</span>]: {text}</p>", "newbie");
        Handler.Client.WriteToOthersInRoom($"<p class='newbie'>[<span>Newbie</span>] {player.Name}: {text}</p>", room, player);
        
        foreach (var pc in room.Players.Where(pc => !pc.Name.Equals(player.Name, StringComparison.CurrentCultureIgnoreCase) && pc.Config.NewbieChannel))
        {
            Handler.Client.UpdateCommunication(pc, $"<p class='newbie'>[<span>Newbie</span>] {player.Name}: {text}</p>", "newbie");
        }

        if(Handler.Config.PostToDiscord)
            Helpers.PostToDiscord($"<p>[Newbie] {player.Name} {text}</p>", Handler.Config.ChannelDiscordWebHookURL);
    }
}