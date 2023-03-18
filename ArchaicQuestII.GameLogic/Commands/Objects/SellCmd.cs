using System;
using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Objects;

public class SellCmd : ICommand
{
    public SellCmd(ICore core)
    {
        Aliases = new[] {"sell", "sel"};
        Description = "Sell items in your inventory to a shop keeper. You can also use list to view items for sale and inspect to view item properties or buy to buy items. ";
        Usages = new[] {"Type: sell <Item> e.g sell Sword."};
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
            CharacterStatus.Status.Resting,
            CharacterStatus.Status.Sitting,
        };
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
            Core.Writer.WriteLine("<p>Sell what?</p>", player.ConnectionId);
            return;
        }
        var vendor = room.Mobs.FirstOrDefault(x => x.Shopkeeper.Equals(true));

        if (vendor == null)
        {
            Core.Writer.WriteLine("<p>You can't do that here.</p>", player.ConnectionId);
            return;
        }

        var hasItem = player.Inventory.FirstOrDefault(x =>
            x.Name.Contains(itemName, StringComparison.InvariantCultureIgnoreCase) && x.Equipped == false);

        if (hasItem == null)
        {
            Core.Writer.WriteLine($"<p>{vendor.Name} says 'You don't have that item.'</p>", player.ConnectionId);
            return;
        }

        var vendorInterested = vendor.Inventory.FirstOrDefault(x => x.ItemType.Equals(hasItem.ItemType));

        if (vendorInterested == null)
        {
            Core.Writer.WriteLine($"<p>{vendor.Name} says 'I'm not interested in {hasItem.Name.ToLower()}.'</p>", player.ConnectionId);
            return;
        }

        var vendorBuyPrice = (int)Math.Floor((decimal)hasItem.Value / 2);

        player.Money.Gold += vendorBuyPrice <= 0 ? 1 : vendorBuyPrice;
        player.Inventory.Remove(hasItem);

        // if we wanted to show sold items in the vendors list we would add it here
        // currently we can't set limits so this would make all items sold infinite 

        //if (vendor.Inventory.FirstOrDefault(x => x.Name.Equals(hasItem.Name)) == null)
        //{
        //    vendor.Inventory.Add(hasItem);
        //}

        player.Weight -= hasItem.Weight;
        Core.UpdateClient.UpdateScore(player);
        Core.UpdateClient.UpdateInventory(player);

        Core.Writer.WriteLine($"<p>You sell {hasItem.Name.ToLower()} for {(vendorBuyPrice <= 0 ? 1 : vendorBuyPrice)} gold.</p>", player.ConnectionId);
    }
   
}