using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Skill.Enum;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Core
{
    /// <summary>
    /// Random functions to go here unless there is
    /// enough related random functions to move out to their own class
    /// </summary>
   public static class Helpers
    {
        /// <summary>
        /// Her / His
        /// </summary>
        /// <param name="gender"></param>
        /// <returns></returns>
        public static string GetPronoun(string gender)
        {
            return gender switch
            {
                "Female" => "her",
                "Male" => "his",
                _ => "their",
            };
        }
        /// <summary>
        /// She / He
        /// </summary>
        /// <param name="gender"></param>
        /// <returns></returns>
        public static string GetSubjectPronoun(string gender)
        {
            return gender switch
            {
                "Female" => "she",
                "Male" => "he",
                _ => "it",
            };
        }
        /// <summary>
        /// Her / Him
        /// </summary>
        /// <param name="gender"></param>
        /// <returns></returns>
        public static string GetObjectPronoun(string gender)
        {
            return gender switch
            {
                "Female" => "her",
                "Male" => "him",
                _ => "it",
            };
        }

        public static Player FindPlayer(string target, Room room)
        {
            return room.Players.FirstOrDefault(x => x.Name.StartsWith(target, StringComparison.CurrentCultureIgnoreCase));
        }

        public static Player FindMob(string target, Room room)
        {
            return (Player)room.Mobs.FirstOrDefault(x => x.Name.StartsWith(target, StringComparison.CurrentCultureIgnoreCase));
        }

        public static Item.Item FindItemInInventory(string target, Player player)
        {
            return player.Inventory.FirstOrDefault(x => x.Name.StartsWith(target, StringComparison.CurrentCultureIgnoreCase));
        }

        public static Item.Item FindItemInRoom(string target, Room room)
        {
            return room.Items.FirstOrDefault(x => x.Name.StartsWith(target, StringComparison.CurrentCultureIgnoreCase));
        }

       

    }


}
