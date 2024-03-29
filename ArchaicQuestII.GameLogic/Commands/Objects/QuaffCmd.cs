using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Objects;

public class QuaffCmd : ICommand
{
    public QuaffCmd()
    {
        Aliases = new[] { "quaff" };
        Description =
            "To drink potions you need to quaff them, 'quaff healing' will quaff a potion of healing. Drink is used for drinking water, beer or other liquids.";
        Usages = new[] { "Type: quaff potion" };
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
        };
        UserRole = UserRole.Player;
    }

    public string[] Aliases { get; }
    public string Description { get; }
    public string[] Usages { get; }
    public string Title { get; }
    public CharacterStatus.Status[] DeniedStatus { get; }
    public UserRole UserRole { get; }

    public void Execute(Player player, Room room, string[] input)
    {
        var target = input.ElementAtOrDefault(1);

        if (string.IsNullOrEmpty(target))
        {
            Services.Instance.Writer.WriteLine("<p>Quaff what?</p>", player);
            return;
        }

        var nthItem = Helpers.findNth(target);
        var foundItem =
            Helpers.findRoomObject(nthItem, room) ?? player.FindObjectInInventory(nthItem);

        if (foundItem == null)
        {
            Services.Instance.Writer.WriteLine("<p>You can't find that potion.</p>", player);
            return;
        }

        player.Inventory.Remove(foundItem);
        Services.Instance.UpdateClient.UpdateInventory(player);

        Services.Instance.UpdateClient.PlaySound("quaff", player);

        if (
            string.IsNullOrEmpty(foundItem.SpellName)
            && foundItem.ItemType == Item.Item.ItemTypes.Potion
        )
        {
            Services.Instance.Writer.WriteLine(
                "<p>You quaff the potion but nothing happens.</p>",
                player
            );
            return;
        }

        Services.Instance.Writer.WriteToOthersInRoom(
            $"{player.Name} quaffs {foundItem.Name.ToLower()}.",
            room,
            player
        );

        //potion to cast at level of potion and not the player level
        var dummyPlayer = new Player() { Level = foundItem.Level };

        //TODO: Fix this
        //_castSpell.CastSpell(foundItem.SpellName, string.Empty, player, foundItem.SpellName, dummyPlayer, room, false);
    }
}
