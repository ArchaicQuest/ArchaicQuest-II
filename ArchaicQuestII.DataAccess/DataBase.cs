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
            Account,
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
            _db.GetCollection<T>(GetCollectionName(collectionName)).Upsert(data);

            return true;
        }

        public List<T> GetList<T>(Collections collectionName)
        {

            return _db.GetCollection<T>(GetCollectionName(collectionName)).FindAll().ToList();
        }

        public LiteCollection<T> GetCollection<T>(Collections collectionName)
        {
            return _db.GetCollection<T>(GetCollectionName(collectionName));
        }

        public T GetById<T>(Guid id, Collections collectionName)
        {
            return _db.GetCollection<T>(GetCollectionName(collectionName)).FindById(id);
        }

        public T GetById<T>(int id, Collections collectionName)
        {
            return _db.GetCollection<T>(GetCollectionName(collectionName)).FindById(id);
        }

        private static string GetCollectionName(Collections collectionName)
        {
            switch (collectionName)
            {
                case Collections.Alignment:
                    return "Alignment";
                case Collections.Account:
                    return "Account";
                case Collections.Area:
                    return "Area";
                case Collections.AttackType:
                    return "AttackType";
                case Collections.Class:
                    return "Class";
                case Collections.Items:
                    return "Items";
                case Collections.Mobs:
                    return "Mobs";
                case Collections.Players:
                    return "Players";
                case Collections.Race:
                    return "Race";
                case Collections.Room:
                    return "Room";
                case Collections.Status:
                    return "Status";
                default:
                    return "error";

            }
        }
    }
}
