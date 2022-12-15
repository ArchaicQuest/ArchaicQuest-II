using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Movement;

public class RestCmd : ICommand
{
    public RestCmd(ICore core)
    {
        Aliases = new[] {"rest"};
        Description = "Your character will rest.";
        Usages = new[] {"Type: rest"};
        UserRole = UserRole.Player;
        Core = core;
    }
    
    public string[] Aliases { get; }
    public string Description { get; }
    public string[] Usages { get; }
    public UserRole UserRole { get; }
    public ICore Core { get; }

    public void Execute(Player player, Room room, string[] input)
    {
        if (!string.IsNullOrEmpty(player.Mounted.Name))
        {
            Core.Writer.WriteLine("<p>You can't do that while mounted.</p>", player.ConnectionId);
            return;
        }

        if (player.Status == CharacterStatus.Status.Resting)
        {
            Core.Writer.WriteLine("<p>You are already resting!</p>", player.ConnectionId);
            return;
        }

        SetCharacterStatus(player, "is sprawled out here", CharacterStatus.Status.Resting);

        foreach (var pc in room.Players)
        {

            if (pc.Id.Equals(player.Id))
            {
                Core.Writer.WriteLine("<p>You sprawl out haphazardly.</p>", player.ConnectionId);
            }
            else
            {
                Core.Writer.WriteLine($"<p>{player.Name} sprawls out haphazardly.</p>", pc.ConnectionId);
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