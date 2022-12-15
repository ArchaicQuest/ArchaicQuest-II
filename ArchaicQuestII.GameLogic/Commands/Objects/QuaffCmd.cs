using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Objects;

public class QuaffCmd : ICommand
{
    public QuaffCmd(ICore core)
    {
        Aliases = new[] {"quaff"};
        Description = "Tries inhale an elixir.";
        Usages = new[] {"Type: quaff potion"};
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
        var target = input.ElementAtOrDefault(1); 
        
        if (string.IsNullOrEmpty(target))
        {
            Core.Writer.WriteLine("<p>Quaff what?</p>", player.ConnectionId);
            return;
        }
        
        var nthItem = Helpers.findNth(target);
        var foundItem =
            Helpers.findRoomObject(nthItem, room) ?? Helpers.findObjectInInventory(nthItem, player);
            
        if (foundItem == null)
        {
            Core.Writer.WriteLine("<p>You can't find that potion.</p>", player.ConnectionId);
            return;
        }

        player.Inventory.Remove(foundItem);
        Core.UpdateClient.UpdateInventory(player);
            
        Core.UpdateClient.PlaySound("quaff", player);

        if (string.IsNullOrEmpty(foundItem.SpellName) && foundItem.ItemType == Item.Item.ItemTypes.Potion)
        {
            Core.Writer.WriteLine("<p>You quaff the potion but nothing happens.</p>", player.ConnectionId);
            return;
        }
   
        foreach (var pc in room.Players.Where(pc => pc.Name != player.Name))
        {
            Core.Writer.WriteLine($"{player.Name} quaffs {foundItem.Name.ToLower()}.", pc.ConnectionId);
        }

        //potion to cast at level of potion and not the player level
        var dummyPlayer = new Player()
        {
            Level = foundItem.Level
        };

        //TODO: Fix this
        //_castSpell.CastSpell(foundItem.SpellName, string.Empty, player, foundItem.SpellName, dummyPlayer, room, false);

    }
}