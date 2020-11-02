using System;
using System.Collections.Generic;
using System.Text;

namespace ArchaicQuestII.GameLogic.Core
{
    public class Time:ITime
    {
        public int Tick { get; set; }
        public double Hour { get; set; }
        public int Minute { get; set; }

        private IWriteToClient _writeToClient;
        public Time(IWriteToClient writeToClient)
        {
            _writeToClient = writeToClient;
        }

        public void UpdateTime()
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
                        _writeToClient.WriteLine("The moon is slowly moving west across the sky.");
                        break;
                    case 4:
                        _writeToClient.WriteLine("The moon slowly sets in the west.");
                        break;
                    case 6:
                        _writeToClient.WriteLine("The sun slowly rises from the east.");
                        break;
                    case 8:
                        _writeToClient.WriteLine("The sun has risen from the east, the day has begun.");
                        break;
                    case 9:
                    case 10:
                    case 11:
                        _writeToClient.WriteLine("The sun is slowly moving west across the sky.");
                        break;
                    case 12:
                        _writeToClient.WriteLine("The sun is high in the sky.");
                        break;
                    case 13:
                    case 14:
                    case 15:
                    case 16:
                    case 17:
                        _writeToClient.WriteLine("The sun is slowly moving west across the sky.");
                        break;
                    case 18:
                        _writeToClient.WriteLine("The sun slowly sets in the west.");
                        break;
                    case 19:
                        _writeToClient.WriteLine("The moon slowly rises in the east.");
                        break;
                    case 20:
                        _writeToClient.WriteLine("The moon has risen from the east, the night has begun.");
                        break;
                    case 21:
                    case 22:
                    case 23:
                        _writeToClient.WriteLine("The moon is slowly moving west across the sky.");
                        break;
                    case 24:
                        _writeToClient.WriteLine("The moon is high in the sky.");
                        break;
                }

                if (Hour > 12)
                {
                    Hour = 1;
                }
            }
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
