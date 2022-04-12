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
        private readonly Mock<IPlayerDataBase> _playerDatabse;


        public DataBaseTests()
        {
            _database = new Mock<IDataBase>();
            _playerDatabse = new Mock<IPlayerDataBase>();
        }
 

        [Fact]
        public void Save()
        {
            var player = true;
            //arrange
            _playerDatabse.Setup((d => d.Save(player,PlayerDataBase.Collections.Players))).Returns(true);

            var result = _playerDatabse.Object.Save(player, PlayerDataBase.Collections.Players);

            _playerDatabse.Verify(db => db.Save(player, PlayerDataBase.Collections.Players), Times.Once());

            Assert.True(result);

        }

        [Fact]
        public void GetCollection()
        {

             _playerDatabse.Object.GetCollection<string>(PlayerDataBase.Collections.Players);

            _playerDatabse.Verify(db => db.GetCollection<string>(PlayerDataBase.Collections.Players), Times.Once());

        }


        [Fact]
        public void GetById()
        {

            var expected = "found object";

            _playerDatabse.Setup((d => d.GetById<string>(1, PlayerDataBase.Collections.Players))).Returns("found object");

            var result = _playerDatabse.Object.GetById<string>(1, PlayerDataBase.Collections.Players);

            _playerDatabse.Verify(db => db.GetById<string>(1, PlayerDataBase.Collections.Players), Times.Once());

            Assert.Equal(expected, result);

        }
    }
}
