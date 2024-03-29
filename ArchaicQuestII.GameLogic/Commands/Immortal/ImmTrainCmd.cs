using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Immortal;

public class ImmTrainCmd : ICommand
{
    public ImmTrainCmd()
    {
        Aliases = new[] { "/train" };
        Description = "Max out a players stats";
        Usages = new[] { "Example: /train bob", "Example: /train" };
        Title = "";
        DeniedStatus = null;
        UserRole = UserRole.Player;
    }

    public string[] Aliases { get; }
    public string Description { get; }
    public string[] Usages { get; }
    public string Title { get; }
    public CharacterStatus.Status[] DeniedStatus { get; }
    public UserRole UserRole { get; }

    public void Execute(Player player, Room room, string[] input)
    {
        var target = input.ElementAtOrDefault(1);

        if (string.IsNullOrEmpty(target))
        {
            foreach (var skill in player.Skills)
            {
                skill.Proficiency = 85;
            }

            Services.Instance.Writer.WriteLine("<p>You have max stats now.</p>", player);
            return;
        }

        Player foundPlayer = null;

        foreach (var checkRoom in Services.Instance.Cache.GetAllRooms())
        {
            foreach (
                var checkRoomPlayer in checkRoom.Players.Where(
                    checkRoomPlayer => checkRoomPlayer.Name == target
                )
            )
            {
                foundPlayer = checkRoomPlayer;
                break;
            }
        }

        if (foundPlayer == null)
        {
            Services.Instance.Writer.WriteLine("<p>They're not here.</p>", player);
            return;
        }

        foreach (var skill in foundPlayer.Skills)
        {
            skill.Proficiency = 85;
        }

        Services.Instance.Writer.WriteLine($"<p>{foundPlayer} has max stats now.</p>", player);
    }
}
