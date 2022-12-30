using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LiteDB;

namespace ArchaicQuestII.DataAccess
{
    public class DataBase : IDataBase
    {
        private LiteDatabase _db { get; }
        public DataBase(LiteDatabase db)
        {
            _db = db;
        }

        /// <summary>
        /// DB methods require a collection name
        /// to avoid errors due to typos, collection names
        /// are managed by the collections enum
        /// </summary>
        public enum Collections
        {
            Alignment,
            Area,
            AttackType,
            Class,
            Help,
            Items,
            Mobs,
            Race,
            Room,
            Skill,
            Status,
            Config,
            Socials,
            Quests,
            Users,
            Log,
            CraftingRecipes,
            ErrorLog
        }

        public bool Save<T>(T data, Collections collectionName)
        {
            var collection = _db.GetCollection<T>(GetCollectionName(collectionName));
            collection.Upsert(data);
            SetIndex((LiteCollection<T>)collection, collectionName);
            _db.Checkpoint();

            return true;
        }

        public List<T> GetList<T>(Collections collectionName)
        {
            return _db.GetCollection<T>(GetCollectionName(collectionName)).FindAll().ToList();
        }

        public LiteCollection<T> GetCollection<T>(Collections collectionName)
        {
            return (LiteCollection<T>)_db.GetCollection<T>(GetCollectionName(collectionName));
        }

        public T GetById<T>(Guid id, Collections collectionName)
        {
            return _db.GetCollection<T>(GetCollectionName(collectionName)).FindById(id);
        }

        public T GetById<T>(int id, Collections collectionName)
        {
            return _db.GetCollection<T>(GetCollectionName(collectionName)).FindById(id);
        }

        public bool Delete<T>(int id, Collections collectionName)
        {
            return _db.GetCollection<T>(GetCollectionName(collectionName)).Delete(id);
        }

        public bool DoesCollectionExist(Collections collectionName)
        {
            return _db.CollectionExists(GetCollectionName(collectionName));
        }

        private static string GetCollectionName(Collections collectionName)
        {
            return collectionName switch
            {
                Collections.Alignment => "Alignment",
                Collections.Area => "Area",
                Collections.AttackType => "AttackType",
                Collections.Class => "Class",
                Collections.Help => "Help",
                Collections.Items => "Items",
                Collections.Mobs => "Mobs",
                Collections.Race => "Race",
                Collections.Room => "Room",
                Collections.Skill => "Skill",
                Collections.Status => "Status",
                Collections.Config => "Config",
                Collections.Socials => "Socials",
                Collections.Quests => "Quests",
                Collections.Users => "Users",
                Collections.Log => "Log",
                Collections.ErrorLog => "ErrorLog",
                Collections.CraftingRecipes => "CraftingRecipes",
                _ => "error",
            };
        }
        /// <summary>
        /// Set index for each collection type
        /// </summary>
        private static void SetIndex<T>(LiteCollection<T> collection, Collections collectionName)
        {

            //if (collectionName == Collections.Account)
            //{
            //    (collection as LiteCollection<Account>)?.EnsureIndex(x => x.UserName);
            //    (collection as LiteCollection<Account>)?.EnsureIndex(x => x.Id);
            //    (collection as LiteCollection<Account>)?.EnsureIndex(x => x.Characters);
            //}

        }

        public void ExportDBToJSON()
        {
            var dateT = DateTime.Now.ToString("yyyy-dd-M-HH-mm"); ;
            Directory.CreateDirectory($"backup/{dateT}");
            
            _db.Execute($"select $ into $file('backup/{dateT}/Alignment.json') from Alignment");
            _db.Execute($"select $ into $file('backup/{dateT}/Area.json') from Area");
            _db.Execute($"select $ into $file('backup/{dateT}/AttackType.json') from AttackType");
            _db.Execute($"select $ into $file('backup/{dateT}/Class.json') from Class");
            _db.Execute($"select $ into $file('backup/{dateT}/Help.json') from Help");
            _db.Execute($"select $ into $file('backup/{dateT}/Items.json') from Items");
            _db.Execute($"select $ into $file('backup/{dateT}/Mobs.json') from Mobs");
            _db.Execute($"select $ into $file('backup/{dateT}/Race.json') from Race");
            _db.Execute($"select $ into $file('backup/{dateT}/Room.json') from Room");
            _db.Execute($"select $ into $file('backup/{dateT}/Skill.json') from Skill");
            _db.Execute($"select $ into $file('backup/{dateT}/Status.json') from Status");
            _db.Execute($"select $ into $file('backup/{dateT}/Config.json') from Config");
            _db.Execute($"select $ into $file('backup/{dateT}/Socials.json') from Socials");
            _db.Execute($"select $ into $file('backup/{dateT}/Quests.json') from Quests");
            _db.Execute($"select $ into $file('backup/{dateT}/Users.json') from Users");
            _db.Execute($"select $ into $file('backup/{dateT}/CraftingRecipes.json') from CraftingRecipes");
        }
    }
}
