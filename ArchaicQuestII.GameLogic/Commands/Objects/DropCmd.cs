using System;
using System.Collections.Generic;
using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Equipment;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Objects;

public class DropCmd : ICommand
{
    public DropCmd(IWriteToClient writeToClient, ICache cache, IUpdateClientUI updateClient, IRoomActions roomActions)
    {
        Aliases = new[] { "drop" };
        Description = "Tries to drop items or gold.";
        Usages = new[] { "Example: drop apple", "Example: drop all", "Example: drop apple chest" };
        UserRole = UserRole.Player;
        Writer = writeToClient;
        Cache = cache;
        UpdateClient = updateClient;
        RoomActions = roomActions;
    }

    public string[] Aliases { get; }
    public string Description { get; }
    public string[] Usages { get; }
    public UserRole UserRole { get; }
    public IWriteToClient Writer { get; }
    public ICache Cache { get; }
    public IUpdateClientUI UpdateClient { get; }
    public IRoomActions RoomActions { get; }

    public void Execute(Player player, Room room, string[] input)
    {
        var target = input.ElementAtOrDefault(1);
        var container = input.ElementAtOrDefault(2);
        
        if (string.IsNullOrEmpty(target))
        {
            Writer.WriteLine("<p>Drop what?</p>", player.ConnectionId);
            return;
        }
        
        if (player.Affects.Blind)
        {
            Writer.WriteLine("<p>You are blind and can't see a thing!</p>", player.ConnectionId);
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
            Writer.WriteLine("<p>You don't have that item.</p>", player.ConnectionId);
            return;
        }

        if (item.Equipped)
        {
            Writer.WriteLine($"<p>You must remove {item.Name.ToLower()} before you can drop it.</p>",
                player.ConnectionId);
            return;
        }

        if ((item.ItemFlag & Item.Item.ItemFlags.Nodrop) != 0)
        {
            Writer.WriteLine($"<p>You can't let go of {item.Name.ToLower()}. It appears to be cursed.</p>",
                player.ConnectionId);
            return;
        }


        player.Inventory.Remove(item);

        foreach (var pc in room.Players)
        {
            if (pc.Name == player.Name)
            {
                continue;
            }

            Writer.WriteLine($"<p>{player.Name} drops {item.Name.ToLower()}.</p>",
                pc.ConnectionId);
        }

        room.Items.Add(item);
        player.Weight -= item.Weight;

        Writer.WriteLine($"<p>You drop {item.Name.ToLower()}.</p>", player.ConnectionId);
        UpdateClient.UpdateInventory(player);
        UpdateClient.UpdateScore(player);
    }

    private void DropAll(Player player, Room room)
    {
        if (player.Inventory.Count == 0)
        {
            Writer.WriteLine("<p>You don't have anything to drop.</p>", player.ConnectionId);
            return;
        }

        for (var i = player.Inventory.Count - 1; i >= 0; i--)
        {
            if (player.Inventory[i].Stuck == false && player.Inventory[i].Equipped == false)
            {

                if (player.Inventory[i].Equipped)
                {
                    Writer.WriteLine(
                        $"<p>You must remove {player.Inventory[i].Name.ToLower()} before you can drop it.</p>",
                        player.ConnectionId);
                    return;
                }

                if ((player.Inventory[i].ItemFlag & Item.Item.ItemFlags.Nodrop) != 0)
                {
                    Writer.WriteLine($"<p>You can't let go of {player.Inventory[i].Name}. It appears to be cursed.</p>", player.ConnectionId);
                    return;
                }
                
                UpdateClient.PlaySound("drop", player);
                room.Items.Add(player.Inventory[i]);
                player.Weight -= player.Inventory[i].Weight;

                Writer.WriteLine($"<p>You drop {player.Inventory[i].Name.ToLower()}.</p>", player.ConnectionId);

                foreach (var pc in room.Players.Where(pc => pc.Name != player.Name))
                {
                    Writer.WriteLine($"<p>{player.Name} drops {player.Inventory[i].Name.ToLower()}.</p>",
                        pc.ConnectionId);
                }
                
                player.Inventory.RemoveAt(i);

            }
        }
        UpdateClient.UpdateInventory(player);
        // TODO: You are over encumbered 
    }

    private void DropGold(Player player, Room room, int amount)
    {
        if (player.Money.Gold < amount)
        {
            Writer.WriteLine("<p>You don't have that much gold to drop.</p>", player.ConnectionId);
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

        Writer.WriteLine($"<p>You drop {(amount == 1 ? "1 gold coin." : $"{amount} gold coins.")}</p>",
            player.ConnectionId);

        foreach (var pc in room.Players.Where(pc => pc.Name != player.Name))
        {
            Writer.WriteLine($"<p>{player.Name} drops {ItemList.DisplayMoneyAmount(amount).ToLower()}.</p>",
                pc.ConnectionId);
        }

        player.Money.Gold -= amount;
        room.Items.Add(goldCoin);

        player.Weight -= amount * 0.1;

        UpdateClient.UpdateScore(player);
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
            Writer.WriteLine($"<p>You don't see that here.</p>", player.ConnectionId);
            return;
        }

        if (!containerObj.Container.IsOpen)
        {
            Writer.WriteLine($"<p>You need to open it first.</p>", player.ConnectionId);
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
            Writer.WriteLine("<p>You don't have that item.</p>", player.ConnectionId);
            return;
        }

        if ((item.ItemFlag & Item.Item.ItemFlags.Nodrop) != 0)
        {
            Writer.WriteLine($"<p>You can't let go of {item.Name}. It appears to be cursed.</p>", player.ConnectionId);
            return;
        }

        player.Inventory.Remove(item);
        player.Weight -= item.Weight;

        foreach (var pc in room.Players.Where(pc => pc.Name != player.Name))
        {
            Writer.WriteLine($"<p>{player.Name} puts {item.Name.ToLower()} into {containerObj.Name.ToLower()}.</p>",
                pc.ConnectionId);
        }

        containerObj.Container.Items.Add(item);
        UpdateClient.PlaySound("drop", player);
        Writer.WriteLine($"<p>You put {item.Name.ToLower()} into {containerObj.Name.ToLower()}.</p>",
            player.ConnectionId);
        UpdateClient.UpdateInventory(player);
        UpdateClient.UpdateScore(player);
    }

    private void DropAllInContainer(Player player, Room room, Item.Item container)
    {
        if (player.Inventory.Count == 0)
        {
            Writer.WriteLine("<p>You don't have anything to drop.</p>", player.ConnectionId);
            return;
        }

        for (var i = player.Inventory.Count - 1; i >= 0; i--)
        {
            if((player.Inventory[i].ItemFlag & Item.Item.ItemFlags.Nodrop) != 0)
            {
                Writer.WriteLine($"<p>You can't let go of {player.Inventory[i].Name}. It appears to be cursed.</p>", player.ConnectionId);
                continue;
            }
            UpdateClient.PlaySound("drop", player);
            container.Container.Items.Add(player.Inventory[i]);
            player.Weight -= player.Inventory[i].Weight;
            Writer.WriteLine($"<p>You place {player.Inventory[i].Name.ToLower()} into {container.Name.ToLower()}.</p>", player.ConnectionId);

            foreach (var pc in room.Players.Where(pc => pc.Name != player.Name))
            {
                Writer.WriteLine($"<p>{player.Name} puts {player.Inventory.Name.ToLower()} into {container.Name.ToLower()}.</p>",
                    pc.ConnectionId);
            }
            
            player.Inventory.RemoveAt(i);
        }
        UpdateClient.UpdateInventory(player);
        UpdateClient.UpdateScore(player);
        // TODO: You are over encumbered 
    }
}