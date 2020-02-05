using System;
using System.Collections.Generic;
using LiteDB;
using Moq;
using Xunit;

namespace ArchaicQuestII.DataAccess.Tests
{
    public class DataBaseTests
    {
        private readonly Mock<IDataBase> _database;
        private readonly Mock<LiteCollection<string>> _collection;

        public DataBaseTests()
        {
            _database = new Mock<IDataBase>();

        }
 

        [Fact]
        public void Save()
        {
            var player = true;
            //arrange
           _database.Setup((d => d.Save(player,DataBase.Collections.Players))).Returns(true);

            var result = _database.Object.Save(player, DataBase.Collections.Players);

           _database.Verify(db => db.Save(player, DataBase.Collections.Players), Times.Once());

            Assert.True(result);

        }

        [Fact]
        public void GetCollection()
        {

             _database.Object.GetCollection<string>(DataBase.Collections.Players);

            _database.Verify(db => db.GetCollection<string>(DataBase.Collections.Players), Times.Once());

        }


        [Fact]
        public void GetById()
        {

            var expected = "found object";

            _database.Setup((d => d.GetById<string>(1, DataBase.Collections.Players))).Returns("found object");

            var result = _database.Object.GetById<string>(1, DataBase.Collections.Players);

            _database.Verify(db => db.GetById<string>(1, DataBase.Collections.Players), Times.Once());

            Assert.Equal(expected, result);

        }
    }
}
