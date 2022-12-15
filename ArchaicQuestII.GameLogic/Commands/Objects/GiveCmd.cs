using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;
using MoonSharp.Interpreter;

namespace ArchaicQuestII.GameLogic.Commands.Objects;

public class GiveCmd : ICommand
{
    public GiveCmd(ICore core)
    {
        Aliases = new[] {"give"};
        Description = "You can give items and gold to players or mobs.";
        Usages = new[] {"Type: give 'target' 'object'", "Example: give timmy apple", "Example: give timmy 10 gold"};
        UserRole = UserRole.Player;
        Core = core;
    }
    
    public string[] Aliases { get; }
    public string Description { get; }
    public string[] Usages { get; }
    public UserRole UserRole { get; }
    public ICore Core { get; }

    public void Execute(Player player, Room room, string[] input)
    {
        var targetName = input.ElementAtOrDefault(1);
        
        if (string.IsNullOrEmpty(targetName))
        {
            Core.Writer.WriteLine("<p>Give what to whom?</p>", player.ConnectionId);
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
            itemName = input.ElementAtOrDefault(2);
            itemAmount = "1";
        }

        if (player.Affects.Blind)
        {
            Core.Writer.WriteLine("<p>You are blind and can't see a thing!</p>", player.ConnectionId);
            return;
        }

        if (string.IsNullOrEmpty(itemName))
        {
            Core.Writer.WriteLine("<p>Give what to whom?</p>", player.ConnectionId);
            return;
        }

        var nthItem = Helpers.findNth(itemName);
        var nthTarget = Helpers.findNth(targetName);
        var target = Helpers.FindMob(nthTarget, room) ?? Helpers.findPlayerObject(nthTarget, room);
        
        if (target == null)
        {
            Core.Writer.WriteLine("<p>They aren't here.</p>", player.ConnectionId);
            return;
        }

        if (itemName == "gold" && int.TryParse(itemAmount, out var amount))
        {
            GiveGold(player, room, target, amount);
            return;
        }

        var item = Helpers.findObjectInInventory(nthItem, player);

        if (item == null)
        {
            Core.Writer.WriteLine("<p>You do not have that item.</p>", player.ConnectionId);
            return;
        }

        if (item.Equipped)
        {
            Core.Writer.WriteLine($"<p>You must remove {item.Name.ToLower()} before you can give it.</p>",
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
        player.Weight -= item.Weight;

        foreach (var pc in room.Players)
        {
            if (pc.Name == player.Name)
            {
                Core.Writer.WriteLine($"<p>You give {item.Name.ToLower()} to {target.Name.ToLower()}.</p>", pc.ConnectionId);
                continue;
            }

            if (pc.Name == target.Name)
            {
                continue;
            }

            Core.Writer.WriteLine($"<p>{player.Name} gives {item.Name.ToLower()} to {target.Name.ToLower()}.</p>",
                pc.ConnectionId);
        }

        target.Inventory.Add(item);
        Core.Writer.WriteLine($"<p>{player.Name} gives you {item.Name.ToLower()}.</p>", target.ConnectionId);
        Core.UpdateClient.UpdateInventory(player);
        Core.UpdateClient.UpdateInventory(target);
        

        if (!string.IsNullOrEmpty(target.Events.Give))
        {
            UserData.RegisterType<MobScripts>();

            var script = new Script();
            var obj = UserData.Create(Core.MobScripts);
            script.Globals.Set("obj", obj);
            UserData.RegisterProxyType<MyProxy, Room>(r => new MyProxy(room));
            UserData.RegisterProxyType<ProxyPlayer, Player>(r => new ProxyPlayer(player));

            script.Globals["room"] = room;
            script.Globals["player"] = player;
            script.Globals["mob"] = target;

            var res = script.DoString(target.Events.Give);
        }

        // TODO: You are over encumbered 
    }

    private void GiveGold(Player player, Room room, Player target, int amount)
    {

        if (player.Money.Gold < amount)
        {
            Core.Writer.WriteLine("<p>You don't have that much gold to give.</p>", player.ConnectionId);
            return;
        }

        if (target == null)
        {
            Core.Writer.WriteLine("<p>They aren't here.</p>", player.ConnectionId);
            return;
        }

        Core.Writer.WriteLine(
            $"<p>You give {target.Name} {(amount == 1 ? "1 gold coin." : $"{amount} gold coins.")}</p>",
            player.ConnectionId);

        player.Money.Gold -= amount;
        player.Weight -= amount * 0.1;
        target.Money.Gold += amount;
        target.Weight += amount * 0.1;
        Core.UpdateClient.UpdateScore(player);

        foreach (var pc in room.Players.Where(pc => pc.Name != player.Name))
        {
            if (pc.Name == target.Name)
            {
                Core.Writer.WriteLine(
                    $"<p>{player.Name} gives you {(amount == 1 ? "1 gold coin." : $"{amount} gold coins.")}</p>",
                    pc.ConnectionId);
                continue;
            }

            Core.Writer.WriteLine($"<p>{player.Name} gives {target.Name} some gold.</p>",
                pc.ConnectionId);
        }
    }
}