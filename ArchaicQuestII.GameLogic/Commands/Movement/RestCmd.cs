using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Movement;

public class RestCmd : ICommand
{
    public RestCmd(ICoreHandler coreHandler)
    {
        Aliases = new[] {"rest"};
        Description = "Your character will rest. Resting will increase the speed of health, mana, and moves regeneration." +
                      " Make sure you are somewhere safe because if attacked it will be a guaranteed critical hit.<br /><br />To stop resting enter stand.";
        Usages = new[] {"Type: rest"};
        Title = "";
        DeniedStatus = new[]
        {
            CharacterStatus.Status.Busy,
            CharacterStatus.Status.Dead,
            CharacterStatus.Status.Fighting,
            CharacterStatus.Status.Ghost,
            CharacterStatus.Status.Fleeing,
            CharacterStatus.Status.Incapacitated,
            CharacterStatus.Status.Sleeping,
            CharacterStatus.Status.Stunned,
            CharacterStatus.Status.Resting
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
        if (!string.IsNullOrEmpty(player.Mounted.Name))
        {
            Handler.Client.WriteLine("<p>You can't do that while mounted.</p>", player.ConnectionId);
            return;
        }

        if (player.Status == CharacterStatus.Status.Resting)
        {
            Handler.Client.WriteLine("<p>You are already resting!</p>", player.ConnectionId);
            return;
        }

        Handler.Client.WriteLine("<p>You sprawl out haphazardly.</p>", player.ConnectionId);
        Helpers.SetCharacterStatus(player, "is sprawled out here", CharacterStatus.Status.Resting);
        Handler.Client.WriteToOthersInRoom($"<p>{player.Name} sprawls out haphazardly.</p>", room, player);
    }
}