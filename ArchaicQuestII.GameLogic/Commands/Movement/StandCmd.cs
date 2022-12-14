using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Movement;

public class StandCmd : ICommand
{
    public StandCmd(IWriteToClient writeToClient, ICache cache, IUpdateClientUI updateClient, IRoomActions roomActions)
    {
        Aliases = new[] {"stand"};
        Description = "You character stands up.";
        Usages = new[] {"Type: stand"};
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
        if (!string.IsNullOrEmpty(player.Mounted.Name))
        {
            Writer.WriteLine("<p>You can't do that while mounted.</p>", player.ConnectionId);
            return;
        }

        if (player.Status == CharacterStatus.Status.Standing)
        {
            Writer.WriteLine("<p>You are already standing!</p>", player.ConnectionId);
            return;
        }

        var standMessage = "rises up.";
        if (player.Status == CharacterStatus.Status.Resting)
        {
            standMessage = $"arises from {(player.Gender == "Male" ? "his" : "her")} rest.";
        }
        else if (player.Status == CharacterStatus.Status.Sleeping)
        {
            standMessage = $"arises from {(player.Gender == "Male" ? "his" : "her")} slumber.";
        }

        SetCharacterStatus(player, "", CharacterStatus.Status.Standing);
        
        foreach (var pc in room.Players)
        {
            if (pc.Id.Equals(player.Id))
            {
                Writer.WriteLine("<p>You move quickly to your feet.</p>", player.ConnectionId);
            }
            else
            {
                Writer.WriteLine($"<p>{player.Name} {standMessage}</p>", pc.ConnectionId);
            }
        }
    }

    private void SetCharacterStatus(Player player, string longName, CharacterStatus.Status status)
    {
        player.Status = status;
        player.LongName = longName;
        player.Pose = "";
    }
}