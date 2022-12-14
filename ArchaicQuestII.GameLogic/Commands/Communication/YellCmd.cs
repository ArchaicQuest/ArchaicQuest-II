using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Communication;

public class YellCmd : ICommand
{
    public YellCmd(IWriteToClient writeToClient, ICache cache, IUpdateClientUI updateClient, IRoomActions roomActions)
    {
        Aliases = new[] {"yell"};
        Description = "Sends a message to everyone in the area";
        Usages = new[] {"Type: yell 'message'"};
        UserRole = UserRole.Player;
        Writer = writeToClient;
        Cache = cache;
        UpdateClient = updateClient;
        RoomActions = roomActions;
    }
    
    public string[] Aliases { get; }
    public string Description { get; }
    public string[] Usages { get; }
    public UserRole UserRole { get; }
    public IWriteToClient Writer { get; }
    public ICache Cache { get; }
    public IUpdateClientUI UpdateClient { get; }
    public IRoomActions RoomActions { get; }

    public void Execute(Player player, Room room, string[] input)
    {
        if (string.IsNullOrEmpty(input.ElementAtOrDefault(1)))
        {
            Writer.WriteLine("Yell what?", player.ConnectionId);
            return;
        }
        
        var text = string.Join(" ", input.Skip(1));
        
        var rooms = Cache.GetAllRoomsInArea(room.AreaId);
        
        Writer.WriteLine($"<p class='yell'>You yell, {text.ToUpper()}</p>", player.ConnectionId);

        foreach (var pc in from rm in rooms from pc in rm.Players where pc.Name != player.Name select pc)
        {
            Writer.WriteLine($"<p class='yell'>{player.Name} yells, {text.ToUpper()}</p>", pc.ConnectionId);
            UpdateClient.UpdateCommunication(pc, $"<p class='yell'>{player.Name} yells, {text.ToUpper()}</p>", "room");
        }

        UpdateClient.UpdateCommunication(player, $"<p class='yell'>You yell, {text.ToUpper()}</p>", "room");
    }
}