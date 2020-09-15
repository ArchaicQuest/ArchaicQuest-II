using System;
using System.Collections.Generic;
using System.Text;

namespace ArchaicQuestII.GameLogic.Character.Emote
{
   public class Emote
    {
        /// <summary>
        /// Can this emote be aimed at someone
        /// </summary>
        public bool CanTarget { get; set; }
        /// <summary>
        /// What the player sees when they emote
        /// </summary>
        public string ToSender { get; set; }
        /// <summary>
        /// What the player sees when they emote @ someone
        /// </summary>
        public string ToSenderAtTarget { get; set; }
        /// <summary>
        /// What the room sees when a player emotes
        /// </summary>
        public string ToRoomTarget { get; set; }
        /// <summary>
        /// What the target sees if a player emotes @ them.
        /// </summary>
        public string ToTarget { get; set; }
        /// <summary>
        /// What the room sees if a player emotes @ someone
        /// </summary>
        public string ToRoom { get; set; }
        
    }
}
