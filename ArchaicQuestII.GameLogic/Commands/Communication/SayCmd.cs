using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Communication;

public class SayCmd : ICommand
{
    public SayCmd(ICore core)
    {
        Aliases = new[] {"say", "'"};
        Description = "Say something to the room.";
        Usages = new[] {"Type: say what ever you want"};
        Title = "";
        DeniedStatus = new[]
        {
            CharacterStatus.Status.Busy,
            CharacterStatus.Status.Dead,
            CharacterStatus.Status.Fleeing,
            CharacterStatus.Status.Incapacitated,
            CharacterStatus.Status.Sleeping,
            CharacterStatus.Status.Stunned
        };
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
            Core.Writer.WriteLine("<p>Say what?</p>", player.ConnectionId);
            return;
        }
        
        var text = string.Join(" ", input.Skip(1));
        
        Core.Writer.WriteLine($"<p class='say'>You say {text}</p>", player.ConnectionId);
        Core.UpdateClient.UpdateCommunication(player, $"<p class='say'>You say {text}</p>", "room");
        Core.Writer.WriteToOthersInRoom($"<p class='say'>{player.Name} says {text}</p>", room, player);
        
        foreach (var pc in room.Players.Where(pc => pc.Name != player.Name))
        {
            Core.UpdateClient.UpdateCommunication(pc, $"<p class='say'>{player.Name} says {text}</p>", "room");
        }
    }
}