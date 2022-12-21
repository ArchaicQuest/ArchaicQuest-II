using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Immortal;

public class ImmTrainCmd : ICommand
{
    public ImmTrainCmd(ICore core)
    {
        Aliases = new[] {"/train"};
        Description = "Max out a players stats";
        Usages = new[]
        {
            "Example: /train bob",
            "Example: /train"
        };
        DeniedStatus = null;
        UserRole = UserRole.Staff;
        Core = core;
    }
    
    public string[] Aliases { get; }
    public string Description { get; }
    public string[] Usages { get; }
    public CharacterStatus.Status[] DeniedStatus { get; }
    public UserRole UserRole { get; }
    public ICore Core { get; }

    public void Execute(Player player, Room room, string[] input)
    {
        var target = input.ElementAtOrDefault(1);

        if (string.IsNullOrEmpty(target))
        {
            foreach (var skill in player.Skills)
            {
                skill.Proficiency = 85;
            }
            
            Core.Writer.WriteLine("<p>You have max stats now.</p>", player.ConnectionId);
            return;
        }
        
        Player foundPlayer = null;
        
        foreach (var checkRoom in Core.Cache.GetAllRooms())
        {
            foreach (var checkRoomPlayer in checkRoom.Players.Where(checkRoomPlayer => checkRoomPlayer.Name == target))
            {
                foundPlayer = checkRoomPlayer;
                break;
            }
        }

        if (foundPlayer == null)
        {
            Core.Writer.WriteLine("<p>They're not here.</p>", player.ConnectionId);
            return;
        }
        
        foreach (var skill in foundPlayer.Skills)
        {
            skill.Proficiency = 85;
        }
        
        Core.Writer.WriteLine($"<p>{foundPlayer} has max stats now.</p>", player.ConnectionId);
    }
}