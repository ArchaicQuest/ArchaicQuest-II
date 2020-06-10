using System;
using System.Collections.Generic;
using System.Text;
using LiteDB;

namespace ArchaicQuestII.DataAccess
{
    public interface IDataBase
    {
        /// <summary>
        /// Use this to Save or Update
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="collectionName"></param>
        /// <returns>true</returns>
        bool Save<T>(T data, DataBase.Collections collectionName);

        /// <summary>
        /// Use this to get a List of the collection data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionName"></param>
        /// <returns>true</returns>
        List<T> GetList<T>(DataBase.Collections collectionName);

        /// <summary>
        /// Use this to get the LiteDB Collection, allows you to use linq
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionName"></param>
        /// <returns>true</returns>
        LiteCollection<T> GetCollection<T>(DataBase.Collections collectionName);

        /// <summary>
        /// Use this to get item by Id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        T GetById<T>(Guid id, DataBase.Collections collectionName);
        T GetById<T>(int id, DataBase.Collections collectionName);
        /// <summary>
        /// Deletes an item by id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        bool Delete<T>(int id, DataBase.Collections collectionName);


        bool DoesCollectionExist(DataBase.Collections collectionName);
    }
}
