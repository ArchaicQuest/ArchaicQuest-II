using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Info
{
    public class LookCmd : ICommand
    {
        public LookCmd(ICore core)
        {
            Aliases = new[] {"look"};
            Description = "Shows info about room or object.";
            Usages = new[] {"Type: look"};
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
            if (player.Status == CharacterStatus.Status.Sleeping)
            {
                Core.Writer.WriteLine("You can't do that while asleep.", player.ConnectionId);
                return;
            }

            if (player.Affects.Blind)
            {
                Core.Writer.WriteLine("<p>You are blind and can't see a thing!</p>", player.ConnectionId);
                return;
            }
            
            var target = input.ElementAtOrDefault(1);
            Core.RoomActions.Look(target, room, player);
        }
    }
}