using System;
using System.Collections.Generic;
using System.Text;

namespace ArchaicQuestII.DataAccess
{
    public interface IDataBase
    {
        bool Save<T>(T data, string collectionName);
        List<T> GetCollection<T>(string collectionName);
        T GetById<T>(string id, string collectionName);
    }
}
