using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LiteDB;

namespace ArchaicQuestII.DataAccess
{
    public class DataBase : IDataBase
    {
        private LiteDatabase _db { get; set; }
        public DataBase(LiteDatabase db)
        {
            _db = db;
        }
        public bool Save<T>(T data, string collectionName)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

             _db.GetCollection<T>(collectionName).Upsert(data);

            return true;

        }

        public List<T> GetCollection<T>(string collectionName)
        {
            return _db.GetCollection<T>(collectionName).FindAll().ToList();
        }

        public T GetById<T>(string id, string collectionName)
        {
          return  _db.GetCollection<T>(collectionName).FindById(id);
        }
    }
}
