using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchaicQuestII.GameLogic.Core
{
    /// <summary>
    /// Add secrets here
    /// </summary>
    public class Keys
    {
        /// <summary>
        /// Sends a message to a discord channel when ever a player uses a channel
        /// OOC / newbie / gossip
        /// </summary>
        public  string ChannelDiscordWebHookURL { get; set; }

        /// <summary>
        /// Sends a message to a discord channel when ever a player joins the game, levels up or dies
        /// </summary>
        public string EventsDiscordWebHookURL { get; set; }
    }
}
