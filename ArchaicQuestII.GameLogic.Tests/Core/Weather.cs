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
        private readonly Mock<ITime> _time;
        public Weather()
        {
            _dice = new Mock<IDice>();
            _time = new Mock<ITime>();
        }
        
        /* see Time tests */

        // [Fact]
        // public void SunnyToCloudy()
        // {
        //     var weather = new GameLogic.Core.Weather(_dice.Object, _time.Object);
        //
        //     //// sunny to cloudy
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(10);
        //     var a = weather.SimulateWeatherTransitions();
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(10);
        //     var b = weather.SimulateWeatherTransitions();
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(10);
        //     var c = weather.SimulateWeatherTransitions();
        //     Assert.Equal("The sky is blue with a few wispy clouds.", a);
        //     Assert.Equal("The sky begins to get more cloudy.", b);
        //     Assert.Equal("More clouds roll in creating a blanket over the sky.", c);
        //
        //     
        //
        // }
        //
        // [Fact]
        // public void Cloudy()
        // {
        //     var weather = new GameLogic.Core.Weather(_dice.Object, _time.Object);
        //
        //     //// sunny to cloudy
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(10);
        //     var a = weather.SimulateWeatherTransitions();
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(10);
        //     var b = weather.SimulateWeatherTransitions();
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(10);
        //     var c = weather.SimulateWeatherTransitions();
        //     Assert.Equal("The sky is blue with a few wispy clouds.", a);
        //     Assert.Equal("The sky begins to get more cloudy.", b);
        //     Assert.Equal("More clouds roll in creating a blanket over the sky.", c);
        //
        //     //// Cloudy
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(10);
        //     var e = weather.SimulateWeatherTransitions();
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(70);
        //     var f = weather.SimulateWeatherTransitions();
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(50);
        //     var g = weather.SimulateWeatherTransitions();
        //
        //     Assert.Equal("Clouds cover the sky", e);
        //
        //     Assert.Equal("Clouds cover the sky", f);
        //
        //     Assert.Equal("Clouds cover the sky", g);
        //
        // }
        //
        // [Fact]
        // public void CloudyToRain()
        // {
        //     var weather = new GameLogic.Core.Weather(_dice.Object, _time.Object);
        //
        //     //// sunny to cloudy
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(10);
        //     var a = weather.SimulateWeatherTransitions();
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(10);
        //     var b = weather.SimulateWeatherTransitions();
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(10);
        //     var c = weather.SimulateWeatherTransitions();
        //     Assert.Equal("The sky is blue with a few wispy clouds.", a);
        //     Assert.Equal("The sky begins to get more cloudy.", b);
        //     Assert.Equal("More clouds roll in creating a blanket over the sky.", c);
        //
        //     //// Cloudy
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(10);
        //     var e = weather.SimulateWeatherTransitions();
        //
        //     //rain
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(85);
        //     var f = weather.SimulateWeatherTransitions();
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(50);
        //     var g = weather.SimulateWeatherTransitions();
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(90);
        //     var h = weather.SimulateWeatherTransitions();
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(90);
        //     var i = weather.SimulateWeatherTransitions();
        //
        //
        //     Assert.Equal("Clouds cover the sky", f);
        //
        //     Assert.Equal("The clouds appear darker in the sky.", g);
        //
        //     Assert.Equal("Light rain patters on the ground around you.", h);
        //
        //     Assert.Equal("The light rain picks up a bit.", i);
        //
        // }
        //
        // [Fact]
        // public void LightRain()
        // {
        //     var weather = new GameLogic.Core.Weather(_dice.Object, _time.Object);
        //
        //     //// sunny to cloudy
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(10);
        //     var a = weather.SimulateWeatherTransitions();
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(10);
        //     var b = weather.SimulateWeatherTransitions();
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(10);
        //     var c = weather.SimulateWeatherTransitions();
        //     Assert.Equal("The sky is blue with a few wispy clouds.", a);
        //     Assert.Equal("The sky begins to get more cloudy.", b);
        //     Assert.Equal("More clouds roll in creating a blanket over the sky.", c);
        //
        //     //// Cloudy
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(10);
        //     var e = weather.SimulateWeatherTransitions();
        //
        //     //rain
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(85);
        //     var f = weather.SimulateWeatherTransitions();
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(50);
        //     var g = weather.SimulateWeatherTransitions();
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(90);
        //     var h = weather.SimulateWeatherTransitions();
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(90);
        //     var i = weather.SimulateWeatherTransitions();
        //
        //
        //     Assert.Equal("Clouds cover the sky", f);
        //
        //     Assert.Equal("The clouds appear darker in the sky.", g);
        //
        //     Assert.Equal("Light rain patters on the ground around you.", h);
        //
        //     Assert.Equal("The light rain picks up a bit.", i);
        //
        //     _dice.SetupSequence(x => x.Roll(1, 1, 100)).Returns(50).Returns(0);
        //     
        //     var j = weather.SimulateWeatherTransitions();
        //
        //     _dice.SetupSequence(x => x.Roll(1, 1, 100)).Returns(50).Returns(1);
        //
        //     _dice.Setup(x => x.Roll(1, 0, 1)).Returns(1);
        //     var k = weather.SimulateWeatherTransitions();
        //
        //     Assert.Equal("The rain falls steady.", j);
        //
        //     Assert.Equal("The rain falls steadily forming small puddles here and there.", k);
        // }
        //
        // [Fact]
        // public void LightRainToCloudy()
        // {
        //     var weather = new GameLogic.Core.Weather(_dice.Object, _time.Object);
        //
        //     //// sunny to cloudy
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(10);
        //     var a = weather.SimulateWeatherTransitions();
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(10);
        //     var b = weather.SimulateWeatherTransitions();
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(10);
        //     var c = weather.SimulateWeatherTransitions();
        //     Assert.Equal("The sky is blue with a few wispy clouds.", a);
        //     Assert.Equal("The sky begins to get more cloudy.", b);
        //     Assert.Equal("More clouds roll in creating a blanket over the sky.", c);
        //
        //     //// Cloudy
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(10);
        //     var e = weather.SimulateWeatherTransitions();
        //
        //     //rain
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(85);
        //     var f = weather.SimulateWeatherTransitions();
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(50);
        //     var g = weather.SimulateWeatherTransitions();
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(90);
        //     var h = weather.SimulateWeatherTransitions();
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(90);
        //     var i = weather.SimulateWeatherTransitions();
        //
        //
        //     Assert.Equal("Clouds cover the sky", f);
        //
        //     Assert.Equal("The clouds appear darker in the sky.", g);
        //
        //     Assert.Equal("Light rain patters on the ground around you.", h);
        //
        //     Assert.Equal("The light rain picks up a bit.", i);
        //
        //     _dice.SetupSequence(x => x.Roll(1, 1, 100)).Returns(50).Returns(0);
        //
        //     var j = weather.SimulateWeatherTransitions();
        //
        //     _dice.SetupSequence(x => x.Roll(1, 1, 100)).Returns(50).Returns(1);
        //
        //     _dice.Setup(x => x.Roll(1, 0, 1)).Returns(1);
        //     var k = weather.SimulateWeatherTransitions();
        //
        //     Assert.Equal("The rain falls steady.", j);
        //
        //     Assert.Equal("The rain falls steadily forming small puddles here and there.", k);
        //
        //     _dice.Setup(x => x.Roll(1, 0, 1)).Returns(0);
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(15);
        //     var l = weather.SimulateWeatherTransitions();
        //
        //     Assert.Equal("The rain falls steady.", l);
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(15);
        //     var m = weather.SimulateWeatherTransitions();
        //
        //     Assert.Equal("The rain slows to a light rain.", m);
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(15);
        //     var n = weather.SimulateWeatherTransitions();
        //
        //     Assert.Equal("The rain reduces to a drizzle.", n);
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(15);
        //     var o = weather.SimulateWeatherTransitions();
        //
        //     Assert.Equal("The rain stops.", o);
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(15);
        //     var p = weather.SimulateWeatherTransitions();
        //
        //     Assert.Equal("Clouds cover the sky", p);
        //
        // }
        //
        // [Fact]
        // public void LightRainToHeavyRain()
        // {
        //     var weather = new GameLogic.Core.Weather(_dice.Object, _time.Object);
        //
        //     //// sunny to cloudy
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(10);
        //     var a = weather.SimulateWeatherTransitions();
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(10);
        //     var b = weather.SimulateWeatherTransitions();
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(10);
        //     var c = weather.SimulateWeatherTransitions();
        //     Assert.Equal("The sky is blue with a few wispy clouds.", a);
        //     Assert.Equal("The sky begins to get more cloudy.", b);
        //     Assert.Equal("More clouds roll in creating a blanket over the sky.", c);
        //
        //     //// Cloudy
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(10);
        //     var e = weather.SimulateWeatherTransitions();
        //
        //     //rain
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(85);
        //     var f = weather.SimulateWeatherTransitions();
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(50);
        //     var g = weather.SimulateWeatherTransitions();
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(90);
        //     var h = weather.SimulateWeatherTransitions();
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(90);
        //     var i = weather.SimulateWeatherTransitions();
        //
        //
        //     Assert.Equal("Clouds cover the sky", f);
        //
        //     Assert.Equal("The clouds appear darker in the sky.", g);
        //
        //     Assert.Equal("Light rain patters on the ground around you.", h);
        //
        //     Assert.Equal("The light rain picks up a bit.", i);
        //
        //     _dice.SetupSequence(x => x.Roll(1, 1, 100)).Returns(50).Returns(0);
        //
        //     var j = weather.SimulateWeatherTransitions();
        //
        //     _dice.SetupSequence(x => x.Roll(1, 1, 100)).Returns(50).Returns(1);
        //
        //     _dice.Setup(x => x.Roll(1, 0, 1)).Returns(1);
        //     var k = weather.SimulateWeatherTransitions();
        //
        //     Assert.Equal("The rain falls steady.", j);
        //
        //     Assert.Equal("The rain falls steadily forming small puddles here and there.", k);
        //
        //     _dice.Setup(x => x.Roll(1, 0, 1)).Returns(0);
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(75);
        //     var l = weather.SimulateWeatherTransitions();
        //
        //     Assert.Equal("The rain falls steady.", l);
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(75);
        //     var m = weather.SimulateWeatherTransitions();
        //
        //     Assert.Equal("The rain begins to fall heavily.", m);
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(75);
        //     var n = weather.SimulateWeatherTransitions();
        //
        //     Assert.Equal("The rain falls heavily forming puddles here and there.", n);
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(75);
        //     var o = weather.SimulateWeatherTransitions();
        //
        //     Assert.Equal("The rain continues to fall heavily.", o);
        //
        // }
        //
        // [Fact]
        // public void HeavyRainToLightRain()
        // {
        //     var weather = new GameLogic.Core.Weather(_dice.Object, _time.Object);
        //
        //     //// sunny to cloudy
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(10);
        //     var a = weather.SimulateWeatherTransitions();
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(10);
        //     var b = weather.SimulateWeatherTransitions();
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(10);
        //     var c = weather.SimulateWeatherTransitions();
        //     Assert.Equal("The sky is blue with a few wispy clouds.", a);
        //     Assert.Equal("The sky begins to get more cloudy.", b);
        //     Assert.Equal("More clouds roll in creating a blanket over the sky.", c);
        //
        //     //// Cloudy
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(10);
        //     var e = weather.SimulateWeatherTransitions();
        //
        //     //rain
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(85);
        //     var f = weather.SimulateWeatherTransitions();
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(50);
        //     var g = weather.SimulateWeatherTransitions();
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(90);
        //     var h = weather.SimulateWeatherTransitions();
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(90);
        //     var i = weather.SimulateWeatherTransitions();
        //
        //
        //     Assert.Equal("Clouds cover the sky", f);
        //
        //     Assert.Equal("The clouds appear darker in the sky.", g);
        //
        //     Assert.Equal("Light rain patters on the ground around you.", h);
        //
        //     Assert.Equal("The light rain picks up a bit.", i);
        //
        //     _dice.SetupSequence(x => x.Roll(1, 1, 100)).Returns(50).Returns(0);
        //
        //     var j = weather.SimulateWeatherTransitions();
        //
        //     _dice.SetupSequence(x => x.Roll(1, 1, 100)).Returns(50).Returns(1);
        //
        //     _dice.Setup(x => x.Roll(1, 0, 1)).Returns(1);
        //     var k = weather.SimulateWeatherTransitions();
        //
        //     Assert.Equal("The rain falls steady.", j);
        //
        //     Assert.Equal("The rain falls steadily forming small puddles here and there.", k);
        //
        //     _dice.Setup(x => x.Roll(1, 0, 1)).Returns(0);
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(75);
        //     var l = weather.SimulateWeatherTransitions();
        //
        //     Assert.Equal("The rain falls steady.", l);
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(75);
        //     var m = weather.SimulateWeatherTransitions();
        //
        //     Assert.Equal("The rain begins to fall heavily.", m);
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(75);
        //     var n = weather.SimulateWeatherTransitions();
        //
        //     Assert.Equal("The rain falls heavily forming puddles here and there.", n);
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(75);
        //     var o = weather.SimulateWeatherTransitions();
        //
        //     Assert.Equal("The rain continues to fall heavily.", o);
        //
        //     _dice.Setup(x => x.Roll(1, 0, 1)).Returns(0);
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(45);
        //     var p = weather.SimulateWeatherTransitions();
        //
        //     Assert.Equal("The rain falls heavily.", p);
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(45);
        //     var q = weather.SimulateWeatherTransitions();
        //
        //     Assert.Equal("The rain begins to slow down.", q);
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(45);
        //     var r = weather.SimulateWeatherTransitions();
        //
        //     Assert.Equal("The rain no longer pounds the ground and lessens some what.", r);
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(45);
        //     var s = weather.SimulateWeatherTransitions();
        //
        //     Assert.Equal("The rain slows to a light rain.", s);
        //
        // }
        //
        // [Fact]
        // public void HeavyRainToThunder()
        // {
        //     var weather = new GameLogic.Core.Weather(_dice.Object, _time.Object);
        //
        //     //// sunny to cloudy
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(10);
        //     var a = weather.SimulateWeatherTransitions();
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(10);
        //     var b = weather.SimulateWeatherTransitions();
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(10);
        //     var c = weather.SimulateWeatherTransitions();
        //     Assert.Equal("The sky is blue with a few wispy clouds.", a);
        //     Assert.Equal("The sky begins to get more cloudy.", b);
        //     Assert.Equal("More clouds roll in creating a blanket over the sky.", c);
        //
        //     //// Cloudy
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(10);
        //     var e = weather.SimulateWeatherTransitions();
        //
        //     //rain
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(85);
        //     var f = weather.SimulateWeatherTransitions();
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(50);
        //     var g = weather.SimulateWeatherTransitions();
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(90);
        //     var h = weather.SimulateWeatherTransitions();
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(90);
        //     var i = weather.SimulateWeatherTransitions();
        //
        //
        //     Assert.Equal("Clouds cover the sky", f);
        //
        //     Assert.Equal("The clouds appear darker in the sky.", g);
        //
        //     Assert.Equal("Light rain patters on the ground around you.", h);
        //
        //     Assert.Equal("The light rain picks up a bit.", i);
        //
        //     _dice.SetupSequence(x => x.Roll(1, 1, 100)).Returns(50).Returns(0);
        //
        //     var j = weather.SimulateWeatherTransitions();
        //
        //     _dice.SetupSequence(x => x.Roll(1, 1, 100)).Returns(50).Returns(1);
        //
        //     _dice.Setup(x => x.Roll(1, 0, 1)).Returns(1);
        //     var k = weather.SimulateWeatherTransitions();
        //
        //     Assert.Equal("The rain falls steady.", j);
        //
        //     Assert.Equal("The rain falls steadily forming small puddles here and there.", k);
        //
        //     _dice.Setup(x => x.Roll(1, 0, 1)).Returns(0);
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(75);
        //     var l = weather.SimulateWeatherTransitions();
        //
        //     Assert.Equal("The rain falls steady.", l);
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(75);
        //     var m = weather.SimulateWeatherTransitions();
        //
        //     Assert.Equal("The rain begins to fall heavily.", m);
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(75);
        //     var n = weather.SimulateWeatherTransitions();
        //
        //     Assert.Equal("The rain falls heavily forming puddles here and there.", n);
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(75);
        //     var o = weather.SimulateWeatherTransitions();
        //
        //     Assert.Equal("The rain continues to fall heavily.", o);
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(75);
        //     var p = weather.SimulateWeatherTransitions();
        //
        //     Assert.Equal("The rain falls heavily.", p);
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(75);
        //     var q = weather.SimulateWeatherTransitions();
        //
        //     Assert.Equal("The sound of thunder rumbles in the distance.", q);
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(75);
        //     var r = weather.SimulateWeatherTransitions();
        //
        //     Assert.Equal("Lightning flashes in the sky, accompanied shortly by booming thunder.", r);
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(75);
        //     var s = weather.SimulateWeatherTransitions();
        //
        //     Assert.Equal("Lightning forks across the sky, followed by a bang of thunder.", s);
        //
        //
        // }
        //
        //
        // [Fact]
        // public void ThunderToLightRain()
        // {
        //     var weather = new GameLogic.Core.Weather(_dice.Object, _time.Object);
        //
        //     //// sunny to cloudy
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(10);
        //     var a = weather.SimulateWeatherTransitions();
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(10);
        //     var b = weather.SimulateWeatherTransitions();
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(10);
        //     var c = weather.SimulateWeatherTransitions();
        //     Assert.Equal("The sky is blue with a few wispy clouds.", a);
        //     Assert.Equal("The sky begins to get more cloudy.", b);
        //     Assert.Equal("More clouds roll in creating a blanket over the sky.", c);
        //
        //     //// Cloudy
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(10);
        //     var e = weather.SimulateWeatherTransitions();
        //
        //     //rain
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(85);
        //     var f = weather.SimulateWeatherTransitions();
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(50);
        //     var g = weather.SimulateWeatherTransitions();
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(90);
        //     var h = weather.SimulateWeatherTransitions();
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(90);
        //     var i = weather.SimulateWeatherTransitions();
        //
        //
        //     Assert.Equal("Clouds cover the sky", f);
        //
        //     Assert.Equal("The clouds appear darker in the sky.", g);
        //
        //     Assert.Equal("Light rain patters on the ground around you.", h);
        //
        //     Assert.Equal("The light rain picks up a bit.", i);
        //
        //     _dice.SetupSequence(x => x.Roll(1, 1, 100)).Returns(50).Returns(0);
        //
        //     var j = weather.SimulateWeatherTransitions();
        //
        //     _dice.SetupSequence(x => x.Roll(1, 1, 100)).Returns(50).Returns(1);
        //
        //     _dice.Setup(x => x.Roll(1, 0, 1)).Returns(1);
        //     var k = weather.SimulateWeatherTransitions();
        //
        //     Assert.Equal("The rain falls steady.", j);
        //
        //     Assert.Equal("The rain falls steadily forming small puddles here and there.", k);
        //
        //     _dice.Setup(x => x.Roll(1, 0, 1)).Returns(0);
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(75);
        //     var l = weather.SimulateWeatherTransitions();
        //
        //     Assert.Equal("The rain falls steady.", l);
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(75);
        //     var m = weather.SimulateWeatherTransitions();
        //
        //     Assert.Equal("The rain begins to fall heavily.", m);
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(75);
        //     var n = weather.SimulateWeatherTransitions();
        //
        //     Assert.Equal("The rain falls heavily forming puddles here and there.", n);
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(75);
        //     var o = weather.SimulateWeatherTransitions();
        //
        //     Assert.Equal("The rain continues to fall heavily.", o);
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(75);
        //     var p = weather.SimulateWeatherTransitions();
        //
        //     Assert.Equal("The rain falls heavily.", p);
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(75);
        //     var q = weather.SimulateWeatherTransitions();
        //
        //     Assert.Equal("The sound of thunder rumbles in the distance.", q);
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(75);
        //     var r = weather.SimulateWeatherTransitions();
        //
        //     Assert.Equal("Lightning flashes in the sky, accompanied shortly by booming thunder.", r);
        //
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(75);
        //     var s = weather.SimulateWeatherTransitions();
        //
        //     Assert.Equal("Lightning forks across the sky, followed by a bang of thunder.", s);
        //
        //     // thunder to light rain
        //     _dice.Setup(x => x.Roll(1, 0, 1)).Returns(0);
        //     _dice.Setup(x => x.Roll(1, 1, 100)).Returns(35);
        //
        //     var t = weather.SimulateWeatherTransitions();
        //
        //     Assert.Equal("Lightning forks across the sky, followed by a bang of thunder.", t);
        //
        //     var u = weather.SimulateWeatherTransitions();
        //     _dice.Setup(x => x.Roll(1, 0, 1)).Returns(0);
        //
        //     Assert.Equal("Lightning forks across the sky, followed by a bang of thunder. The rain starts to ease.", u);
        //
        //     var v = weather.SimulateWeatherTransitions();
        //     _dice.Setup(x => x.Roll(1, 0, 1)).Returns(1);
        //
        //     Assert.Equal("A flash of lighting then a long pause before the rumble of thunder is heard.", v);
        //
        //     var w = weather.SimulateWeatherTransitions();
        //     _dice.Setup(x => x.Roll(1, 0, 1)).Returns(1);
        //
        //     Assert.Equal("The rain is no longer so heavy, the only sound of thunder is off in the distance.", w);
        //
        // }
        //

    }
}
