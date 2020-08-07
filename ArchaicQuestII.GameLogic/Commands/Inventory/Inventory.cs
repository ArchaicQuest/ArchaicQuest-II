using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;

namespace ArchaicQuestII.GameLogic.Commands.Inventory
{
    
    public class Inventory: IInventory
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

            if (player.Inventory.Count > 0)
            {
                inventory.Append("<ul>");


                foreach (var item in player.Inventory.List())
                {
                    inventory.Append($"<li>{item}</li>");
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
