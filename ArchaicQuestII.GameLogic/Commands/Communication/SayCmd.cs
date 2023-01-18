using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Communication;

public class SayCmd : ICommand
{
    public SayCmd(ICoreHandler coreHandler)
    {
        Aliases = new[] {"say", "'"};
        Description = "Say something to the room. Everyone present will hear what you say if they're awake.";
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
            Handler.Client.WriteLine("<p>Say what?</p>", player.ConnectionId);
            return;
        }
        
        var text = string.Join(" ", input.Skip(1));
        
        Handler.Client.WriteLine($"<p class='say'>You say {text}</p>", player.ConnectionId);
        Handler.Client.UpdateCommunication(player, $"<p class='say'>You say {text}</p>", "room");
        Handler.Client.WriteToOthersInRoom($"<p class='say'>{player.Name} says {text}</p>", room, player);
        
        foreach (var pc in room.Players.Where(pc => pc.Name != player.Name))
        {
            Handler.Client.UpdateCommunication(pc, $"<p class='say'>{player.Name} says {text}</p>", "room");
        }
    }
}