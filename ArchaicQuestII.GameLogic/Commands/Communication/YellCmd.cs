using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Communication;

public class YellCmd : ICommand
{
    public YellCmd(ICore core)
    {
        Aliases = new[] {"yell"};
        Description = "Sends a message to everyone in the area";
        Usages = new[] {"Type: yell 'message'"};
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
            Core.Writer.WriteLine("<p>Yell what?</p>", player.ConnectionId);
            return;
        }
        
        var text = string.Join(" ", input.Skip(1));
        
        var rooms = Core.Cache.GetAllRoomsInArea(room.AreaId);
        
        Core.Writer.WriteLine($"<p class='yell'>You yell, {text.ToUpper()}</p>", player.ConnectionId);

        foreach (var pc in from rm in rooms from pc in rm.Players where pc.Name != player.Name select pc)
        {
            Core.Writer.WriteLine($"<p class='yell'>{player.Name} yells, {text.ToUpper()}</p>", pc.ConnectionId);
            Core.UpdateClient.UpdateCommunication(pc, $"<p class='yell'>{player.Name} yells, {text.ToUpper()}</p>", "room");
        }

        Core.UpdateClient.UpdateCommunication(player, $"<p class='yell'>You yell, {text.ToUpper()}</p>", "room");
    }
}