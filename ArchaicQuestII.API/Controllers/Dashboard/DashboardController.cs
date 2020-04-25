using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Item;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using ArchaicQuestII.GameLogic.Character.Equipment;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.World.Area;
using ArchaicQuestII.GameLogic.World.Room;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ArchaicQuestII.Controllers.Dashboard
{
    public class QuickStats
    {
        public int ItemCount { get; set; }
        public int MobCount { get; set; }
        public int AreaCount { get; set; }
        public int RoomCount { get; set; }
        public int QuestCount { get; set; }
    }

    public class DashboardController : Controller
    {
        private IDataBase _db { get; }
        public DashboardController(IDataBase db)
        {
            _db = db;
        }

        [HttpGet]
        [Route("api/dashboard/quickStats")]
        public JsonResult get()
        {
            var stats = new QuickStats
            {
                ItemCount = _db.GetCollection<Item>(DataBase.Collections.Items).Count(),
                MobCount = _db.GetCollection<Character>(DataBase.Collections.Mobs).Count(),
                AreaCount = _db.GetCollection<Area>(DataBase.Collections.Area).Count(),
                RoomCount = _db.GetCollection<Room>(DataBase.Collections.Room).Count(),
                QuestCount = 0
            };

            return Json(stats);

        }




    }
}
