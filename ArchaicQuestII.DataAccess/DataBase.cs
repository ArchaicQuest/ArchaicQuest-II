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
            Items,
            Mobs,
            Players,
            Race,
            Room,
            Status
        }

        public bool Save<T>(T data, Collections collectionName)
        { 
             _db.GetCollection<T>(nameof(collectionName)).Upsert(data);

            return true;
        }

        public List<T> GetList<T>(Collections collectionName)
        {
            return _db.GetCollection<T>(nameof(collectionName)).FindAll().ToList();
        }

        public LiteCollection<T> GetCollection<T>(Collections collectionName)
        {
            return _db.GetCollection<T>(nameof(collectionName));
        }

        public T GetById<T>(int id, Collections collectionName)
        {
          return  _db.GetCollection<T>(nameof(collectionName)).FindById(id);
        }
    }
}
