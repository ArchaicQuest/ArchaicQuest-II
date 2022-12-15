using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Character
{
    public class TitleCmd : ICommand
    {
        public TitleCmd(ICore core)
        {
            Aliases = new[] {"title"};
            Description = "Changes your characters title.";
            Usages = new[] {"Type: title The Cleaver"};
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
            if (string.IsNullOrEmpty(input.ElementAtOrDefault(1)))
            {
                Core.Writer.WriteLine("Change your title to what?", player.ConnectionId);
                return;
            }
            
            var titleText = string.Join(' ', input.Skip(1));

            player.Title = new string(titleText.Take(55).ToArray());
            Core.Writer.WriteLine($"Title changed to {player.Title}", player.ConnectionId);
        }
    }
}