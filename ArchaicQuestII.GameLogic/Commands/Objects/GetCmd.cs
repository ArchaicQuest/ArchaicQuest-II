using System;
using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Objects;

public class GetCmd : ICommand
{
    public GetCmd()
    {
        Aliases = new[] { "get", "take", "loot" };
        Description =
            @"'{yellow}get{/}' is used to get the specified item or gold from the ground, container, or a corpse.  

Examples:
get sword 
get 2.sword (if you have several items the same you can pick the nth one)
get sword chest
get all 
get all corpse

Related help files: drop, put, give
";
        Usages = new[] { "Type: get apple, get all, get apple crate" };
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
        var container = input.ElementAtOrDefault(2);

        if (string.IsNullOrEmpty(target))
        {
            Services.Instance.Writer.WriteLine("<p>Get what?</p>", player.ConnectionId);
            return;
        }

        if (player.Affects.Blind)
        {
            Services.Instance.Writer.WriteLine(
                "<p>You are blind and can't see a thing!</p>",
                player.ConnectionId
            );
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
                containerObj =
                    Helpers.findRoomObject(nthContainer, room)
                    ?? Helpers.findObjectInInventory(nthContainer, player);
            }

            if (containerObj == null)
            {
                Services.Instance.Writer.WriteLine(
                    "<p>You don't see that here.</p>",
                    player.ConnectionId
                );
                return;
            }

            if (
                containerObj.ItemType != Item.Item.ItemTypes.Container
                && containerObj.ItemType != Item.Item.ItemTypes.Cooking
            )
            {
                if (containerObj.ItemType == Item.Item.ItemTypes.Forage)
                {
                    Services.Instance.Writer.WriteLine(
                        "<p>Try forage instead.</p>",
                        player.ConnectionId
                    );
                    return;
                }

                Services.Instance.Writer.WriteLine(
                    "<p>This is not a container.</p>",
                    player.ConnectionId
                );
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
            Services.Instance.Writer.WriteLine(
                "<p>You don't see that here.</p>",
                player.ConnectionId
            );
            return;
        }

        if (item.Stuck)
        {
            Services.Instance.Writer.WriteLine(
                "<p>You can't pick that up.</p>",
                player.ConnectionId
            );
            return;
        }

        room.Items.Remove(item);

        if (item.ItemType == Item.Item.ItemTypes.Money)
            Services.Instance.Writer.WriteToOthersInRoom(
                $"<p>{player.Name} picks up {ItemList.DisplayMoneyAmount(item.Value).ToLower()}.</p>",
                room,
                player
            );
        else
            Services.Instance.Writer.WriteToOthersInRoom(
                $"<p>{player.Name} picks up {item.Name.ToLower()}.</p>",
                room,
                player
            );

        if (item.ItemType == Item.Item.ItemTypes.Money)
        {
            Services.Instance.Writer.WriteLine(
                $"<p>You pick up {ItemList.DisplayMoneyAmount(item.Value).ToLower()}.</p>",
                player.ConnectionId
            );
            player.Money.Gold += item.Value;
            player.Weight += item.Value * 0.1;
        }
        else
        {
            item.IsHiddenInRoom = false;
            player.Inventory.Add(item);
            player.Weight += item.Weight;
            Services.Instance.Writer.WriteLine(
                $"<p>You pick up {item.Name.ToLower()}.</p>",
                player.ConnectionId
            );
        }

        Services.Instance.UpdateClient.UpdateInventory(player);
        Services.Instance.UpdateClient.UpdateScore(player);
        room.Clean = false;

        if (player.Weight > player.Attributes.Attribute[EffectLocation.Strength] * 3)
        {
            Services.Instance.Writer.WriteLine(
                $"<p>You are now over encumbered by carrying too much weight.</p>",
                player.ConnectionId
            );
        }
    }

    private void GetAll(Player player, Room room)
    {
        if (room.Items.Count == 0)
        {
            Services.Instance.Writer.WriteLine(
                "<p>You don't see anything here.</p>",
                player.ConnectionId
            );
            return;
        }

        for (var i = room.Items.Count - 1; i >= 0; i--)
        {
            if (room.Items[i].Stuck == false)
            {
                if (room.Items[i].ItemType == Item.Item.ItemTypes.Money)
                {
                    Services.Instance.Writer.WriteLine(
                        $"<p>You pick up {ItemList.DisplayMoneyAmount(room.Items[i].Value).ToLower()}.</p>",
                        player.ConnectionId
                    );

                    player.Money.Gold += room.Items[i].Value;
                    player.Weight += room.Items[i].Value * 0.1;
                }
                else
                {
                    Services.Instance.UpdateClient.PlaySound("get", player);
                    room.Items[i].IsHiddenInRoom = false;
                    player.Inventory.Add(room.Items[i]);
                    Services.Instance.Writer.WriteLine(
                        $"<p>You pick up {room.Items[i].Name.ToLower()}</p>",
                        player.ConnectionId
                    );
                    player.Weight += room.Items[i].Weight;
                }

                if (room.Items[i].ItemType == Item.Item.ItemTypes.Money)
                    Services.Instance.Writer.WriteToOthersInRoom(
                        $"<p>{player.Name} picks up {ItemList.DisplayMoneyAmount(room.Items[i].Value).ToLower()}.</p>",
                        room,
                        player
                    );
                else
                    Services.Instance.Writer.WriteToOthersInRoom(
                        $"<p>{player.Name} picks up {room.Items[i].Name.ToLower()}.</p>",
                        room,
                        player
                    );

                room.Items.RemoveAt(i);
            }
            else
            {
                Services.Instance.Writer.WriteLine(
                    "<p>You can't get that.</p>",
                    player.ConnectionId
                );
            }
        }

        room.Clean = false;
        Services.Instance.UpdateClient.UpdateInventory(player);
        Services.Instance.UpdateClient.UpdateScore(player);

        if (player.Weight > player.Attributes.Attribute[EffectLocation.Strength] * 3)
        {
            Services.Instance.Writer.WriteLine(
                $"<p>You are now over encumbered by carrying too much weight.</p>",
                player.ConnectionId
            );
        }
    }

    private void GetFromContainer(Player player, Room room, string target, Item.Item container)
    {
        if (container.Container.CanOpen && !container.Container.IsOpen)
        {
            Services.Instance.Writer.WriteLine(
                "<p>You need to open it first.</p>",
                player.ConnectionId
            );
            return;
        }

        if (target == "all")
        {
            GetAllFromContainer(player, room, container);
            return;
        }

        var item = container.Container.Items
            .Where(x => x.Stuck == false)
            .FirstOrDefault(
                x => x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase)
            );

        if (item == null)
        {
            Services.Instance.Writer.WriteLine(
                "<p>You don't have that item.</p>",
                player.ConnectionId
            );
            return;
        }

        container.Container.Items.Remove(item);

        if (item.ItemType == Item.Item.ItemTypes.Money)
            Services.Instance.Writer.WriteToOthersInRoom(
                $"<p>{player.Name} gets {ItemList.DisplayMoneyAmount(item.Value).ToLower()} from {container.Name.ToLower()}</p>",
                room,
                player
            );
        else
            Services.Instance.Writer.WriteToOthersInRoom(
                $"<p>{player.Name} gets {item.Name.ToLower()} from {container.Name.ToLower()}.</p>",
                room,
                player
            );

        if (item.ItemType == Item.Item.ItemTypes.Money)
        {
            Services.Instance.Writer.WriteLine(
                $"<p>You get {ItemList.DisplayMoneyAmount(item.Value).ToLower()} from {container.Name.ToLower()}.</p>",
                player.ConnectionId
            );
            player.Money.Gold += item.Value;
            player.Weight += item.Value * 0.1;
        }
        else
        {
            Services.Instance.UpdateClient.PlaySound("get", player);
            item.IsHiddenInRoom = false;
            player.Inventory.Add(item);
            player.Weight += item.Weight;
            Services.Instance.Writer.WriteLine(
                $"<p>You get {item.Name.ToLower()} from {container.Name.ToLower()}.</p>",
                player.ConnectionId
            );
        }

        Services.Instance.UpdateClient.UpdateInventory(player);
        Services.Instance.UpdateClient.UpdateScore(player);
        room.Clean = false;

        if (player.Weight > player.Attributes.Attribute[EffectLocation.Strength] * 3)
        {
            Services.Instance.Writer.WriteLine(
                $"<p>You are now over encumbered by carrying too much weight.</p>",
                player.ConnectionId
            );
        }
    }

    private void GetAllFromContainer(Player player, Room room, Item.Item container)
    {
        if (container.Container.Items.Count == 0)
        {
            Services.Instance.Writer.WriteLine(
                $"<p>You see nothing in {container.Name.ToLower()}.</p>",
                player.ConnectionId
            );
            return;
        }

        for (var i = container.Container.Items.Count - 1; i >= 0; i--)
        {
            if (container.Container.Items[i].ItemType == Item.Item.ItemTypes.Money)
            {
                Services.Instance.Writer.WriteLine(
                    $"<p>You pick up {ItemList.DisplayMoneyAmount(container.Container.Items[i].Value).ToLower()} from {container.Name.ToLower()}.</p>",
                    player.ConnectionId
                );

                player.Money.Gold += container.Container.Items[i].Value;
                player.Weight += 0.1;
            }
            else
            {
                Services.Instance.UpdateClient.PlaySound("get", player);
                container.Container.Items[i].IsHiddenInRoom = false;
                player.Inventory.Add(container.Container.Items[i]);
                player.Weight += container.Container.Items[i].Weight;
                Services.Instance.Writer.WriteLine(
                    $"<p>You pick up {container.Container.Items[i].Name.ToLower()} from {container.Name.ToLower()}.</p>",
                    player.ConnectionId
                );
            }

            if (container.Container.Items[i].ItemType == Item.Item.ItemTypes.Money)
                Services.Instance.Writer.WriteToOthersInRoom(
                    $"<p>{player.Name} picks up {ItemList.DisplayMoneyAmount(container.Container.Items.Value).ToLower()} from {container.Name.ToLower()}.</p>",
                    room,
                    player
                );
            else
                Services.Instance.Writer.WriteToOthersInRoom(
                    $"<p>{player.Name} picks up {container.Container.Items[i].Name.ToLower()} from {container.Name.ToLower()}</p>",
                    room,
                    player
                );

            container.Container.Items.RemoveAt(i);
        }

        Services.Instance.UpdateClient.UpdateInventory(player);
        Services.Instance.UpdateClient.UpdateScore(player);
        room.Clean = false;
        if (player.Weight > player.Attributes.Attribute[EffectLocation.Strength] * 3)
        {
            Services.Instance.Writer.WriteLine(
                $"<p>You are now over encumbered by carrying too much weight.</p>",
                player.ConnectionId
            );
        }
    }
}
