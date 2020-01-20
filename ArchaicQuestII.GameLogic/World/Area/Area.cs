using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.World.Area;

namespace ArchaicQuestII.GameLogic.World.Area
{
    public class Area: IArea
    {
        private IDataBase _db { get; }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? DateCreated { get; set; } = DateTime.Now;
        public DateTime? DateUpdated { get; set; }
        public string CreatedBy { get; set; } = "Malleus";
        public string[] ModifiedBy { get; set; }
        public List<Room.Room> Rooms { get; set; }

        public Area(IDataBase db)
        {
            _db = db;
        }

        public void UpdateArea(Area area)
        {
            var data = _db.GetById<Area>(area.Id, "Area");

            data.Description = area.Description;
            data.DateUpdated = DateTime.Now;
            data.Title = area.Title;

            _db.Save(data, "Area");
        }

        public Area GetArea(int id)
        {
            var area = _db.GetById<Area>(Id, "Area");
            var rooms = _db.GetCollection<Room.Room>("Room").Find(x => x.AreaId == id).ToList();

            area.Rooms = rooms;

            return area;
        }
    }
}
