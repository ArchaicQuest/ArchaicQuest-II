using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Crafting
{
    
    public interface ICrafting
    {
        /// <summary>
        /// Displays lists of crafts the player can build
        /// based on materials in their inventory
        /// </summary>
        /// <param name="player">player</param>
        public void ListCrafts(Player player);

        /// <summary>
        /// Redirects to the correct command
        /// </summary>
        /// <param name="player"></param>
        /// <param name="room"></param>
        /// <param name="keyword"></param>
        public void CraftingManager(Player player, Room room, string keyword);

        public void CraftItem(Player player, Room room, string item);

        public List<CraftingRecipes> ReturnValidRecipes(Player player);
    }
}
