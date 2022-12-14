using System;
using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Objects;

public class GetCmd : ICommand
{
    public GetCmd(IWriteToClient writeToClient, ICache cache, IUpdateClientUI updateClient, IRoomActions roomActions)
    {
        Aliases = new[] {"get"};
        Description = "Your character will get something.";
        Usages = new[] {"Type: get apple"};
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
            Writer.WriteLine("<p>Get what?</p>", player.ConnectionId);
            return;
        }
        
        if (player.Affects.Blind)
        {
            Writer.WriteLine("<p>You are blind and can't see a thing!</p>", player.ConnectionId);
            return;
        }
        
        //TODO: Get all, get nth (get 2.apple)
        if (target == "all" && string.IsNullOrEmpty(container))
        {
            GetAll(player, room);
            return;
        }
        
        if (!string.IsNullOrEmpty(container))
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
                Writer.WriteLine("<p>You don't see that here.</p>", player.ConnectionId);
                return;
            }

            if (containerObj.ItemType != Item.Item.ItemTypes.Container)
            {
                if (containerObj.ItemType == Item.Item.ItemTypes.Forage)
                {
                    Writer.WriteLine("<p>Try forage instead.</p>", player.ConnectionId);
                    return;
                }

                Writer.WriteLine("<p>This is not a container.</p>", player.ConnectionId);
                return;
            }
            
            GetFromContainer(player, room, target, containerObj);
            return;
        }

        //Check room first
        var nthTarget = Helpers.findNth(target);

        var item = Helpers.findRoomObject(nthTarget, room);
        
        if (item == null)
        {
            Writer.WriteLine("<p>You don't see that here.</p>", player.ConnectionId);
            return;
        }

        if (item.Stuck)
        {
            Writer.WriteLine("<p>You can't pick that up.</p>", player.ConnectionId);
            return;
        }

        room.Items.Remove(item);

        foreach (var pc in room.Players.Where(pc => pc.Name != player.Name))
        {
            if (item.ItemType == Item.Item.ItemTypes.Money)
            {
                Writer.WriteLine($"<p>{player.Name} picks up {ItemList.DisplayMoneyAmount(item.Value).ToLower()}.</p>", pc.ConnectionId);
                continue;
            }
            
            Writer.WriteLine($"<p>{player.Name} picks up {item.Name.ToLower()}.</p>", pc.ConnectionId);
        }

        if (item.ItemType == Item.Item.ItemTypes.Money)
        {
            Writer.WriteLine($"<p>You pick up {ItemList.DisplayMoneyAmount(item.Value).ToLower()}.</p>", player.ConnectionId);
            player.Money.Gold += item.Value;
            player.Weight += item.Value * 0.1;
        }
        else
        {
            item.IsHiddenInRoom = false;
            player.Inventory.Add(item);
            player.Weight += item.Weight;
            Writer.WriteLine($"<p>You pick up {item.Name.ToLower()}.</p>", player.ConnectionId);
        }

        UpdateClient.UpdateInventory(player);
        UpdateClient.UpdateScore(player);
        room.Clean = false;
        
        // TODO: You are over encumbered 
    }

    private void GetAll(Player player, Room room)
    {
        if (room.Items.Count == 0)
        {
            Writer.WriteLine("<p>You don't see anything here.</p>", player.ConnectionId);
            return;
        }

        for (var i = room.Items.Count - 1; i >= 0; i--)
        {
            if (room.Items[i].Stuck == false)
            {
                if (room.Items[i].ItemType == Item.Item.ItemTypes.Money)
                {
                    Writer.WriteLine(
                        $"<p>You pick up {ItemList.DisplayMoneyAmount(room.Items[i].Value).ToLower()}.</p>",
                        player.ConnectionId);

                    player.Money.Gold += room.Items[i].Value;
                    player.Weight += room.Items[i].Value * 0.1;
                }
                else
                {
                    UpdateClient.PlaySound("get", player);
                    room.Items[i].IsHiddenInRoom = false;
                    player.Inventory.Add(room.Items[i]);
                    Writer.WriteLine($"<p>You pick up {room.Items[i].Name.ToLower()}</p>", player.ConnectionId);
                    player.Weight += room.Items[i].Weight;
                }

                foreach (var pc in room.Players.Where(pc => pc.Name != player.Name))
                {
                    Writer.WriteLine(
                        room.Items[i].ItemType == Item.Item.ItemTypes.Money
                            ? $"<p>{player.Name} picks up  {ItemList.DisplayMoneyAmount(room.Items[i].Value).ToLower()}</p>"
                            : $"<p>{player.Name} picks up {room.Items[i].Name.ToLower()}.</p>",
                        pc.ConnectionId);
                }

                room.Items.RemoveAt(i);
            }
            else
            {
                Writer.WriteLine("<p>You can't get that.</p>", player.ConnectionId);
            }
        }

        room.Clean = false;
        UpdateClient.UpdateInventory(player);
        UpdateClient.UpdateScore(player);
        // TODO: You are over encumbered 
    }

    private void GetFromContainer(Player player, Room room, string target, Item.Item container)
    {
        if (container.Container.CanOpen && !container.Container.IsOpen)
        {
            Writer.WriteLine("<p>You need to open it first.</p>", player.ConnectionId);
            return;
        }

        if (target == "all")
        {
            GetAllFromContainer(player, room, container);
            return;
        }

        var item = container.Container.Items.Where(x => x.Stuck == false)
            .FirstOrDefault(x => x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase));

        if (item == null)
        {
            Writer.WriteLine("<p>You don't have that item.</p>", player.ConnectionId);
            return;
        }

        container.Container.Items.Remove(item);

        foreach (var pc in room.Players.Where(pc => pc.Name != player.Name))
        {
            Writer.WriteLine(
                item.ItemType == Item.Item.ItemTypes.Money
                    ? $"<p>{player.Name} gets {ItemList.DisplayMoneyAmount(item.Value).ToLower()} from {container.Name.ToLower()}</p>"
                    : $"<p>{player.Name} gets {item.Name.ToLower()} from {container.Name.ToLower()}.</p>",
                pc.ConnectionId);
        }

        if (item.ItemType == Item.Item.ItemTypes.Money)
        {
            Writer.WriteLine(
                $"<p>You get {ItemList.DisplayMoneyAmount(item.Value).ToLower()} from {container.Name.ToLower()}.</p>",
                player.ConnectionId);
            player.Money.Gold += item.Value;
            player.Weight += item.Value * 0.1;
        }
        else
        {
            UpdateClient.PlaySound("get", player);
            item.IsHiddenInRoom = false;
            player.Inventory.Add(item);
            player.Weight += item.Weight;
            Writer.WriteLine($"<p>You get {item.Name.ToLower()} from {container.Name.ToLower()}.</p>",
                player.ConnectionId);
        }

        UpdateClient.UpdateInventory(player);
        UpdateClient.UpdateScore(player);
        room.Clean = false;
    }

    private void GetAllFromContainer(Player player, Room room, Item.Item container)
    {
        if (container.Container.Items.Count == 0)
        {
            Writer.WriteLine($"<p>You see nothing in {container.Name.ToLower()}.</p>", player.ConnectionId);
            return;
        }

        for (var i = container.Container.Items.Count - 1; i >= 0; i--)
        {
            if (container.Container.Items[i].ItemType == Item.Item.ItemTypes.Money)
            {
                Writer.WriteLine(
                    $"<p>You pick up {ItemList.DisplayMoneyAmount(container.Container.Items[i].Value).ToLower()} from {container.Name.ToLower()}.</p>",
                    player.ConnectionId);

                player.Money.Gold += container.Container.Items[i].Value;
                player.Weight += 0.1;
            }
            else
            {
                UpdateClient.PlaySound("get", player);
                container.Container.Items[i].IsHiddenInRoom = false;
                player.Inventory.Add(container.Container.Items[i]);
                player.Weight += container.Container.Items[i].Weight;
                Writer.WriteLine(
                    $"<p>You pick up {container.Container.Items[i].Name.ToLower()} from {container.Name.ToLower()}.</p>",
                    player.ConnectionId);
            }
            
            foreach (var pc in room.Players.Where(pc => pc.Name != player.Name))
            {
                Writer.WriteLine(
                    container.Container.Items[i].ItemType == Item.Item.ItemTypes.Money
                        ? $"<p>{player.Name} picks up {ItemList.DisplayMoneyAmount(container.Container.Items.Value).ToLower()} from {container.Name.ToLower()}.</p>"
                        : $"<p>{player.Name} picks up {container.Container.Items[i].Name.ToLower()} from {container.Name.ToLower()}</p>",
                    pc.ConnectionId);
            }

            container.Container.Items.RemoveAt(i);
        }

        UpdateClient.UpdateInventory(player);
        UpdateClient.UpdateScore(player);
        room.Clean = false;
        // TODO: You are over encumbered 
    }
}