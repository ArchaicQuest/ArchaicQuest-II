using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Item;

namespace ArchaicQuestII.GameLogic.Commands.Inventory
{

    public class Inventory : IInventory
    {
        private readonly IWriteToClient _writer;
        public Inventory(IWriteToClient writer)
        {
            _writer = writer;
        }
        public void List(Player player)
        {
            var inventory = new StringBuilder();
            inventory.Append("<p>You are carrying:</p>");

            if (player.Inventory.Where(x => x.Equipped == false).ToList().Count > 0)
            {
                inventory.Append("<ul>");
                var inv = new ItemList();

                foreach (var item in player.Inventory.Where(x => x.Equipped == false).ToList())
                {
                    inv.Add(item);
                }

                foreach (var item in inv.List(false))
                {
                    if (player.Affects.Blind)
                    {
                        inventory.Append($"<li>Something</li>");
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


            _writer.WriteLine(inventory.ToString(), player.ConnectionId);
        }
    }
}
