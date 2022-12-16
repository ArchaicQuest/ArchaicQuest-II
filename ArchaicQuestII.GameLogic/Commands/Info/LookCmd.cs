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
            Aliases = new[] {"l", "look"};
            Description = "Shows info about room or object.";
            Usages = new[] {"Type: look"};
            DeniedStatus = default;
            UserRole = UserRole.Player;
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

            if (!string.IsNullOrEmpty(target) && target.Equals("in") && input.Length == 3)
            {
                target = input.ElementAtOrDefault(2);
               Core.RoomActions.LookInContainer(target, room, player);
                return;
            }
            
            Core.RoomActions.Look(target, room, player);
            
        }
    }
}