using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
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
            DeniedStatus = default;
            UserRole = UserRole.Player;
            Title = "";
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
            if (string.IsNullOrEmpty(input.ElementAtOrDefault(1)))
            {
                Core.Writer.WriteLine("<p>Change your title to what?</p>", player.ConnectionId);
                return;
            }
            
            var titleText = string.Join(' ', input.Skip(1));

            player.Title = new string(titleText.Take(55).ToArray());
            Core.Writer.WriteLine($"<p>Title changed to {player.Title}.</p>", player.ConnectionId);
        }
    }
}