using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Commands.Objects;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Crafting
{
    public class Crafting: ICrafting
    {
        private IWriteToClient _writeToClient;
        private ICache _cache;
        private IDice _dice;
        private IUpdateClientUI _clientUi; 
        public Crafting(IWriteToClient writeToClient, ICache cache, IDice dice, IUpdateClientUI clientUi)
        {
            _writeToClient = writeToClient;
            _cache = cache;
            _dice = dice;
            _clientUi = clientUi;
            
        }
        public void ListCrafts(Player player)
        {
            if (player.Status != CharacterStatus.Status.Standing)
            {
                _writeToClient.WriteLine($"<p>You can't do that while {Enum.GetName(typeof(CharacterStatus.Status), player.Status).ToLower()}.</p>", player.ConnectionId);
                return;
            }

            var materials = player.Inventory.Where(x => x.ItemType == Item.Item.ItemTypes.Material).ToList();

            if (materials.Count == 0)
            {
                _writeToClient.WriteLine("<p>You don't have any materials to craft a thing.</p>", player.ConnectionId);
                return;
            }

            // Lets find what you can craft
            var craftingRecipes = _cache.GetCraftingRecipes();

            if (craftingRecipes == null)
            {
                _writeToClient.WriteLine("<p>No crafting recipes have been set up.</p>", player.ConnectionId);
                return;
            }

            var craftingList = ReturnValidRecipes(player);

            if (craftingList.Count == 0)
            {
                _writeToClient.WriteLine("<p>No crafting recipes found with the current materials you have.</p>", player.ConnectionId);
                return;
            }

         
            var sb = new StringBuilder();
            sb.Append("<p>You can craft the following items:</p>");
            sb.Append("<table class='simple'>");
            sb.Append($"<tr><td>Name</td><td>Materials</td></tr>");
            foreach (var craft in craftingList.Distinct())
            {
                var materialsRequired = "";
                foreach (var material in craft.CraftingMaterials)
                {
                    materialsRequired += $"{material.Material} x{material.Quantity}, ";
                }

                sb.Append($"<tr><td>{craft.Title}</td><td>{materialsRequired}</td></tr>");
            }

            sb.Append($"</table>");

            _writeToClient.WriteLine(sb.ToString(), player.ConnectionId);

        }

        public void CraftingManager(Player player, Room room, string keyword)
        {

            keyword = keyword.Substring(keyword.IndexOf(' ')).TrimStart();

            if (!string.IsNullOrEmpty(keyword))
            {
                if (keyword.StartsWith("list"))
                {  
                    ListCrafts(player);
                    return;
                }

                CraftItem(player, room, keyword);
                return;
            }

           
        }

        public void CraftItem(Player player, Room room, string item)
        {
            var craftingRecipes = ReturnValidRecipes(player);

            if (room.Items.FirstOrDefault(x => x.ItemType == Item.Item.ItemTypes.Crafting) == null)
            {
                _writeToClient.WriteLine("<p>To begin crafting you require the correct tools such as a crafting bench.</p>", player.ConnectionId);
                return;
            }

            var recipe =
                craftingRecipes.FirstOrDefault(x =>
                    x.Title.StartsWith(item, StringComparison.CurrentCultureIgnoreCase));

            if (recipe == null) {
                _writeToClient.WriteLine("<p>You can't craft that.</p>", player.ConnectionId);
                return;
            }
            _writeToClient.WriteLine($"<p>You begin crafting {recipe.Title}.</p>", player.ConnectionId);
            // use up materials
            foreach (var material in recipe.CraftingMaterials)
            {
               var craftItem = player.Inventory.FirstOrDefault(x => x.Name.Equals(material.Material, StringComparison.CurrentCultureIgnoreCase));

               var limit = 0;
               for (var i = player.Inventory.Count - 1; i >= 0; i--)
               {
                   if (player.Inventory[i].Name == craftItem.Name && limit <= material.Quantity)
                   {
                       limit++;
                       player.Weight -= craftItem.Weight;
                        player.Inventory.RemoveAt(i);
                   }
               }


            }

            _clientUi.UpdateScore(player);
            _clientUi.UpdateInventory(player);

            var roll = _dice.Roll(1, 1, 100);

            if (roll > 50)
            {
                player.Inventory.Add(recipe.CreatedItem);
                player.Weight += recipe.CreatedItem.Weight;
                _writeToClient.WriteLine($"<p>You slave over the crafting bench working away.</p>", player.ConnectionId, 2000);
                _writeToClient.WriteLine($"<p class='improve'>You have crafted successfully {recipe.Title}.</p>", player.ConnectionId, 4000);
            }
            else
            {
               
                _writeToClient.WriteLine($"<p>You slave over the crafting bench working away.</p>", player.ConnectionId, 2000);
                _writeToClient.WriteLine($"<p>You have failed to craft {recipe.Title}. It looks nothing like!</p>", player.ConnectionId, 4000);
            }
            

          

        }

        public List<CraftingRecipes> ReturnValidRecipes(Player player)
        {
            var craftingRecipes = _cache.GetCraftingRecipes();
            var materials = player.Inventory.Where(x => x.ItemType == Item.Item.ItemTypes.Material).GroupBy(y => y.Name)
                .Select(z => z.First());
            var craftingList = new List<CraftingRecipes>();
            foreach (var material in materials)
            {
                var quantity = player.Inventory.Where(x => x.ItemType == Item.Item.ItemTypes.Material && x.Name.Equals(material.Name, StringComparison.CurrentCultureIgnoreCase)).ToList();
                var canCraft = craftingRecipes.Where(x =>
                    x.CraftingMaterials.Any(y => y.Material.Equals(material.Name, StringComparison.CurrentCultureIgnoreCase) && y.Quantity <= quantity.Count));

                craftingList.AddRange(canCraft);
            }

            return craftingList;
        }
    }
}
