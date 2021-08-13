using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Core
{
    public class Time:ITime
    {
        public int Tick { get; set; }
        public double Hour { get; set; }
        public int Minute { get; set; }
        public int Day { get; set; }
        public int Month { get; set; }

        public int Year { get; set; }

        public List<string> Days { get; set; } = new List<string>
        {
            "The Sun",
            "The Moon",
            "Mars",
            "Mercury",
            "Jupiter",
            "Venus",
            "Saturn"
        };

        public List<string> Months { get; set; } = new List<string>
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

        private IWriteToClient _writeToClient;
        private ICache _cache;
        public Time(IWriteToClient writeToClient, ICache cache)
        {
            _writeToClient = writeToClient;
            _cache = cache;
        }

        //public string UpdateCalendar()
        //{
        //    var startDate = new DateTime(2016, 04, 14);
        //    var dateNow = DateTime.Today;
 

         

        //    var monthsSinceStart = dateNow.Subtract(startDate);
        //    // game year is 2 years for 1 year in real life
        //    var gameYearSinceStart = monthsSinceStart * 2;
        //    var gameDaysSinceStart = 17 * 30 * gameYearSinceStart;
        //    var yearsSinceStart = gameDaysSinceStart / (gameYearSinceStart / 17)
        //}

        public void DisplayTimeOfDayMessage(string TickMessage)
        {

            var players = _cache.GetPlayerCache();
            
            foreach (var pc in players.Values)
            {
                var room = _cache.GetRoom(pc.RoomId);

                if (room.Terrain != Room.TerrainType.Inside && room.Terrain != Room.TerrainType.Underground && !string.IsNullOrEmpty(TickMessage))
                {
                    _writeToClient.WriteLine(TickMessage, pc.ConnectionId);
                }
            }
        }

        public string UpdateTime()
        {
            Minute += 1;
            
            if (Tick > 24)
            {
                Tick = 0;
            }

            if (Minute == 60)
            {
                Minute = 0;
                Hour += 1;
                Tick += 1;

                switch (Tick)
                {
                    case 0:
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

                    case 24:
                        return "The moon is high in the sky.";

                }
                if (Hour > 12)
                {
                    Hour = 1;
                }
            }

     

            return string.Empty;

        }

        public bool IsNightTime()
        {
            return Tick >= 6 && Tick <= 18;
        }

        public string ReturnTime()
        {
            return $"{Hour}:{(Minute < 30 ? "00" : "30")}{(Tick >= 12 ? " PM" : " AM")}";
        }


    }
}
