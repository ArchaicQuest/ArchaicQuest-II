using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Character
{
    public class DrinkCmd : ICommand
    {
        public DrinkCmd(ICoreHandler coreHandler)
        {
            Aliases = new[] {"drink"};
            Description = "Drink a liquid from a container. Drinking is for RP purposes, your character does not get hungry or thirsty.";
            Usages = new[] {"Type: drink ale"};
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
                CharacterStatus.Status.Stunned
            };
            UserRole = UserRole.Player;

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
            var target = input.ElementAtOrDefault(1);

            if (string.IsNullOrEmpty(target))
            {
                Handler.Client.WriteLine("<p>Drink what?</p>", player.ConnectionId);
                return;
            }
            
            var findNth = Helpers.findNth(target);
            var drink = Helpers.findObjectInInventory(findNth, player) ??
                        Helpers.findRoomObject(findNth, room);

            if (drink == null)
            {
                Handler.Client.WriteLine("<p>You can't find that.</p>", player.ConnectionId);
                return;
            }

            if (drink.ItemType != Item.Item.ItemTypes.Drink)
            {
                Handler.Client.WriteLine($"<p>You can't drink from {drink.Name.ToLower()}.</p>", player.ConnectionId);
                return;
            }
            
            //TODO: Add drink affects
            
            Handler.Client.WriteLine($"<p>You drink from {drink.Name.ToLower()}.</p>", player.ConnectionId);
            Handler.Client.WriteToOthersInRoom($"<p>{player.Name} drink from {drink.Name.ToLower()}.</p>", room, player);
        }
    }
}