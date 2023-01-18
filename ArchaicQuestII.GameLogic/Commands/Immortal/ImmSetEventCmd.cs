using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Immortal;

public class ImmSetEventCmd : ICommand
{
    public ImmSetEventCmd(ICoreHandler coreHandler)
    {
        Aliases = new[] {"/setevent"};
        Description = "Set an event, for mob testing";
        Usages = new[]
        {
            "Example: /train bob",
        };
            Title = "";
    DeniedStatus = null;
        UserRole = UserRole.Staff;

        Handler = coreHandler;
    }
    
    public string[] Aliases { get; }
    public string Description { get; }
    public string[] Usages { get; }
  
    public string Title { get; }  public CharacterStatus.Status[] DeniedStatus { get; }
    public UserRole UserRole { get; }
    public ICoreHandler Handler { get; }

    public void Execute(Player player, Room room, string[] input)
    {
        var eventName = input.ElementAtOrDefault(1);
        var value = input.ElementAtOrDefault(2);

        if (string.IsNullOrEmpty(eventName) || !int.TryParse(value, out var num))
        {
            foreach (var ev in player.EventState)
            {
                Handler.Client.WriteLine($"{ev.Key} - {ev.Value}", player.ConnectionId);
            }

            return;
        }

        if (player.EventState.ContainsKey(eventName))
        {
            player.EventState[eventName] = num;
            Handler.Client.WriteLine($"{eventName} state changed to {player.EventState[eventName]}", player.ConnectionId);
            return;
        }

        Handler.Client.WriteLine("Invalid Event state", player.ConnectionId);
    }
}