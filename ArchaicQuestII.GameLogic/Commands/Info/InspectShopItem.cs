using System;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Info;

public class InspectShopItemCmd : ICommand
{
    public InspectShopItemCmd(ICore core)
    {
        Aliases = new[] {"inspect", "inpect", "insp",};
        Description = "Inspect an item that is being sold at a shop, the mob will explain the properties of the item. You may also be interested in the help files " +
                      "list, buy or sell.";
        Usages = new[] {"Type: inspect lantern to view the properties of a lantern."};
            Title = "Shop - Inspect Item";
    DeniedStatus = null;
        UserRole = UserRole.Player;
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
        var itemName = input.ElementAtOrDefault(1);

        if (string.IsNullOrEmpty(itemName))
        {
            Core.Writer.WriteLine("Inspect what?");
            return;
        }
         if (int.TryParse(itemName, out var n))
            {
                InspectItem(n, room, player);
                return;
            }

            var vendor = room.Mobs.FirstOrDefault(x => x.Shopkeeper.Equals(true));

            if (vendor == null)
            {
                Core.Writer.WriteLine("<p>You can't do that here.</p>", player.ConnectionId);
                return;
            }

            var hasItem = vendor.Inventory.FirstOrDefault(x =>
                x.Name.Contains(itemName, StringComparison.InvariantCultureIgnoreCase));

            if (hasItem == null)
            {
                Core.Writer.WriteLine($"<p>{vendor.Name} says 'I don't have anything like that to show you.'</p>", player.ConnectionId);
                return;
            }

            var sb = new StringBuilder();
            sb.Append($"<p>{vendor.Name} explains to you the properties of {hasItem.Name.ToLower()}:</p>");
            sb.Append(
                $"<p>'{hasItem.Name}' is type {hasItem.ItemType}<br />weight is {hasItem.Weight}, value is {hasItem.Value}, level is {hasItem.Level}.<br/>");

            string flags = "Extra flags: ";
            foreach (Enum value in Enum.GetValues(hasItem.ItemFlag.GetType()))
            {
                if (hasItem.ItemFlag.HasFlag(value))
                {

                    flags += value + ", ";

                }
            }

            if (hasItem.ItemType == Item.Item.ItemTypes.Armour)
            {
                sb.Append($"Armour Type: {hasItem.ArmourType}, Defense {hasItem.ArmourRating.Armour} and {hasItem.ArmourRating.Magic} vs magic.<br />");
            }

            if (hasItem.ItemType == Item.Item.ItemTypes.Weapon)
            {
                sb.Append($"Weapon Type: {hasItem.WeaponType}, Damage is {hasItem.Damage.Minimum}-{hasItem.Damage.Maximum} (average {hasItem.Damage.Minimum + hasItem.Damage.Maximum / 2}).<br />");
                sb.Append($"Damage type: {hasItem.DamageType}</br>");
            }

            sb.Append($"{flags}<br />");
            sb.Append("</p>");
            Core.Writer.WriteLine(sb.ToString(), player.ConnectionId);
    }

      public void InspectItem(int itemNumber, Room room, Player player)
        {

            itemNumber -= 1;
            if (itemNumber < 0)
            {
                itemNumber = 0;
            }

            var vendor = room.Mobs.FirstOrDefault(x => x.Shopkeeper.Equals(true));

            if (vendor == null)
            {
                Core.Writer.WriteLine("<p>You can't do that here.</p>", player.ConnectionId);
                return;
            }

            var hasItem = vendor.Inventory.Distinct().OrderBy(x => x.Level).ThenBy(x => x.Value).ToArray()[itemNumber];
            if (hasItem == null)
            {
                Core.Writer.WriteLine($"<p>{vendor.Name} says 'I don't have anything like that to show you.'</p>", player.ConnectionId);
                return;
            }

            var sb = new StringBuilder();
            sb.Append($"<p>{vendor.Name} explains to you the properties of {hasItem.Name.ToLower()}:</p>");
            sb.Append(
                $"<p>'{hasItem.Name}' is type {hasItem.ItemType}<br />weight is {hasItem.Weight}, value is {hasItem.Value}, level is {hasItem.Level}.<br/>");

            string flags = "Extra flags: ";
            foreach (Enum value in Enum.GetValues(hasItem.ItemFlag.GetType()))
            {
                if (hasItem.ItemFlag.HasFlag(value))
                {

                    flags += value + ", ";

                }
            }

            if (hasItem.ItemType == Item.Item.ItemTypes.Armour)
            {
                sb.Append($"Armour Type: {hasItem.ArmourType}, Defense {hasItem.ArmourRating.Armour} and {hasItem.ArmourRating.Magic} vs magic.<br />");
            }

            if (hasItem.ItemType == Item.Item.ItemTypes.Weapon)
            {
                sb.Append($"Weapon Type: {hasItem.WeaponType}, Damage is {hasItem.Damage.Minimum}-{hasItem.Damage.Maximum} (average {hasItem.Damage.Minimum + hasItem.Damage.Maximum / 2}).<br />");
                sb.Append($"Damage type: {hasItem.DamageType}</br>");
            }

            sb.Append($"{flags}<br />");
            sb.Append("</p>");
            Core.Writer.WriteLine(sb.ToString(), player.ConnectionId);
        }
 
}