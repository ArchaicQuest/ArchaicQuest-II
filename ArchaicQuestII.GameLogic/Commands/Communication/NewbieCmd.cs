using System;
using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
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
        
        Core.Writer.WriteLine($"<p class='newbie'>[<span>Newbie</span>]: {text}</p>", player.ConnectionId);
        Core.UpdateClient.UpdateCommunication(player, $"<p class='newbie'>[<span>Newbie</span>]: {text}</p>", "newbie");
        Core.Writer.WriteToOthersInRoom($"<p class='newbie'>[<span>Newbie</span>] {player.Name}: {text}</p>", room, player);
        
        foreach (var pc in room.Players.Where(pc => !pc.Name.Equals(player.Name, StringComparison.CurrentCultureIgnoreCase) && pc.Config.NewbieChannel))
        {
            Core.UpdateClient.UpdateCommunication(pc, $"<p class='newbie'>[<span>Newbie</span>] {player.Name}: {text}</p>", "newbie");
        }

        Helpers.PostToDiscord($"<p>[Newbie] {player.Name} {text}</p>", "channels", Core.Cache.GetConfig());
    }
}