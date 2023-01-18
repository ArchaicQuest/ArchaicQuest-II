using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Movement;

public class SleepCmd : ICommand
{
    public SleepCmd(ICoreHandler coreHandler)
    {
        Aliases = new[] {"sleep"};
        Description = "Your character will go to sleep and will not see anything that happens the room. Sleeping will increase the speed of health, mana, and moves regeneration." +
                      " Make sure you are somewhere safe because if attacked it will be a guaranteed critical hit.<br /><br />To wake up enter stand or wake.";
        Usages = new[] {"Type: sleep"};
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
            CharacterStatus.Status.Resting,
            CharacterStatus.Status.Sitting,
            CharacterStatus.Status.Mounted
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
        Helpers.SetCharacterStatus(player, "is sleeping nearby", CharacterStatus.Status.Sleeping);
        Handler.Client.WriteLine("<p>You collapse into a deep sleep.</p>", player.ConnectionId);
        Handler.Client.WriteToOthersInRoom($"<p>{player.Name} collapses into a deep sleep.</p>", room, player);
    }
}