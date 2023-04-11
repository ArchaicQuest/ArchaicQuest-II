using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;
using MoonSharp.Interpreter;

namespace ArchaicQuestII.GameLogic.Commands.Objects;

public class GiveCmd : ICommand
{
    public GiveCmd()
    {
        Aliases = new[] { "give" };
        Description =
            @"'{yellow}give{/}' is used to give the specified item or gold to a player or mob.  

Examples:
give sword charlotte
give 10 gold larissa

Related help files: get, put, give, drop
";
        Usages = new[]
        {
            "Type: give 'object' 'target', Example: give apple timmy, Example: give 10 gold timmy"
        };
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
        var targetName = input.ElementAtOrDefault(2);

        if (string.IsNullOrEmpty(targetName))
        {
            Services.Instance.Writer.WriteLine("<p>Give what to whom?</p>", player.ConnectionId);
            return;
        }

        string itemName;
        string itemAmount;

        if (input.Length == 4)
        {
            itemAmount = input[2];
            itemName = input[3];
        }
        else
        {
            itemName = input.ElementAtOrDefault(1);
            itemAmount = "1";
        }

        if (player.Affects.Blind)
        {
            Services.Instance.Writer.WriteLine(
                "<p>You are blind and can't see a thing!</p>",
                player.ConnectionId
            );
            return;
        }

        if (string.IsNullOrEmpty(itemName))
        {
            Services.Instance.Writer.WriteLine("<p>Give what to whom?</p>", player.ConnectionId);
            return;
        }

        var nthItem = Helpers.findNth(itemName);
        var nthTarget = Helpers.findNth(targetName);
        var target = Helpers.FindMob(nthTarget, room) ?? Helpers.findPlayerObject(nthTarget, room);

        if (target == null)
        {
            Services.Instance.Writer.WriteLine("<p>They aren't here.</p>", player.ConnectionId);
            return;
        }

        if (itemName == "gold" && int.TryParse(itemAmount, out var amount))
        {
            GiveGold(player, room, target, amount);
            return;
        }

        var item = player.FindObjectInInventory(nthItem);

        if (item == null)
        {
            Services.Instance.Writer.WriteLine(
                "<p>You do not have that item.</p>",
                player.ConnectionId
            );
            return;
        }

        if (item.Equipped)
        {
            Services.Instance.Writer.WriteLine(
                $"<p>You must remove {item.Name.ToLower()} before you can give it.</p>",
                player.ConnectionId
            );
            return;
        }

        if ((item.ItemFlag & Item.Item.ItemFlags.Nodrop) != 0)
        {
            Services.Instance.Writer.WriteLine(
                $"<p>You can't let go of {item.Name.ToLower()}. It appears to be cursed.</p>",
                player.ConnectionId
            );
            return;
        }

        player.Inventory.Remove(item);
        player.Weight -= item.Weight;

        Services.Instance.Writer.WriteLine(
            $"<p>You give {item.Name.ToLower()} to {target.Name.ToLower()}.</p>",
            player.ConnectionId
        );
        Services.Instance.Writer.WriteLine(
            $"<p>{player.Name} gives you {item.Name.ToLower()}.</p>",
            target.ConnectionId
        );
        Services.Instance.Writer.WriteToOthersInRoom(
            $"<p>{player.Name} gives {item.Name.ToLower()} to {target.Name.ToLower()}.</p>",
            room,
            player
        );

        target.Inventory.Add(item);
        Services.Instance.UpdateClient.UpdateInventory(player);
        Services.Instance.UpdateClient.UpdateInventory(target);

        if (!string.IsNullOrEmpty(target.Events.Give))
        {
            UserData.RegisterType<MobScripts>();

            var script = new Script();
            var obj = UserData.Create(Services.Instance.MobScripts);
            script.Globals.Set("obj", obj);
            UserData.RegisterProxyType<MyProxy, Room>(r => new MyProxy(room));
            UserData.RegisterProxyType<ProxyPlayer, Player>(r => new ProxyPlayer(player));

            script.Globals["room"] = room;
            script.Globals["player"] = player;
            script.Globals["mob"] = target;

            var res = script.DoString(target.Events.Give);
        }

        if (target.Weight > target.Attributes.Attribute[EffectLocation.Strength] * 3)
        {
            Services.Instance.Writer.WriteLine(
                $"<p>You are now over encumbered by carrying too much weight.</p>",
                target.ConnectionId
            );
        }
    }

    private void GiveGold(Player player, Room room, Player target, int amount)
    {
        if (player.Money.Gold < amount)
        {
            Services.Instance.Writer.WriteLine(
                "<p>You don't have that much gold to give.</p>",
                player.ConnectionId
            );
            return;
        }

        if (target == null)
        {
            Services.Instance.Writer.WriteLine("<p>They aren't here.</p>", player.ConnectionId);
            return;
        }

        Services.Instance.Writer.WriteLine(
            $"<p>You give {target.Name} {(amount == 1 ? "1 gold coin." : $"{amount} gold coins.")}</p>",
            player.ConnectionId
        );
        Services.Instance.Writer.WriteLine(
            $"<p>{player.Name} gives you {(amount == 1 ? "1 gold coin." : $"{amount} gold coins.")}</p>",
            target.ConnectionId
        );
        Services.Instance.Writer.WriteToOthersInRoom(
            $"<p>{player.Name} gives {target.Name} some gold.</p>",
            room,
            player
        );

        player.Money.Gold -= amount;
        player.Weight -= amount * 0.1;
        target.Money.Gold += amount;
        target.Weight += amount * 0.1;
        Services.Instance.UpdateClient.UpdateScore(player);
    }
}
