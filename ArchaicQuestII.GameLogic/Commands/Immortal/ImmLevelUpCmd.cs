using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Immortal;

public class ImmLevelUpCmd : ICommand
{
    public ImmLevelUpCmd(ICore core)
    {
        Aliases = new[] {"/level"};
        Description = "Increase a characters level";
        Usages = new[]
        {
            "Example: /level bob",
            "Example: /level"
        };
            Title = "";
    DeniedStatus = null;
        UserRole = UserRole.Staff;
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
        var target = input.ElementAtOrDefault(1);

        if (string.IsNullOrEmpty(target))
        {
            player.GainLevel(out _);
            return;
        }

        var otherPlayer = Core.Cache.GetAllPlayers().FirstOrDefault(x => x.Name == target);

        if(otherPlayer == null)
        {
            Core.Writer.WriteLine($"No player '{target}' found.", player.ConnectionId);
            return;
        }
        
        otherPlayer.GainLevel(out _);
    }
}