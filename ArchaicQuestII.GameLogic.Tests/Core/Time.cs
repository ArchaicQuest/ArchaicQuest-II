using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Core;
using Moq;
using Xunit;

namespace ArchaicQuestII.GameLogic.Tests.Core
{
    public class Time
    {
        private readonly Mock<IWriteToClient> _writeToClient;
        private readonly Mock<ICache> _cache;
        public Time()
        {
        _writeToClient = new Mock<IWriteToClient>();
        _cache = new Mock<ICache>();
        }

        [Fact]
        public void Get_MUD_time_on_birth()
        {
            var time = new GameLogic.Core.Time(_writeToClient.Object, _cache.Object);

            var startDate = new DateTime(2016, 04, 14);
            var endDate = new DateTime(2021, 10, 16);

            var timePassed = time.MudTimePassed(DateTime.Now, startDate);

            var month = time.Months[(int)timePassed.Month];

            var dayName = time.GetDay((int) timePassed.Day);
            Assert.Equal("Day of Mars, 25th month of the battle, year 236", $"Day of {dayName}, {timePassed.Month}th month of {month}, year {timePassed.Year}");

            //Day of Jupiter, 25th month of the battle, year 236

        }


     



    }
}
