using System.Threading.Tasks;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Character
{
    public class RecallCmd : ICommand
    {
        public RecallCmd(ICore core)
        {
            Aliases = new[] {"recall"};
            Description = "Transports your character to recall room.";
            Usages = new[] {"Type: recall"};
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
            if ((player.Status & CharacterStatus.Status.Sleeping) != 0)
            {
                Core.Writer.WriteLine("In your dreams, or what?", player.ConnectionId);
                return;
            }
            if ((player.Status & CharacterStatus.Status.Sitting) != 0)
            {
                Core.Writer.WriteLine("Better stand up first.", player.ConnectionId);
                return;
            }
            if ((player.Status & CharacterStatus.Status.Resting) != 0)
            {
                Core.Writer.WriteLine("Nah... You feel too relaxed...", player.ConnectionId);
                return;
            }

            var recallRoom = Core.Cache.GetRoom(player.RecallId);
            
            player.Buffer.Clear();
            
            Recall(player, room, recallRoom);
        }

        private async void Recall(Player player, Room currentRoom, Room recallRoom)
        {
            Core.Writer.WriteLine("<p>You begin to channel your energy to perform recall.</p>", player.ConnectionId);

            await Task.Delay(2000);
            
            Core.Writer.WriteLine("<p>You feel the air crackle and the ground shift.</p>", player.ConnectionId);
            
            await Task.Delay(2000);
            
            Core.RoomActions.RoomChange(player, currentRoom, recallRoom);
        }
    }
}