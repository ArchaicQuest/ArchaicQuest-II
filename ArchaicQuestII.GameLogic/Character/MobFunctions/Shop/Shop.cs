using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Character.MobFunctions.Shop
{
    public class Shop: IMobFunctions
    {
        private readonly IWriteToClient _writer;
        private readonly IUpdateClientUI _clientUi;

        public Shop(IWriteToClient writer, IUpdateClientUI clientUi)
        {
            _writer = writer;
            _clientUi = clientUi;
        }

        public void DisplayInventory(Player mob, Player player)
        {
            _writer.WriteLine(mob.Name + " says 'Here's what I have for sale.'", player.ConnectionId);
            var sb = new StringBuilder();
            sb.Append("<table class='data'><tr><td style='width: 30px; text-align: center;'>#</td><td style='width: 30px; text-align: center;'>Level</td><td  style='width: 100px;'>Price</td><td>Item</td></tr>");

            int i = 0;
            foreach (var item in mob.Inventory.Distinct().OrderBy(x => x.Level).ThenBy(x => x.Value))
            {
                i++;
                sb.Append($"<tr><td style='width: 30px; text-align: center;'>{i}</td><td style='width: 30px; text-align: center;'>{item.Level}</td><td  style='width: 100px;'>{DisplayUnit(item.Value)}</td><td>{item.Name}</td></tr>");
            }

            sb.Append("</table>");
            _writer.WriteLine(sb.ToString(), player.ConnectionId);

        }

        public Player FindShopKeeper(Room room)
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

        public void List(Room room, Player player)
        {
            var shopKeeper = FindShopKeeper(room);
            if (shopKeeper == null)
            {
                _writer.WriteLine("<p>There is no one selling here.</p>", player.ConnectionId);
                return;
            }

            DisplayInventory(shopKeeper, player);
        }

        public double AddMarkUp(int price)
        {
            return price * 1.5;
        }

        /// <summary>
        /// Each gold piece is worth 100 silver pieces.
        /// 100 silver = 1 gold
        /// 1000 silver = 10 gold
        /// 10,000 silver = 100 gold
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        public string DisplayUnit(int price)
        {
            var goldPrice = AddMarkUp(price) / 100;
            return goldPrice < 1 ? $"{Math.Floor(goldPrice * 100)} SP" : $"{Math.Floor(goldPrice)} GP";
        }
    }
}
