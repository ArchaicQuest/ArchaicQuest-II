using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Movement;

public class SleepCmd : ICommand
{
    public SleepCmd(IWriteToClient writeToClient, ICache cache, IUpdateClientUI updateClient, IRoomActions roomActions)
    {
        Aliases = new[] {"sleep"};
        Description = "Your character will go to sleep.";
        Usages = new[] {"Type: sleep"};
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

        if (player.Status == CharacterStatus.Status.Sleeping)
        {
            Writer.WriteLine("<p>You are already sleeping!</p>", player.ConnectionId);
            return;
        }

        SetCharacterStatus(player, "is sleeping nearby", CharacterStatus.Status.Sleeping);

        foreach (var pc in room.Players)
        {

            if (pc.Id.Equals(player.Id))
            {
                Writer.WriteLine("<p>You collapse into a deep sleep.</p>", player.ConnectionId);
            }
            else
            {
                Writer.WriteLine($"<p>{player.Name} collapses into a deep sleep.</p>", pc.ConnectionId);
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