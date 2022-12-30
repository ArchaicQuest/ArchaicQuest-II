using System;
using System.Collections.Generic;
using System.Text;

namespace ArchaicQuestII.GameLogic.Character.Emote
{
    public class Emote
    {

        /// <summary>
        /// What the player sees when they emote without a target
        /// </summary>
        public string CharNoTarget { get; set; }
        /// <summary>
        ///What the room sees when the player emotes without a target
        /// </summary>
        public string RoomNoTarget { get; set; }
        /// <summary>
        /// What the player sees when they emote at a target
        /// </summary>
        public string TargetFound { get; set; }
        /// <summary>
        /// What the room sees when a player emotes at a target
        /// </summary>
        public string RoomTarget { get; set; }
        /// <summary>
        /// What the target sees if a player emotes @ them.
        /// </summary>
        public string ToTarget { get; set; }
        /// <summary>
        /// What the room sees if a player emotes @ someone
        /// </summary>
        public string TargetSelf { get; set; }
        public string RoomSelf { get; set; }


    }
}
