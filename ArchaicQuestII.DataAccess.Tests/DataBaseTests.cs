using System;
using System.Collections.Generic;
using Moq;
using Xunit;

namespace ArchaicQuestII.DataAccess.Tests
{
    public class DataBaseTests
    {
        private readonly Mock<IDataBase> _database;

        public DataBaseTests()
        {
            _database = new Mock<IDataBase>();
        }
 

        [Fact]
        public void Save()
        {
            var player = true;
            //arrange
           _database.Setup((d => d.Save(player, "Players"))).Returns(true);

            var result = _database.Object.Save(player, "Players");

           _database.Verify(db => db.Save(player, "Players"), Times.Once());

            Assert.True(result);

        }

        [Fact]
        public void GetCollection()
        {
            var data = new List<string>()
            {
                "test"
            };

            var expected = new List<string>()
            {
                "test"
            };

            _database.Setup((d => d.GetCollection<string>("Players"))).Returns(data);

            var result = _database.Object.GetCollection<string>("Players");

            _database.Verify(db => db.GetCollection<string>("Players"), Times.Once());

         Assert.Equal(expected, result);

        }


        [Fact]
        public void GetById()
        {

            var expected = "found object";

            _database.Setup((d => d.GetById<string>("1", "Players"))).Returns("found object");

            var result = _database.Object.GetById<string>("1", "Players");

            _database.Verify(db => db.GetById<string>("1", "Players"), Times.Once());

            Assert.Equal(expected, result);

        }
    }
}
