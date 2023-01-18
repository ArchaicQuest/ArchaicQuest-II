using System;
using System.Collections.Generic;

namespace ArchaicQuestII.GameLogic.World
{
    public class Time
    {
        public int SecondsPerMudHour { get; set; } = 60;
        public int SecondsPerMudDay { get; set; }
        public int SecondsPerMudMonth { get; set; }
        public int SecondsPerMudYear { get; set; }
        public int SecondsPerRealMinute { get; set; } = 60;
        public int SecondsPerRealHour { get; set; }
        public int SecondsPerRealDay { get; set; }
        public int SecondsPerRealYear { get; set; }
        public MudTime GameTime { get; set; }
        
        public class MudTime
        {
            public double Hours { get; set; }
            public double Day { get; set; }
            public double Month { get; set; }
            public double Year { get; set; }
            public DateTime RLDateTime { get; set; }
        }
        
        public List<string> Days { get; set; } = new()
        {
            "The Sun",
            "The Moon",
            "Mars",
            "Mercury",
            "Jupiter",
            "Venus",
            "Saturn"
        };

        public List<string> Months { get; set; } = new()
        {
            "Winter",
            "The Winter Wolf",
            "The Frozen Forests",
            "The Crystal Tundra",
            "Rebirth",
            "The Spring",
            "Nature",
            "Growth",
            "The Dragon",
            "The Sun",
            "The Heat",
            "The Battle",
            "The Dark Shades",
            "The Shadows",
            "The Long Shadows",
            "The Ancient Darkness",
            "The Great Evil",
        };

        public Time()
        {
            /*
             *  Real life seconds in one mud day.
             *  1,440 seconds = 24 real life minutes.
             */
            SecondsPerMudDay = 24 * SecondsPerMudHour;
            /*
             *  Real life seconds per mud month
             *  43,200 seconds = 12 real life hours
             */
            SecondsPerMudMonth = 30 * SecondsPerMudDay;
            /*
             *  Real life seconds per mud year
             *  734,400 seconds = 8.5 real life days
             */
            SecondsPerMudYear = 17 * SecondsPerMudMonth;

            SecondsPerRealHour = 60 * SecondsPerRealMinute;
            SecondsPerRealDay = 24 * SecondsPerRealHour;
            SecondsPerRealYear = 365 * SecondsPerRealDay;

            var dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

            GameTime = MudTimePassed(dt, new DateTime(2016, 04, 14));
        }

        public MudTime MudTimePassed(DateTime currentTime, DateTime pastTime)
        {

            var mudTime = new MudTime();
            var seconds = (currentTime - pastTime).TotalSeconds;

            mudTime.Hours = (seconds / SecondsPerMudHour) % 24;
            seconds -= SecondsPerMudHour * mudTime.Hours;

            mudTime.Day = (seconds / SecondsPerMudDay) % 30;
            seconds -= SecondsPerMudDay * mudTime.Day;

            mudTime.Month = (seconds / SecondsPerMudMonth) % 17;
            seconds -= SecondsPerMudMonth * mudTime.Month;

            mudTime.Year = (seconds / SecondsPerMudYear);
            mudTime.RLDateTime = currentTime;


            GameTime = mudTime;
            return mudTime;
        }
        
        public string GetDay(int day)
        {
            // 100% a smarter way to do this
            var TheMoon = new List<int> { 0, 7, 14, 21, 28, 35 };
            var Mars = new List<int> { 1, 8, 15, 22, 29 };
            var Mercury = new List<int> { 2, 9, 16, 23, 30 };
            var Jupiter = new List<int> { 3, 10, 17, 24, 31 };
            var Venus = new List<int> { 4, 11, 18, 25, 32 };
            var Saturn = new List<int> { 5, 12, 19, 26, 33 };
            var TheSun = new List<int> { 6, 13, 20, 27, 34 };

            var nameOfDay = TheMoon.Contains(day) ? "The Moon" :
                Mars.Contains(day) ? "Mars" :
                Mercury.Contains(day) ? "Mercury" :
                Jupiter.Contains(day) ? "Jupiter" :
                Venus.Contains(day) ? "Venus" :
                Saturn.Contains(day) ? "Saturn" : "TheSun";

            return nameOfDay;
        }

        public string FormatDateString()
        {
            var nth = "";

            if (GameTime.Day > 3 && GameTime.Day < 21)
            {
                nth = "th";
            }

            switch (GameTime.Day % 10)
            {
                case 1:
                    nth = "st";
                    break;
                case 2:
                    nth = "nd";
                    break;
                case 3:
                    nth = "rd";
                    break;
                default:
                    nth = "th";
                    break;
            }
            return $"Day of {GetDay((int)GameTime.Day)}, {GameTime.Day}{nth} month of {Months[(int)GameTime.Month]}, year {GameTime.Year}.";
        }

        public string UpdateTime()
        {
            var dt = DateTime.Now;

            MudTimePassed(dt, new DateTime(2016, 04, 14));

            switch (Convert.ToInt32(Math.Floor(GameTime.Hours)))
            {
                case 0:
                    return "The moon is high in the sky.";
                case 1:
                case 2:
                case 3:
                    return "The moon is slowly moving west across the sky.";

                case 4:
                    return "The moon slowly sets in the west.";

                case 6:
                    return "The sun slowly rises from the east.";

                case 8:
                    return "The sun has risen from the east, the day has begun.";

                case 9:
                case 10:
                case 11:
                    return "The sun is slowly moving west across the sky.";

                case 12:
                    return "The sun is high in the sky.";

                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                    return "The sun is slowly moving west across the sky.";

                case 18:
                    return "The sun slowly sets in the west.";

                case 19:
                    return "The moon slowly rises in the east.";

                case 20:
                    return "The moon has risen from the east, the night has begun.";

                case 21:
                case 22:
                case 23:
                    return "The moon is slowly moving west across the sky.";


            }
            return string.Empty;
        }

        public string ReturnDate()
        {
            return FormatDateString();
        }

        public string ReturnTime()
        {

            var hour = Math.Floor(GameTime.Hours) > 12 ? Math.Floor(GameTime.Hours) - 12 : Math.Floor(GameTime.Hours);
            if (Math.Floor(GameTime.Hours) == 0)
            {
                hour = 12;
            }
            return $"{hour}:00 {(Convert.ToInt32(Math.Floor(GameTime.Hours)) >= 12 ? " PM" : " AM")}";
        }
    }
}
