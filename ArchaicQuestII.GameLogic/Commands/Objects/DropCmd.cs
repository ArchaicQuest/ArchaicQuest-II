using System;
using System.Collections.Generic;
using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Equipment;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Objects;

public class DropCmd : ICommand
{
    public DropCmd(ICore core)
    {
        Aliases = new[] { "drop", "put" };
        Description =  @"'{yellow}drop{/}' is used to drop the specified item or gold from your inventory to the ground, container, or a corpse.  

Examples:
drop sword 
drop 2.sword (if you have several items the same you can pick the nth one)
drop sword chest
drop 100 gold 
drop all 
drop all corpse

alternatively put can be used, traditionally in MUDs it's used to put items into containers, put sword chest

Related help files: get, put, give, drop
";
        Usages = new[] { "Example: drop apple, Example: drop all, Example: drop apple chest" };
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
        var target = input.ElementAtOrDefault(1);
        var container = input.ElementAtOrDefault(2);
        
        if (string.IsNullOrEmpty(target))
        {
            Core.Writer.WriteLine("<p>Drop what?</p>", player.ConnectionId);
            return;
        }
        
        if (player.Affects.Blind)
        {
            Core.Writer.WriteLine("<p>You are blind and can't see a thing!</p>", player.ConnectionId);
            return;
        }

        if (target == "all" && string.IsNullOrEmpty(container))
        {
            DropAll(player, room);
            return;
        }

        if (int.TryParse(target, out var amount) && container.ToLower() == "gold")
        {
            DropGold(player, room, amount);
        }

        if (!string.IsNullOrEmpty(container) && !int.TryParse(target, out var number))
        {
            DropInContainer(player, room, target, container);
            return;
        }

        var nthTarget = Helpers.findNth(target);

        var item = Helpers.findObjectInInventory(nthTarget, player);

        if (item == null)
        {
            Core.Writer.WriteLine("<p>You don't have that item.</p>", player.ConnectionId);
            return;
        }

        if (item.Equipped)
        {
            Core.Writer.WriteLine($"<p>You must remove {item.Name.ToLower()} before you can drop it.</p>",
                player.ConnectionId);
            return;
        }

        if ((item.ItemFlag & Item.Item.ItemFlags.Nodrop) != 0)
        {
            Core.Writer.WriteLine($"<p>You can't let go of {item.Name.ToLower()}. It appears to be cursed.</p>",
                player.ConnectionId);
            return;
        }

        player.Inventory.Remove(item);
        room.Items.Add(item);
        player.Weight -= item.Weight;

        Core.Writer.WriteLine($"<p>You drop {item.Name.ToLower()}.</p>", player.ConnectionId);
        Core.Writer.WriteToOthersInRoom($"<p>{player.Name} drops {item.Name.ToLower()}.</p>", room, player);
        
        Core.UpdateClient.UpdateInventory(player);
        Core.UpdateClient.UpdateScore(player);
    }

    private void DropAll(Player player, Room room)
    {
        if (player.Inventory.Count == 0)
        {
            Core.Writer.WriteLine("<p>You don't have anything to drop.</p>", player.ConnectionId);
            return;
        }

        for (var i = player.Inventory.Count - 1; i >= 0; i--)
        {
            if (player.Inventory[i].Stuck == false && player.Inventory[i].Equipped == false)
            {

                if (player.Inventory[i].Equipped)
                {
                    Core.Writer.WriteLine(
                        $"<p>You must remove {player.Inventory[i].Name.ToLower()} before you can drop it.</p>",
                        player.ConnectionId);
                    return;
                }

                if ((player.Inventory[i].ItemFlag & Item.Item.ItemFlags.Nodrop) != 0)
                {
                    Core.Writer.WriteLine($"<p>You can't let go of {player.Inventory[i].Name}. It appears to be cursed.</p>", player.ConnectionId);
                    return;
                }
                
                Core.UpdateClient.PlaySound("drop", player);
                room.Items.Add(player.Inventory[i]);
                player.Weight -= player.Inventory[i].Weight;

                Core.Writer.WriteLine($"<p>You drop {player.Inventory[i].Name.ToLower()}.</p>", player.ConnectionId);
                Core.Writer.WriteToOthersInRoom($"<p>{player.Name} drops {player.Inventory[i].Name.ToLower()}.</p>", room, player);
                
                player.Inventory.RemoveAt(i);

            }
        }
        Core.UpdateClient.UpdateInventory(player);
        // TODO: You are over encumbered 
    }

    private void DropGold(Player player, Room room, int amount)
    {
        if (player.Money.Gold < amount)
        {
            Core.Writer.WriteLine("<p>You don't have that much gold to drop.</p>", player.ConnectionId);
        }

        var goldCoin = new Item.Item
        {
            Name = "Gold Coin",
            Value = amount,
            ItemType = Item.Item.ItemTypes.Money,
            ArmourType = Item.Item.ArmourTypes.Cloth,
            AttackType = Item.Item.AttackTypes.Charge,
            WeaponType = Item.Item.WeaponTypes.Arrows,
            Gold = 1,
            Slot = Equipment.EqSlot.Hands,
            Level = 1,
            Modifier = new Modifier(),
            Description = new Description
            {
                Look =
                    "A small gold coin with an embossed crown on one side and the number one on the opposite side, along the edge inscribed is 'de omnibus dubitandum'",
                Exam =
                    "A small gold coin with an embossed crown on one side and the number one on the opposite side, along the edge inscribed is 'de omnibus dubitandum'",
                Room = "A single gold coin.",
            },
            Book = new Book
            {
                Pages = new List<string>()
            },
            ArmourRating = new ArmourRating(),
            Container = new Container
            {
                Items = new ItemList()
            }
        };

        Core.Writer.WriteLine($"<p>You drop {(amount == 1 ? "1 gold coin." : $"{amount} gold coins.")}</p>",
            player.ConnectionId);

        Core.Writer.WriteToOthersInRoom($"<p>{player.Name} drops {ItemList.DisplayMoneyAmount(amount).ToLower()}.</p>",
            room, player);

        player.Money.Gold -= amount;
        room.Items.Add(goldCoin);

        player.Weight -= amount * 0.1;

        Core.UpdateClient.UpdateScore(player);
    }


    private void DropInContainer(Player player, Room room, string target, string container)
    {
        var nthItem = Helpers.findNth(target);
        
        var containerObj = Helpers.findRoomObject(nthItem, room);

        if (containerObj == null)
        {
            var nthContainer = Helpers.findNth(container);
            containerObj = Helpers.findRoomObject(nthContainer, room) ??
                           Helpers.findObjectInInventory(nthContainer, player);
        }

        if (containerObj == null)
        {
            Core.Writer.WriteLine($"<p>You don't see that here.</p>", player.ConnectionId);
            return;
        }

        if (!containerObj.Container.IsOpen)
        {
            Core.Writer.WriteLine($"<p>You need to open it first.</p>", player.ConnectionId);
            return;
        }

        if (target == "all")
        {
            DropAllInContainer(player, room, containerObj);
            return;
        }

        var item = player.Inventory.Where(x => x.Stuck == false)
            .FirstOrDefault(x => x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase));

        if (item == null)
        {
            Core.Writer.WriteLine("<p>You don't have that item.</p>", player.ConnectionId);
            return;
        }

        if ((item.ItemFlag & Item.Item.ItemFlags.Nodrop) != 0)
        {
            Core.Writer.WriteLine($"<p>You can't let go of {item.Name}. It appears to be cursed.</p>", player.ConnectionId);
            return;
        }

        player.Inventory.Remove(item);
        player.Weight -= item.Weight;

        containerObj.Container.Items.Add(item);
        Core.UpdateClient.PlaySound("drop", player);
        
        Core.Writer.WriteLine($"<p>You put {item.Name.ToLower()} into {containerObj.Name.ToLower()}.</p>",
            player.ConnectionId);
        Core.Writer.WriteToOthersInRoom($"<p>{player.Name} puts {item.Name.ToLower()} into {containerObj.Name.ToLower()}.</p>",
            room, player);
        
        Core.UpdateClient.UpdateInventory(player);
        Core.UpdateClient.UpdateScore(player);
    }

    private void DropAllInContainer(Player player, Room room, Item.Item container)
    {
        if (player.Inventory.Count == 0)
        {
            Core.Writer.WriteLine("<p>You don't have anything to drop.</p>", player.ConnectionId);
            return;
        }

        for (var i = player.Inventory.Count - 1; i >= 0; i--)
        {
            if((player.Inventory[i].ItemFlag & Item.Item.ItemFlags.Nodrop) != 0)
            {
                Core.Writer.WriteLine($"<p>You can't let go of {player.Inventory[i].Name}. It appears to be cursed.</p>", player.ConnectionId);
                continue;
            }
            Core.UpdateClient.PlaySound("drop", player);
            container.Container.Items.Add(player.Inventory[i]);
            player.Weight -= player.Inventory[i].Weight;
            
            Core.Writer.WriteLine($"<p>You place {player.Inventory[i].Name.ToLower()} into {container.Name.ToLower()}.</p>", player.ConnectionId);
            Core.Writer.WriteToOthersInRoom($"<p>{player.Name} puts {player.Inventory.Name.ToLower()} into {container.Name.ToLower()}.</p>",
                room, player);
            
            player.Inventory.RemoveAt(i);
        }
        Core.UpdateClient.UpdateInventory(player);
        Core.UpdateClient.UpdateScore(player);
        // TODO: You are over encumbered 
    }
}