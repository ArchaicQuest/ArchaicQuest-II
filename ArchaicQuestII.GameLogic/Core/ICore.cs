using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Help;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Core
{
 public interface ICore
    {
        /// <summary>
        /// Displays lists of players 
        /// </summary>
        /// <param name="player"></param>
        void Who(Player player);

        void Save(Player player);
        void Quit(Player player, Room room);
        /// <summary>
        /// Where are players within the area
        /// </summary>
        /// <param name="player"></param>
        /// <param name="room"></param>
        void Where(Player player, Room room);
        /// <summary>
        /// Display questlog
        /// </summary>
        /// <param name="player"></param>
        void QuestLog(Player player);
        void Recall(Player player, Room room);
        /// <summary>
        /// Train attributes
        /// </summary>
        /// <param name="player"></param>
        /// <param name="room"></param>
        /// <param name="stat"></param>
        void Train(Player player, Room room, string stat);
        /// <summary>
        /// Eat food
        /// </summary>
        /// <param name="player"></param>
        /// <param name="room"></param>
        /// <param name="obj">name of food to eat</param>
        void Eat(Player player, Room room, string obj);
    }
}
