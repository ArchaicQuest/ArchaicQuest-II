using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Combat;

public class CastCmd : ICommand
{
    public CastCmd(ICore core)
    {
        Aliases = new[] {"cast"};
        Description = "Cast one of your spells";
        Usages = new[] {"Type: cast fireball"};
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
        //TODO: Build out spells like new commands
    }
}