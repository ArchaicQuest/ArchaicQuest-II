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
             _db.GetCollection<T>(collectionName).Upsert(data);

            return true;
        }

        public List<T> GetList<T>(string collectionName)
        {
            return _db.GetCollection<T>(collectionName).FindAll().ToList();
        }

        public LiteCollection<T> GetCollection<T>(string collectionName)
        {
            return _db.GetCollection<T>(collectionName);
        }

        public T GetById<T>(int id, string collectionName)
        {
          return  _db.GetCollection<T>(collectionName).FindById(id);
        }
    }
}
