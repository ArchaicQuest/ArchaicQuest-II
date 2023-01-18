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
        public TitleCmd(ICoreHandler coreHandler)
        {
            Aliases = new[] {"title"};
            Description = "Changes your characters title, this is what people see by your name when they enter who.";
            Usages = new[] {"Type: title The Cleaver"};
            DeniedStatus = default;
            UserRole = UserRole.Player;
            Title = "";

            Handler = coreHandler;
        }
        
        public string[] Aliases { get; }
        public string Description { get; }
        public string[] Usages { get; }
        public string Title { get; }
        public CharacterStatus.Status[] DeniedStatus { get; }
        public UserRole UserRole { get; }
        
        public ICoreHandler Handler { get; }

        public void Execute(Player player, Room room, string[] input)
        {
            if (string.IsNullOrEmpty(input.ElementAtOrDefault(1)))
            {
                Handler.Client.WriteLine("<p>Change your title to what?</p>", player.ConnectionId);
                return;
            }
            
            var titleText = string.Join(' ', input.Skip(1));

            player.Title = new string(titleText.Take(55).ToArray());
            Handler.Client.WriteLine($"<p>Title changed to {player.Title}.</p>", player.ConnectionId);
        }
    }
}