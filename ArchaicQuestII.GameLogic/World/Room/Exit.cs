using System;

namespace ArchaicQuestII.GameLogic.World.Room
{
    public class Coordinates
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
    }

    public class Exit
    {
        public Coordinates Coords { get; set; }
        public int AreaId { get; set; }
        public int RoomId { get; set; }
        public string Name { get; set; }
        public string Keyword { get; set; }
        public bool Door { get; set; } = true;
        public bool Closed { get; set; } = true;
        public bool Locked { get; set; }
        public bool PickProof { get; set; }
        public bool NoPass { get; set; }
        public bool NoScan { get; set; }
        public bool Hidden { get; set; }
        public Guid? LockId { get; set; }
    }

    public class ExitDirections
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
