using System;
using System.Collections.Generic;
using System.Text;

namespace ArchaicQuestII.GameLogic.Core
{
    public class Config
    {
        public int Id { get; set; }
        public bool DoubleXp { get; set; } = false;
        public bool DoubleGains { get; set; } = false;
        public bool DoubleQuestPoints { get; set; } = false;
        public bool PkAllowed { get; set; } = true;
        public bool PlayerThievingAllowed { get; set; } = true;
        /// <summary>
        /// Min level required to use Yell, gossip, channels
        /// </summary>
        public int MinLevelCanShout { get; set; } = 3;
        public string StartingRoom { get; set; } = "1000";
        public int DefaultRecallRoom { get; set; } = 0;

        /// <summary>
        /// Number of tics before NPC corpses decompose
        /// </summary>
        public int MaxNpcCorpseTime { get; set; } = 5;
        /// <summary>
        /// Number of tics before NPC corpses decompose
        /// </summary>
        public int MaxPcCorpseTime { get; set; } = 10;

        /// <summary>
        /// Default idle time before being kicked
        /// default is 5 minutes
        /// </summary>
        public int MaxIdleTime { get; set; } = 300000;
        /// <summary>
        /// Time in ms for player update tick
        /// </summary>
        public int PlayerTick { get; set; } = 500;
        /// <summary>
        /// Time in ms for game update tick
        /// </summary>
        public int UpdateTick { get; set; } = 1000;

        public bool PostToDiscord { get; set; } = false;

        public string ChannelDiscordWebHookURL { get; set; } = "";
        public string EventsDiscordWebHookURL { get; set; } = "";
        public string ErrorDiscordWebHookURL { get; set; } = "";

    }
}