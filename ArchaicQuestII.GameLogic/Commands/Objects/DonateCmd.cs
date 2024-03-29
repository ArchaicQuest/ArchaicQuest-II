using System.Collections.Generic;
using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Objects;

public class DonateCmd : ICommand
{
    public DonateCmd()
    {
        Aliases = new[] { "donate" };
        Description =
            @"'{yellow}donate{/}' is used to donate the specified item or gold from your inventory to a random donation room.  

Examples:
donate sword 
donate 2.sword (if you have several items the same you can pick the nth one)
donate 100 gold 

Related help files: get, put, give, drop
";
        Usages = new[] { "Example: donate apple, Example: drop 100 gold" };
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
        if (!GetRandomDonationRoom(out var donationRoom))
        {
            Services.Instance.Writer.WriteLine("<p>There are no donation rooms.</p>", player);
        }

        var target = input.ElementAtOrDefault(1);
        var container = input.ElementAtOrDefault(2);

        if (string.IsNullOrEmpty(target))
        {
            Services.Instance.Writer.WriteLine("<p>Donate what?</p>", player);
            return;
        }

        if (player.Affects.Blind)
        {
            Services.Instance.Writer.WriteLine(
                "<p>You are blind and can't see a thing!</p>",
                player
            );
            return;
        }

        if (int.TryParse(target, out var amount) && container.ToLower() == "gold")
        {
            DonateGold(player, amount);
            return;
        }

        var nthTarget = Helpers.findNth(target);

        var item = player.FindObjectInInventory(nthTarget);

        if (item == null)
        {
            Services.Instance.Writer.WriteLine("<p>You don't have that item.</p>", player);
            return;
        }

        if (item.Equipped)
        {
            Services.Instance.Writer.WriteLine(
                $"<p>You must remove {item.Name.ToLower()} before you can donate it.</p>",
                player
            );
            return;
        }

        if ((item.ItemFlag & Item.Item.ItemFlags.Nodrop) != 0)
        {
            Services.Instance.Writer.WriteLine(
                $"<p>You can't let go of {item.Name.ToLower()}. It appears to be cursed.</p>",
                player
            );
            return;
        }

        player.Inventory.Remove(item);
        donationRoom.Items.Add(item);
        player.Weight -= item.Weight;

        Services.Instance.Writer.WriteLine($"<p>You donate {item.Name.ToLower()}.</p>", player);

        foreach (var pc in Services.Instance.Cache.GetAllPlayers())
        {
            if (pc.Config.NewbieChannel)
                Services.Instance.UpdateClient.UpdateCommunication(
                    pc,
                    $"<p class='newbie'>[<span>Newbie</span>]: {player.Name} donates {item.Name.ToLower()} to {donationRoom.Title}.</p>",
                    "newbie"
                );
        }

        Helpers.PostToDiscord(
            $"<p>[Newbie] {player.Name} donates {item.Name.ToLower()} to {donationRoom.Title}.</p>",
            "channels"
        );

        Services.Instance.UpdateClient.UpdateInventory(player);
        Services.Instance.UpdateClient.UpdateScore(player);
    }

    private void DonateGold(Player player, int amount)
    {
        if (!GetRandomDonationRoom(out var donationRoom))
        {
            Services.Instance.Writer.WriteLine("<p>There are no donation rooms.</p>", player);
        }

        if (player.Money.Gold < amount)
        {
            Services.Instance.Writer.WriteLine(
                "<p>You don't have that much gold to donate.</p>",
                player
            );
        }

        var goldCoin = new Item.Item
        {
            Name = "Gold Coin",
            Value = amount,
            ItemType = Item.Item.ItemTypes.Money,
            ArmourType = Item.Item.ArmourTypes.Cloth,
            AttackType = Item.Item.AttackTypes.Charge,
            WeaponType = SkillName.None,
            Gold = 1,
            Slot = EquipmentSlot.Hands,
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
            Book = new Book { Pages = new List<string>() },
            ArmourRating = new ArmourRating(),
            Container = new Container { Items = new ItemList() }
        };

        Services.Instance.Writer.WriteLine(
            $"<p>You donate {(amount == 1 ? "1 gold coin." : $"{amount} gold coins.")}</p>",
            player
        );

        foreach (var pc in Services.Instance.Cache.GetAllPlayers())
        {
            if (pc.Config.NewbieChannel)
                Services.Instance.UpdateClient.UpdateCommunication(
                    pc,
                    $"<p class='newbie'>[<span>Newbie</span>]: {player.Name} donates {ItemList.DisplayMoneyAmount(amount).ToLower()} to {donationRoom.Title}.</p>",
                    "newbie"
                );
        }

        Helpers.PostToDiscord(
            $"<p>[Newbie] {player.Name} donates {ItemList.DisplayMoneyAmount(amount).ToLower()} to {donationRoom.Title}.</p>",
            "channels"
        );

        player.Money.Gold -= amount;
        donationRoom.Items.Add(goldCoin);

        player.Weight -= amount * 0.1;

        Services.Instance.UpdateClient.UpdateScore(player);
    }

    private bool GetRandomDonationRoom(out Room donationRoom)
    {
        var rooms = new List<Room>();

        foreach (Room room in Services.Instance.Cache.GetAllRooms())
        {
            if (room.RoomFlags.Contains(Room.RoomFlag.Donation))
                rooms.Add(room);
        }

        if (rooms.Count == 0)
        {
            donationRoom = null;
            return false;
        }

        donationRoom = rooms[DiceBag.Roll(1, 0, rooms.Count - 1)];

        return true;
    }
}
