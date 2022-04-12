using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ArchaicQuestII.DataAccess.DataModels;
using LiteDB;

namespace ArchaicQuestII.DataAccess
{
    public class PlayerDataBase : IPlayerDataBase
    {
        private LiteDatabase _db { get; }
        public PlayerDataBase(LiteDatabase db)
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
            Account,
            Players,
            Log
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
              
                Collections.Account => "Account",
                Collections.Players => "Players",
                Collections.Log => "Log",
                _ => "error",
            };
        }
        /// <summary>
        /// Set index for each collection type
        /// </summary>
        private static void SetIndex<T>(LiteCollection<T> collection, Collections collectionName)
        {

            if (collectionName == Collections.Account)
            {
                (collection as LiteCollection<Account>)?.EnsureIndex(x => x.UserName);
                (collection as LiteCollection<Account>)?.EnsureIndex(x => x.Id);
                (collection as LiteCollection<Account>)?.EnsureIndex(x => x.Characters);
            }

        }
    }
}
