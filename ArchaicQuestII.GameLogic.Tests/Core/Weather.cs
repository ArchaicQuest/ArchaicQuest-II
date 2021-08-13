using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Core;
using Moq;
using Xunit;

namespace ArchaicQuestII.GameLogic.Tests.Core
{
    public class Weather
    {
        private readonly Mock<IDice> _dice;
        public Weather()
        {
            _dice = new Mock<IDice>();
        }
        [Fact]
        public void Returns_correct_nth_cont()
        {
            var weather = new GameLogic.Core.Weather(_dice.Object);

         var a =  weather.AutumnWeatherTransitions();
         _dice.Setup(x => x.Roll(1, 1, 100)).Returns(10);
            var b = weather.AutumnWeatherTransitions();
            _dice.Setup(x => x.Roll(1, 1, 100)).Returns(9);
            var c = weather.AutumnWeatherTransitions();
            _dice.Setup(x => x.Roll(1, 1, 100)).Returns(8);
            var d = weather.AutumnWeatherTransitions();
            _dice.Setup(x => x.Roll(1, 1, 100)).Returns(7);
            var e = weather.AutumnWeatherTransitions();
            _dice.Setup(x => x.Roll(1, 1, 100)).Returns(6);
            var f = weather.AutumnWeatherTransitions();

            _dice.Setup(x => x.Roll(1, 1, 100)).Returns(5);
            var g = weather.AutumnWeatherTransitions();

            _dice.Setup(x => x.Roll(1, 1, 100)).Returns(4);
            var h = weather.AutumnWeatherTransitions();

            _dice.Setup(x => x.Roll(1, 1, 100)).Returns(3);
            var i = weather.AutumnWeatherTransitions();

            _dice.Setup(x => x.Roll(1, 1, 100)).Returns(2);
            var j = weather.AutumnWeatherTransitions();

            //bad to good

            var k = weather.AutumnWeatherTransitions();
            _dice.Setup(x => x.Roll(1, 1, 100)).Returns(11);
            var l = weather.AutumnWeatherTransitions();
            _dice.Setup(x => x.Roll(1, 1, 100)).Returns(12);
            var m = weather.AutumnWeatherTransitions();
            _dice.Setup(x => x.Roll(1, 1, 100)).Returns(13);
            var n = weather.AutumnWeatherTransitions();
            _dice.Setup(x => x.Roll(1, 1, 100)).Returns(14);
            var o = weather.AutumnWeatherTransitions();
            _dice.Setup(x => x.Roll(1, 1, 100)).Returns(15);
            var p = weather.AutumnWeatherTransitions();

            _dice.Setup(x => x.Roll(1, 1, 100)).Returns(16);
            var q = weather.AutumnWeatherTransitions();

            _dice.Setup(x => x.Roll(1, 1, 100)).Returns(17);
            var r = weather.AutumnWeatherTransitions();

            _dice.Setup(x => x.Roll(1, 1, 100)).Returns(18);
            var s = weather.AutumnWeatherTransitions();

            _dice.Setup(x => x.Roll(1, 1, 100)).Returns(19);
            var t = weather.AutumnWeatherTransitions();

            // sim random

            //bad to good

            var v = weather.AutumnWeatherTransitions();
            _dice.Setup(x => x.Roll(1, 1, 100)).Returns(11);
            var w = weather.AutumnWeatherTransitions();
            _dice.Setup(x => x.Roll(1, 1, 100)).Returns(10);
            var x = weather.AutumnWeatherTransitions();
            _dice.Setup(x => x.Roll(1, 1, 100)).Returns(11);
            var z = weather.AutumnWeatherTransitions();
            _dice.Setup(x => x.Roll(1, 1, 100)).Returns(12);
            var AB = weather.AutumnWeatherTransitions();
            _dice.Setup(x => x.Roll(1, 1, 100)).Returns(15);
             
            Assert.NotNull(a);

        }
    }
}
