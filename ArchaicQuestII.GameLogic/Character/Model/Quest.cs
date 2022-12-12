﻿using System;
using System.Collections.Generic;

namespace ArchaicQuestII.GameLogic.Character.Model
{

    public enum QuestTypes
    {
        Kill,
        Fetch,
        Discover,
        Escort,
        Rescue,
        Misc
    }

    public class KillQuest
    {
        public string Name { get; set; }
        public int? Current { get; set; } = 0;
        public int Count { get; set; }
    }

    public class FetchQuest
    {
        public string Name { get; set; }
        public int? Current { get; set; } = 0;
        public int Count { get; set; }
    }

    public class Quest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public QuestTypes Type { get; set; }
        public string Description { get; set; }
        public string Area { get; set; }
        public List<KillQuest> MobsToKill { get; set; } = new List<KillQuest>();
        public List<KillQuest> ItemsToGet { get; set; } = new List<KillQuest>();
        public int ExpGain { get; set; }
        public int GoldGain { get; set; }
        public List<Item.Item> ItemGain { get; set; }
        public bool Completed { get; set; }
        public bool RewardClaimed { get; set; }
        public DateTime DateUpdated { get; set; } = DateTime.Now;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public int TimesCompleted { get; set; }
    }
}
