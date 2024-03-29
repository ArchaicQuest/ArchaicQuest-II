using System.Threading.Tasks;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Character
{
    public class RecallCmd : ICommand
    {
        public RecallCmd()
        {
            Aliases = new[] { "recall" };
            Description = "Transports your character to their recall room.";
            Usages = new[] { "Type: recall" };
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
        }

        public string[] Aliases { get; }
        public string Description { get; }
        public string[] Usages { get; }
        public string Title { get; }
        public CharacterStatus.Status[] DeniedStatus { get; }
        public UserRole UserRole { get; }

        private readonly string[] _castBegin =
        {
            "You begin to channel your energy to perform recall.",
            "You focus your mind on returning to somewhere safe."
        };

        private readonly string[] _castEnd =
        {
            "You feel the air crackle and the ground shift.",
            "You feel your body begin to tense."
        };

        public void Execute(Player player, Room room, string[] input)
        {
            var recallRoom = Services.Instance.Cache.GetRoom(player.RecallId);

            player.Buffer.Clear();

            Recall(player, room, recallRoom);
        }

        private async void Recall(Player player, Room currentRoom, Room recallRoom)
        {
            var cb = DiceBag.Roll(1, 0, _castBegin.Length - 1);
            var ce = DiceBag.Roll(1, 0, _castEnd.Length - 1);

            Services.Instance.Writer.WriteLine($"<p>{_castBegin[cb]}</p>", player);

            await Task.Delay(2000);

            Services.Instance.Writer.WriteLine($"<p>{_castEnd[ce]}</p>", player);

            await Task.Delay(2000);

            player.ChangeRoom(currentRoom, recallRoom);
        }
    }
}
