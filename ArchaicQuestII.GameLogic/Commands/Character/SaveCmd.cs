using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Character
{
    public class SaveCmd : ICommand
    {
        public SaveCmd(ICoreHandler coreHandler)
        {
            Aliases = new[] {"save"};
            Description = "Save your character manually, character is saved when you quit and automatically every 15 minutes.";
            Usages = new[] {"Type: save"};
            UserRole = UserRole.Player;
            Title = "";
            DeniedStatus = new []
            {
                CharacterStatus.Status.Sleeping,
                CharacterStatus.Status.Dead,
            };

            Handler = coreHandler;
        }
        
        public string[] Aliases { get; }
        public string Description { get; }
        public string[] Usages { get; }
        public string Title { get; }
        public UserRole UserRole { get; }
        public CharacterStatus.Status[] DeniedStatus { get; }
        
        public ICoreHandler Handler { get; }

        public void Execute(Player player, Room room, string[] input)
        {
            Handler.Pdb.Save(player, PlayerDataBase.Collections.Players);
            Handler.Client.WriteLine("<p>Character saved.</p>", player.ConnectionId);
        }
    }
}