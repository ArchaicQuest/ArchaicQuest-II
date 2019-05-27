using ArchaicQuestII.Log;
using LiteDB;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ArchaicQuestII.Core.World;
using ArchaicQuestII.Engine.Character.Model;
using ArchaicQuestII.Engine.Item;
using ArchaicQuestII.Engine.World;
using ArchaicQuestII.Engine.World.Area.Model;

namespace ArchaicQuestII.Core.Events
{
    public class DB
    {
        private static Log.Log _logger { get; set; }

        public DB() { 
            _logger = new Log.Log();
        }


        public void SavePlayer(Player player)
        {
            if (player == null)
            {

                _logger.Error("Can't save player as name is null");

                throw new ArgumentNullException(nameof(player));
            }

            try
            {
                using (var db = new LiteDatabase(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyData.db")))
                {
                    var col = db.GetCollection<Player>("Player");

                    col.Upsert(player);

                }

            }
            catch (Exception ex)
            {
                _logger.Error("Error Saving player " + ex.Message);
            }

        }

        public static void SaveRoom(Room room)
        {

            try
            {
                using (var db = new LiteDatabase(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyData.db")))
                {
                    var col = db.GetCollection<Room>("Room");

                    col.Upsert(room);

                }

            }
            catch (Exception ex)
            {
                _logger.Error("Error Saving room " + ex.Message);
            }

        }

        public static void SaveMob(Character mob)
        {

            try
            {
                using (var db = new LiteDatabase(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyData.db")))
                {
                    var col = db.GetCollection<Character>("Mobs");

                    col.Upsert(mob);

                }

            }
            catch (Exception ex)
            {
                _logger.Error("Error Saving mob " + ex.Message);
            }

        }

        public static void SaveItem(Item item)
        {
 
            try
            {
                using (var db = new LiteDatabase(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyData.db")))
                {
                    var col = db.GetCollection<Item>("Item");

                    col.Upsert(item);

                }

            }
            catch (Exception ex)
            {
                _logger.Error("Error Saving item " + ex.Message);
            }

        }

        public static void SaveArea(Area area)
        {

            try
            {
                using (var db = new LiteDatabase(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyData.db")))
                {
                    var col = db.GetCollection<Area>("Area");

                    col.Upsert(area);

                }

            }
            catch (Exception ex)
            {
                _logger.Error("Error Saving area " + ex.Message);
            }

        }


        public static List<Area> GetAreas()
        {

                using (var db = new LiteDatabase(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyData.db")))
                {
                    var col = db.GetCollection<Area>("Area");

                  var data = col.FindAll().ToList();

                    return data;

                }
        }

        public static List<Character> GetMobs()
        {
            using (var db = new LiteDatabase(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyData.db")))
            {

                var col = db.GetCollection<Character>("Mobs");

                var mobs = col.FindAll().ToList();

                return mobs;

            }

        }


        public static Character GetMob(string id)
        {
            using (var db = new LiteDatabase(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyData.db")))
            {

                var col = db.GetCollection<Character>("Mobs");

                var mob = col.FindById(id);

                return mob;

            }

        }

        public static List<Item> GetItems()
        {
            using (var db = new LiteDatabase(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyData.db")))
            {

                var col = db.GetCollection<Item>("Item");

                var returnPlayer = col.FindAll().ToList();

                return returnPlayer;

            }

        }

    }
}
