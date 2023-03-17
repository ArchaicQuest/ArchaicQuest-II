using System;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Info;

public class ListShopInventoryCmd : ICommand
{
    public ListShopInventoryCmd(ICore core)
    {
        Aliases = new[] {"list", "li",};
        Description = "Displays items that are for sale at mob stores.";
        Usages = new[] {"Type: list to view a list of items that can be purchased."};
            Title = "Shop - List Inventory";
    DeniedStatus = null;
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
        var shopKeeper = FindShopKeeper(room);
        if (shopKeeper == null)
        {
            Core.Writer.WriteLine("<p>There is no one selling here.</p>", player.ConnectionId);
            return;
        }

        if (!shopKeeper.Inventory.Any())
        {
            if (shopKeeper.SpellList.Any())
            {
                Core.Writer.WriteLine("<p>They have nothing for sale but do offer spells. Try 'heal'.</p>", player.ConnectionId);
                return;
            }

            Core.Writer.WriteLine("<p>They have nothing for sale.</p>", player.ConnectionId);
            return;
        }

        DisplayInventory(shopKeeper, player);
    }

    private void DisplayInventory(Player mob, Player player)
        {

            var hagglePriceReduction = Haggle(player, mob);

            Core.Writer.WriteLine(mob.Name + " says 'Here's what I have for sale.'", player.ConnectionId);
            var sb = new StringBuilder();
            sb.Append("<table class='data'><tr><td style='width: 30px; text-align: center;'>#</td><td style='width: 30px; text-align: center;'>Level</td><td  style='width: 65px;'>Price</td><td>Item</td></tr>");

            int i = 0;
            foreach (var item in mob.Inventory.Distinct().OrderBy(x => x.Level).ThenBy(x => x.Value))
            {
                i++;
                sb.Append($"<tr><td style='width: 30px; text-align: center;'>{i}</td><td style='width: 30px; text-align: center;'>{item.Level}</td><td  style='width: 65px;'>{DisplayUnit(item.Value, hagglePriceReduction)}</td><td>{item.Name}</td></tr>");
            }

            sb.Append("</table>");
            Core.Writer.WriteLine(sb.ToString(), player.ConnectionId);

        }

          private Player FindShopKeeper(Room room)
        {
            foreach (var mob in room.Mobs)
            {
                if (mob.Shopkeeper)
                {
                    return mob;
                }
            }

            return null;
        }

        private int Haggle(Player player, Player target)
        {
            var priceReduction = Core.PassiveSkills.Haggle(player, target);

            return priceReduction;

        }

        private int AddMarkUp(int price)
        {
            return (int)Math.Floor(price * 1.5);
        }



        /// <summary>
        /// Each gold piece is worth 100 silver pieces.
        /// 100 silver = 1 gold
        /// 1000 silver = 10 gold
        /// 10,000 silver = 100 gold
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        private string DisplayUnit(int price, int hagglePriceReduction)
        {
            var goldPrice = AddMarkUp(price); // /100

            // show haggle price
            return $"{Math.Floor((decimal)goldPrice - Helpers.GetPercentage(hagglePriceReduction, (int)goldPrice))} GP";
            // return goldPrice < 1 ? $"{Math.Floor(goldPrice * 100)} SP" : $"{Math.Floor(goldPrice)} GP";
        }

}