using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.Core.World;
using ArchaicQuestII.Engine.Character.Model;

namespace ArchaicQuestII.Engine.World.Room.Model
{
    public class RoomExits
    {
       
        public Exit North { get; set; }
        public Exit NorthEast { get; set; }
        public Exit East { get; set; } 
        public Exit SouthEast { get; set; }
        public Exit South { get; set; } 
        public Exit SouthWest { get; set; }
        public Exit West { get; set; } 
        public Exit NorthWest { get; set; }
        public Exit Up { get; set; } 
        public Exit Down { get; set; }

    }
}
