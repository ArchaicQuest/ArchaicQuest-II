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
            sb.Append("<table class='data'><tr><td style='width: 30px; text-align: center;'>#</td><td>Name</td><td>Price</td></tr>");

            int i = 0;
            foreach (var item in mob.Inventory.Distinct().OrderBy(x => x.Name))
            {
                i++;
                sb.Append($"<tr><td style='width: 30px; text-align: center;'>{i}</td><td>{item.Name}</td><td>{AddMarkUp(item.Value)} GP</td></tr>");
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

        public int AddMarkUp(int price)
        {
            return (int)Math.Floor(price * 1.5);
        }
    }
}
