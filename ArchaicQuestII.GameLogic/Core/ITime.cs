using System;
using System.Collections.Generic;
using System.Text;

namespace ArchaicQuestII.GameLogic.Core
{
    public interface ITime
    {
        public string UpdateTime();
        public string ReturnTime();
        public bool IsNightTime();
        public void DisplayTimeOfDayMessage(string TickMessage);
        public MudTime MudTimePassed(DateTime currentTime, DateTime pastTime);
        public string GetDay(int day);
        public string FormatDateString();
        public string ReturnDate();
    }
}
