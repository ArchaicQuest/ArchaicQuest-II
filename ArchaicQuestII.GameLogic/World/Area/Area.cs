using System;
using System.Collections.Generic;

namespace ArchaicQuestII.GameLogic.World.Area
{
    public class Area
    { 
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? DateCreated { get; set; } = DateTime.Now;
        public DateTime? DateUpdated { get; set; }
        public string CreatedBy { get; set; } = "Malleus";
        public string[] ModifiedBy { get; set; }
        public List<Room.Room> Rooms { get; set; }


    }
}
