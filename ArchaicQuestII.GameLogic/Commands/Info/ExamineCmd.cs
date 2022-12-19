using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Info
{
    public class ExamineCmd : ICommand
    {
        public ExamineCmd(ICore core)
        {
            Aliases = new[] {"examine", "exam", "ex"};
            Description = "Shows detailed info about room or object.";
            Usages = new[] {"Type: examine dagger"};
            DeniedStatus = new []
            {
                CharacterStatus.Status.Sleeping,
                CharacterStatus.Status.Dead,
            };
            UserRole = UserRole.Player;
            Core = core;
        }
        
        public string[] Aliases { get; }
        public string Description { get; }
        public string[] Usages { get; }
        public UserRole UserRole { get; }
        public ICore Core { get; }
        public CharacterStatus.Status[] DeniedStatus { get; }


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

            Core.RoomActions.ExamineObject(target, room, player);
            
        }
    }
}