using System;
using System.Collections.Generic;
using System.Text;
using LiteDB;

namespace ArchaicQuestII.DataAccess
{
    public interface IDataBase
    {
        bool Save<T>(T data, string collectionName);
        List<T> GetList<T>(string collectionName);
        LiteCollection<T> GetCollection<T>(string collectionName);
        T GetById<T>(int id, string collectionName);
    }
}
