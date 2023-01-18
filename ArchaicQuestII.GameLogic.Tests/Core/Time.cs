using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Client;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World;
using Moq;
using Xunit;

namespace ArchaicQuestII.GameLogic.Tests.Core
{
    /* tests fail because of dateTime now is not mocked in the function that returns the correct string for the hour */
    public class Time
    {
        private readonly Mock<IClientHandler> _clientHandler;
        private readonly Mock<IWorldHandler> _worldHandler;

        public Time()
        {
            _clientHandler = new Mock<IClientHandler>();
            _worldHandler = new Mock<IWorldHandler>();
        }

        // [Fact]
        // public void Get_MUD_time_on_birth()
        // {
        //     var time = new GameLogic.Core.Time(_writeToClient.Object, _cache.Object);
        //
        //     var startDate = new DateTime(2016, 04, 14);
        //     var endDate = new DateTime(2021, 10, 16);
        //
        //     var timePassed = time.MudTimePassed(DateTime.Now, startDate);
        //
        //     var month = time.Months[(int)timePassed.Month];
        //
        //     var dayName = time.GetDay((int) timePassed.Day);
        //     var date = time.ReturnDate();
        //     Assert.Equal("Day of TheSun, 20th month of The Dragon, year 254.", date);
        //
        //
        // }
        //
        //
        //
        // [Fact]
        // public void Time_cycle_MIDNIGHT()
        // {
        //     var time = new GameLogic.Core.Time(_writeToClient.Object, _cache.Object);
        //
        //     var startDate = new DateTime(2016, 04, 14);
        //     var endDate = new DateTime(2021, 04, 14);
        //
        //     var timePassed = time.MudTimePassed(endDate, startDate);
        //     time.GameTime = timePassed;
        //
        //     var month = time.UpdateTime();
        //
        //  
        //     Assert.Equal("The moon is high in the sky.", month);
        //
        // }
        //
        // [Fact]
        // public void Time_cycle_1AM()
        // {
        //     var time = new GameLogic.Core.Time(_writeToClient.Object, _cache.Object);
        //
        //     var startDate = new DateTime(2016, 04, 14);
        //     var endDate = new DateTime(2021, 04, 14).AddMinutes(1);
        //
        //     var timePassed = time.MudTimePassed(endDate, startDate);
        //     time.GameTime = timePassed;
        //
        //     var month = time.UpdateTime();
        //
        //
        //     Assert.Equal("The moon is slowly moving west across the sky.", month);
        //
        // }
        //
        // [Fact]
        // public void Time_cycle_2AM()
        // {
        //     var time = new GameLogic.Core.Time(_writeToClient.Object, _cache.Object);
        //
        //     var startDate = new DateTime(2016, 04, 14);
        //     var endDate = new DateTime(2021, 04, 14).AddMinutes(2);
        //
        //     var timePassed = time.MudTimePassed(endDate, startDate);
        //     time.GameTime = timePassed;
        //
        //     var month = time.UpdateTime();
        //
        //
        //     Assert.Equal("The moon is slowly moving west across the sky.", month);
        //
        // }
        //
        // [Fact]
        // public void Time_cycle_3AM()
        // {
        //     var time = new GameLogic.Core.Time(_writeToClient.Object, _cache.Object);
        //
        //     var startDate = new DateTime(2016, 04, 14);
        //     var endDate = new DateTime(2021, 04, 14).AddMinutes(3);
        //
        //     var timePassed = time.MudTimePassed(endDate, startDate);
        //     time.GameTime = timePassed;
        //
        //     var month = time.UpdateTime();
        //
        //
        //     Assert.Equal("The moon is slowly moving west across the sky.", month);
        //
        // }
        //
        // [Fact]
        // public void Time_cycle_4AM()
        // {
        //     var time = new GameLogic.Core.Time(_writeToClient.Object, _cache.Object);
        //
        //     var startDate = new DateTime(2016, 04, 14);
        //     var endDate = new DateTime(2021, 04, 14).AddMinutes(4);
        //
        //     var timePassed = time.MudTimePassed(endDate, startDate);
        //     time.GameTime = timePassed;
        //
        //     var month = time.UpdateTime();
        //
        //
        //     Assert.Equal("The moon slowly sets in the west.", month);
        //
        // }
        //
        //
        // [Fact]
        // public void Time_cycle_5AM()
        // {
        //     var time = new GameLogic.Core.Time(_writeToClient.Object, _cache.Object);
        //
        //     var startDate = new DateTime(2016, 04, 14);
        //     var endDate = new DateTime(2021, 04, 14).AddMinutes(5);
        //
        //     var timePassed = time.MudTimePassed(endDate, startDate);
        //     time.GameTime = timePassed;
        //
        //     var month = time.UpdateTime();
        //
        //
        //     Assert.Equal("", month);
        //
        // }
        //
        //
        //
        // [Fact]
        // public void Time_cycle_6AM()
        // {
        //     var time = new GameLogic.Core.Time(_writeToClient.Object, _cache.Object);
        //
        //     var startDate = new DateTime(2016, 04, 14);
        //     var endDate = new DateTime(2021, 04, 14).AddMinutes(6);
        //
        //     var timePassed = time.MudTimePassed(endDate, startDate);
        //     time.GameTime = timePassed;
        //
        //     var month = time.UpdateTime();
        //
        //
        //     Assert.Equal("The sun slowly rises from the east.", month);
        //
        // }
        //
        // [Fact]
        // public void Time_cycle_7AM()
        // {
        //     var time = new GameLogic.Core.Time(_writeToClient.Object, _cache.Object);
        //
        //     var startDate = new DateTime(2016, 04, 14);
        //     var endDate = new DateTime(2021, 04, 14).AddMinutes(7);
        //
        //     var timePassed = time.MudTimePassed(endDate, startDate);
        //     time.GameTime = timePassed;
        //
        //     var month = time.UpdateTime();
        //
        //
        //     Assert.Equal("", month);
        //
        // }
        //
        // [Fact]
        // public void Time_cycle_8AM()
        // {
        //     var time = new GameLogic.Core.Time(_writeToClient.Object, _cache.Object);
        //
        //     var startDate = new DateTime(2016, 04, 14);
        //     var endDate = new DateTime(2021, 04, 14).AddMinutes(8);
        //
        //     var timePassed = time.MudTimePassed(endDate, startDate);
        //     time.GameTime = timePassed;
        //
        //     var month = time.UpdateTime();
        //
        //
        //     Assert.Equal("The sun has risen from the east, the day has begun.", month);
        //
        // }
        //
        // [Fact]
        // public void Time_cycle_9AM()
        // {
        //     var time = new GameLogic.Core.Time(_writeToClient.Object, _cache.Object);
        //
        //     var startDate = new DateTime(2016, 04, 14);
        //     var endDate = new DateTime(2021, 04, 14).AddMinutes(9);
        //
        //     var timePassed = time.MudTimePassed(endDate, startDate);
        //     time.GameTime = timePassed;
        //
        //     var month = time.UpdateTime();
        //
        //
        //     Assert.Equal("The sun is slowly moving west across the sky.", month);
        //
        // }
        //
        // [Fact]
        // public void Time_cycle_10AM()
        // {
        //     var time = new GameLogic.Core.Time(_writeToClient.Object, _cache.Object);
        //
        //     var startDate = new DateTime(2016, 04, 14);
        //     var endDate = new DateTime(2021, 04, 14).AddMinutes(10);
        //
        //     var timePassed = time.MudTimePassed(endDate, startDate);
        //     time.GameTime = timePassed;
        //
        //     var month = time.UpdateTime();
        //
        //
        //     Assert.Equal("The sun is slowly moving west across the sky.", month);
        //
        // }
        //
        //
        // [Fact]
        // public void Time_cycle_11AM()
        // {
        //     var time = new GameLogic.Core.Time(_writeToClient.Object, _cache.Object);
        //
        //     var startDate = new DateTime(2016, 04, 14);
        //     var endDate = new DateTime(2021, 04, 14).AddMinutes(11);
        //
        //     var timePassed = time.MudTimePassed(endDate, startDate);
        //     time.GameTime = timePassed;
        //
        //     var month = time.UpdateTime();
        //
        //
        //     Assert.Equal("The sun is slowly moving west across the sky.", month);
        //
        // }
        //
        //
        // [Fact]
        // public void Time_cycle_12AM()
        // {
        //     var time = new GameLogic.Core.Time(_writeToClient.Object, _cache.Object);
        //
        //     var startDate = new DateTime(2016, 04, 14);
        //     var endDate = new DateTime(2021, 04, 14).AddMinutes(12);
        //
        //     var timePassed = time.MudTimePassed(endDate, startDate);
        //     time.GameTime = timePassed;
        //
        //     var month = time.UpdateTime();
        //
        //
        //     Assert.Equal("The sun is high in the sky.", month);
        //
        // }
        //
        // [Fact]
        // public void Time_cycle_1PM()
        // {
        //     var time = new GameLogic.Core.Time(_writeToClient.Object, _cache.Object);
        //
        //     var startDate = new DateTime(2016, 04, 14);
        //     var endDate = new DateTime(2021, 04, 14).AddMinutes(13);
        //
        //     var timePassed = time.MudTimePassed(endDate, startDate);
        //     time.GameTime = timePassed;
        //
        //     var month = time.UpdateTime();
        //
        //
        //     Assert.Equal("The sun is slowly moving west across the sky.", month);
        //
        // }
        //
        // [Fact]
        // public void Time_cycle_2PM()
        // {
        //     var time = new GameLogic.Core.Time(_writeToClient.Object, _cache.Object);
        //
        //     var startDate = new DateTime(2016, 04, 14);
        //     var endDate = new DateTime(2021, 04, 14).AddMinutes(14);
        //
        //     var timePassed = time.MudTimePassed(endDate, startDate);
        //     time.GameTime = timePassed;
        //
        //     var month = time.UpdateTime();
        //
        //
        //     Assert.Equal("The sun is slowly moving west across the sky.", month);
        //
        // }
        //
        // [Fact]
        // public void Time_cycle_3PM()
        // {
        //     var time = new GameLogic.Core.Time(_writeToClient.Object, _cache.Object);
        //
        //     var startDate = new DateTime(2016, 04, 14);
        //     var endDate = new DateTime(2021, 04, 14).AddMinutes(15);
        //
        //     var timePassed = time.MudTimePassed(endDate, startDate);
        //     time.GameTime = timePassed;
        //
        //     var month = time.UpdateTime();
        //
        //
        //     Assert.Equal("The sun is slowly moving west across the sky.", month);
        //
        // }
        //
        // [Fact]
        // public void Time_cycle_4PM()
        // {
        //     var time = new GameLogic.Core.Time(_writeToClient.Object, _cache.Object);
        //
        //     var startDate = new DateTime(2016, 04, 14);
        //     var endDate = new DateTime(2021, 04, 14).AddMinutes(16);
        //
        //     var timePassed = time.MudTimePassed(endDate, startDate);
        //     time.GameTime = timePassed;
        //
        //     var month = time.UpdateTime();
        //
        //
        //     Assert.Equal("The sun is slowly moving west across the sky.", month);
        //
        // }
        //
        // [Fact]
        // public void Time_cycle_5PM()
        // {
        //     var time = new GameLogic.Core.Time(_writeToClient.Object, _cache.Object);
        //
        //     var startDate = new DateTime(2016, 04, 14);
        //     var endDate = new DateTime(2021, 04, 14).AddMinutes(17);
        //
        //     var timePassed = time.MudTimePassed(endDate, startDate);
        //     time.GameTime = timePassed;
        //
        //     var month = time.UpdateTime();
        //
        //
        //     Assert.Equal("The sun is slowly moving west across the sky.", month);
        //
        // }
        //
        // [Fact]
        // public void Time_cycle_6PM()
        // { 
        //     var time = new GameLogic.Core.Time(_writeToClient.Object, _cache.Object);
        //
        //     var startDate = new DateTime(2016, 04, 14);
        //     var endDate = new DateTime(2021, 04, 14).AddMinutes(18);
        //
        //     var timePassed = time.MudTimePassed(endDate, startDate);
        //     time.GameTime = timePassed;
        //
        //     var month = time.UpdateTime();
        //
        //
        //     Assert.Equal("The sun slowly sets in the west.", month);
        //
        // }
        //
        // [Fact]
        // public void Time_cycle_7PM()
        // {
        //     var time = new GameLogic.Core.Time(_writeToClient.Object, _cache.Object);
        //
        //     var startDate = new DateTime(2016, 04, 14);
        //     var endDate = new DateTime(2021, 04, 14).AddMinutes(19);
        //
        //     var timePassed = time.MudTimePassed(endDate, startDate);
        //     time.GameTime = timePassed;
        //
        //     var month = time.UpdateTime();
        //
        //
        //     Assert.Equal("The moon slowly rises in the east.", month);
        //
        // }
        //
        // [Fact]
        // public void Time_cycle_8PM()
        // {
        //     var time = new GameLogic.Core.Time(_writeToClient.Object, _cache.Object);
        //
        //     var startDate = new DateTime(2016, 04, 14);
        //     var endDate = new DateTime(2021, 04, 14).AddMinutes(20);
        //
        //     var timePassed = time.MudTimePassed(endDate, startDate);
        //     time.GameTime = timePassed;
        //
        //     var month = time.UpdateTime();
        //
        //
        //     Assert.Equal("The moon has risen from the east, the night has begun.", month);
        //
        // }
        //
        // [Fact]
        // public void Time_cycle_9PM()
        // {
        //     var time = new GameLogic.Core.Time(_writeToClient.Object, _cache.Object);
        //
        //     var startDate = new DateTime(2016, 04, 14);
        //     var endDate = new DateTime(2021, 04, 14).AddMinutes(21);
        //
        //     var timePassed = time.MudTimePassed(endDate, startDate);
        //     time.GameTime = timePassed;
        //
        //     var month = time.UpdateTime();
        //
        //
        //     Assert.Equal("The moon is slowly moving west across the sky.", month);
        //
        // }
        //
        // [Fact]
        // public void Time_cycle_10PM()
        // {
        //     var time = new GameLogic.Core.Time(_writeToClient.Object, _cache.Object);
        //
        //     var startDate = new DateTime(2016, 04, 14);
        //     var endDate = new DateTime(2021, 04, 14).AddMinutes(22);
        //
        //     var timePassed = time.MudTimePassed(endDate, startDate);
        //     time.GameTime = timePassed;
        //     
        //     var month = time.UpdateTime();
        //     
        //     Assert.Equal("The moon is slowly moving west across the sky.", month);
        //
        // }
        //
        // [Fact]
        // public void Time_cycle_11PM()
        // {
        //     var time = new GameLogic.Core.Time(_writeToClient.Object, _cache.Object);
        //
        //     var startDate = new DateTime(2016, 04, 14);
        //     var endDate = new DateTime(2021, 04, 14).AddMinutes(23);
        //
        //     var timePassed = time.MudTimePassed(endDate, startDate);
        //     time.GameTime = timePassed;
        //
        //     var month = time.UpdateTime();
        //
        //
        //     Assert.Equal("The moon is slowly moving west across the sky.", month);
        //
        // }
        //
        // [Fact]
        // public void Time_cycle_12PM()
        // {
        //     var time = new GameLogic.Core.Time(_writeToClient.Object, _cache.Object);
        //
        //     var startDate = new DateTime(2016, 04, 14);
        //     var endDate = new DateTime(2021, 04, 14).AddMinutes(24);
        //
        //     var timePassed = time.MudTimePassed(endDate, startDate);
        //     time.GameTime = timePassed;
        //
        //     var month = time.UpdateTime();
        //
        //
        //     Assert.Equal("The moon is high in the sky.", month);
        //
        // }
        //
        // [Fact]
        // public void Time_cycle_1AM_day2()
        // {
        //     var time = new GameLogic.Core.Time(_writeToClient.Object, _cache.Object);
        //
        //     var startDate = new DateTime(2016, 04, 14);
        //     var endDate = new DateTime(2021, 04, 14).AddMinutes(25);
        //
        //     var timePassed = time.MudTimePassed(endDate, startDate);
        //     time.GameTime = timePassed;
        //
        //     var month = time.UpdateTime();
        //
        //
        //     Assert.Equal("The moon is slowly moving west across the sky.", month);
        //
        // }
        //
        // [Fact]
        // public void Time_cycle_2AM_day2()
        // {
        //     var time = new GameLogic.Core.Time(_writeToClient.Object, _cache.Object);
        //
        //     var startDate = new DateTime(2016, 04, 14);
        //     var endDate = new DateTime(2021, 04, 14).AddMinutes(26);
        //
        //     var timePassed = time.MudTimePassed(endDate, startDate);
        //     time.GameTime = timePassed;
        //
        //     var month = time.UpdateTime();
        //
        //
        //     Assert.Equal("The moon is slowly moving west across the sky.", month);
        //
        // }
        //
        //
        // [Fact]
        // public void Time_cycle_3AM_day2()
        // {
        //     var time = new GameLogic.Core.Time(_writeToClient.Object, _cache.Object);
        //
        //     var startDate = new DateTime(2016, 04, 14);
        //     var endDate = new DateTime(2021, 04, 14).AddMinutes(27);
        //
        //     var timePassed = time.MudTimePassed(endDate, startDate);
        //     time.GameTime = timePassed;
        //
        //     var month = time.UpdateTime();
        //
        //
        //     Assert.Equal("The moon is slowly moving west across the sky.", month);
        //
        // }
        //
        // [Fact]
        // public void Time_cycle_4AM_day2()
        // {
        //     var time = new GameLogic.Core.Time(_writeToClient.Object, _cache.Object);
        //
        //     var startDate = new DateTime(2016, 04, 14);
        //     var endDate = new DateTime(2021, 04, 14).AddMinutes(28);
        //
        //     var timePassed = time.MudTimePassed(endDate, startDate);
        //     time.GameTime = timePassed;
        //
        //     var month = time.UpdateTime();
        //
        //
        //     Assert.Equal("The moon slowly sets in the west.", month);
        //
        // }
        //
        //
        // [Fact]
        // public void Time_cycle_5AM_day2()
        // {
        //     var time = new GameLogic.Core.Time(_writeToClient.Object, _cache.Object);
        //
        //     var startDate = new DateTime(2016, 04, 14);
        //     var endDate = new DateTime(2021, 04, 14).AddMinutes(29);
        //
        //     var timePassed = time.MudTimePassed(endDate, startDate);
        //     time.GameTime = timePassed;
        //
        //     var month = time.UpdateTime();
        //
        //
        //     Assert.Equal("", month);
        //
        // }
        //
        //
        // [Fact]
        // public void Time_cycle_6AM_day2()
        // {
        //     var time = new GameLogic.Core.Time(_writeToClient.Object, _cache.Object);
        //
        //     var startDate = new DateTime(2016, 04, 14);
        //     var endDate = new DateTime(2021, 04, 14).AddMinutes(30);
        //
        //     var timePassed = time.MudTimePassed(endDate, startDate);
        //     time.GameTime = timePassed;
        //
        //     var month = time.UpdateTime();
        //
        //
        //     Assert.Equal("The sun slowly rises from the east.", month);
        //
        // }
        //
        //
        // [Fact]
        // public void Time_cycle_7AM_day2()
        // {
        //     var time = new GameLogic.Core.Time(_writeToClient.Object, _cache.Object);
        //
        //     var startDate = new DateTime(2016, 04, 14);
        //     var endDate = new DateTime(2021, 04, 14).AddMinutes(31);
        //
        //     var timePassed = time.MudTimePassed(endDate, startDate);
        //     time.GameTime = timePassed;
        //
        //     var month = time.UpdateTime();
        //
        //
        //     Assert.Equal("", month);
        // }  
        //
        // [Fact]
        // public void Time_cycle_8AM_day2()
        // {
        //     var time = new GameLogic.Core.Time(_writeToClient.Object, _cache.Object);
        //
        //     var startDate = new DateTime(2016, 04, 14);
        //     var endDate = new DateTime(2021, 04, 14).AddMinutes(32);
        //
        //     var timePassed = time.MudTimePassed(endDate, startDate);
        //     time.GameTime = timePassed;
        //
        //     var month = time.UpdateTime();
        //
        //
        //     Assert.Equal("The sun has risen from the east, the day has begun.", month);
        //     
        // }
        //
        // [Fact]
        // public void Time_cycle_9AM_day2()
        // {
        //     var time = new GameLogic.Core.Time(_writeToClient.Object, _cache.Object);
        //
        //     var startDate = new DateTime(2016, 04, 14);
        //     var endDate = new DateTime(2021, 04, 14).AddMinutes(33);
        //
        //     var timePassed = time.MudTimePassed(endDate, startDate);
        //     time.GameTime = timePassed;
        //
        //     var month = time.UpdateTime();
        //
        //
        //     Assert.Equal("The sun is slowly moving west across the sky.", month);
        //
        // }
        //
        // [Fact]
        // public void Time_cycle_10AM_day2()
        // {
        //     var time = new GameLogic.Core.Time(_writeToClient.Object, _cache.Object);
        //
        //     var startDate = new DateTime(2016, 04, 14);
        //     var endDate = new DateTime(2021, 04, 14).AddMinutes(34);
        //
        //     var timePassed = time.MudTimePassed(endDate, startDate);
        //     time.GameTime = timePassed;
        //
        //     var month = time.UpdateTime();
        //
        //
        //     Assert.Equal("The sun is slowly moving west across the sky.", month);
        //
        // }
        //
        // [Fact]
        // public void Time_cycle_11AM_day2()
        // {
        //     var time = new GameLogic.Core.Time(_writeToClient.Object, _cache.Object);
        //
        //     var startDate = new DateTime(2016, 04, 14);
        //     var endDate = new DateTime(2021, 04, 14).AddMinutes(35);
        //
        //     var timePassed = time.MudTimePassed(endDate, startDate);
        //     time.GameTime = timePassed;
        //
        //     var month = time.UpdateTime();
        //
        //
        //     Assert.Equal("The sun is slowly moving west across the sky.", month);
        //
        // }


    }
}
