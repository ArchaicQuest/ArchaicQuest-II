using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Info;

public class ListInventoryCmd : ICommand
{
    public ListInventoryCmd(ICore core)
    {
        Aliases = new[] {"inventory", "inv"};
        Description = "";
        Usages = new[] {"Type: inv"};
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
        var inventory = new StringBuilder();
        inventory.Append("<p>You are carrying:</p>");

        if (player.Inventory.Where(x => x.Equipped == false).ToList().Count > 0)
        {
            inventory.Append("<ul>");
            var inv = new ItemList();
            
            inv.AddRange(player.Inventory.Where(x => x.Equipped == false).ToList());

            foreach (var item in inv.List(false))
            {
                if (player.Affects.Blind)
                {
                    inventory.Append("<li>Something</li>");
                }
                else
                {
                    inventory.Append($"<li>{item.Name}</li>");
                }
            }

            inventory.Append("</ul>");
        }
        else
        {
            inventory.Append("<p>Nothing.</p>");
        }
        
        Core.Writer.WriteLine(inventory.ToString(), player.ConnectionId);
    }
}