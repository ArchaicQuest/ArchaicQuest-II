using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ArchaicQuestII.GameLogic.Core
{
   public class Weather:IWeather {

       private IDice _dice;
       public int LastRoll = 0;
       public string WeatherState = "Sunny";
       public int SunnyToCloudyTransitionState = 0;
       public int CloudyToSunnyTransitionState = 0;
       public int CloudyToLightRainTransitionState = 0;
       public int LightRainToCloudyTransitionState = 0;
       public int LightRainToHeavyRainTransitionState = 0;
       public int HeavyRainToLightRainTransitionState = 0;
       public int HeavyRainToThunderTransitionState = 0;
       public int ThunderToLightRainTransitionState = 0;

        private ITime _time;
        

        public List<Tuple<string, string>> sunnyToCloudyTransitionStates = new List<Tuple<string, string>>()
       {
            new Tuple<string, string>("The sky is blue with a few wispy clouds.", "The sky is dark with a few wispy clouds."),
            new Tuple<string, string>("The sky begins to get more cloudy.", "The sky begins to get more cloudy."),
            new Tuple<string, string>("More clouds roll in creating a blanket over the sky.", "More clouds roll in creating a blanket over the sky."),
       };


        public List<Tuple<string, string>> cloudyToSunnyTransitionStates = new List<Tuple<string, string>>()
       {
            new Tuple<string, string>("The clouds appear lighter and brighter", "The clouds appear lighter and brighter"),
            new Tuple<string, string>("Some of the clouds begin to break.", "Some of the clouds begin to break."),
            new Tuple<string, string>("A few of the clouds begin to move out leaving only a few clouds left behind.", "A few of the clouds begin to move out leaving only a few clouds left behind.")
       };

       public List<Tuple<string, string>> CloudyStates = new List<Tuple<string, string>>()
       {
           new Tuple<string, string>("Clouds cover the sky", "Clouds cover the night sky")

       };

       public List<Tuple<string, string>> cloudyToRainyTransitionStates = new List<Tuple<string, string>>()
       {
            new Tuple<string, string>("The clouds appear darker in the sky.", "The clouds appear darker in the night sky."),
            new Tuple<string, string>("Light rain patters on the ground around you.","Light rain patters on the ground around you."),
            new Tuple<string, string>("The light rain picks up a bit.", "The light rain picks up a bit.")
       };

       public List<Tuple<string, string>> LightRainToCloudTransitionStates = new List<Tuple<string, string>>()
       {
            new Tuple<string, string>("The rain slows to a light rain.","The rain slows to a light rain."),
            new Tuple<string, string>("The rain reduces to a drizzle.", "The rain reduces to a drizzle."),
            new Tuple<string, string>("The rain stops.", "The rain stops.")
       };


       public List<Tuple<string, string>> LightRainState = new List<Tuple<string, string>>()
       {
           new Tuple<string, string>("The rain falls steady.", "The rain falls steady."),
           new Tuple<string, string>("The rain falls steadily forming small puddles here and there.", "The rain falls steadily forming small puddles here and there."),
       };


       public List<Tuple<string, string>> LightRainToHeavyRainTransitionStates = new List<Tuple<string, string>>()
       {
           new Tuple<string, string>("The rain begins to fall heavily.", "The rain begins to fall heavily."),
           new Tuple<string, string>("The rain falls heavily forming puddles here and there.", "The rain falls heavily forming puddles here and there."),
           new Tuple<string, string>("The rain continues to fall heavily.", "The rain continues to fall heavily.")
       };

       public List<Tuple<string, string>> HeavyRainState = new List<Tuple<string, string>>()
       {
            new Tuple<string, string>("The rain falls heavily.", "The rain falls heavily."),
            new Tuple<string, string>("The rain continues to fall heavily.", "The rain continues to fall heavily.")
       };

       public List<Tuple<string, string>> HeavyRainToLightRainTransitionStates = new List<Tuple<string, string>>()
       {
            new Tuple<string, string>("The rain begins to slow down.", "The rain begins to slow down."),
            new Tuple<string, string>("The rain no longer pounds the ground and lessens some what.", "The rain no longer pounds the ground and lessens some what."),
            new Tuple<string, string>("The rain slows to a light rain.", "The rain slows to a light rain.")
       };

       public List<Tuple<string, string>> HeavyRainToThunderTransitionStates = new List<Tuple<string, string>>()
       {
            new Tuple<string, string>("The sound of thunder rumbles in the distance.", "The sound of thunder rumbles in the distance."),
            new Tuple<string, string>("Lightning flashes in the sky, accompanied shortly by booming thunder.", "Lightning flashes in the sky, accompanied shortly by booming thunder."),
            new Tuple<string, string>("Lightning forks across the sky, followed by a bang of thunder.", "Lightning forks across the sky, followed by a bang of thunder."),
           
       };

       public List<Tuple<string, string>> ThunderState = new List<Tuple<string, string>>()
       {
           new Tuple<string, string>("Lightning forks across the sky, followed by a bang of thunder.", "Lightning forks across the sky, followed by a bang of thunder."),
           new Tuple<string, string>("The rain pours down on the ground. Lightning and thunder light up the sky and shake the ground.", "The rain pours down on the ground. Lightning and thunder light up the sky and shake the ground."),
           new Tuple<string, string>("Thunder cracks, and lightning flashes in the sky as the heavy rain continues to fall.", "Thunder cracks, and lightning flashes in the sky as the heavy rain continues to fall.")
       };

       public List<Tuple<string, string>> ThunderToLightRainTransitionStates = new List<Tuple<string, string>>()
       {
            new Tuple<string, string>("Lightning forks across the sky, followed by a bang of thunder. The rain starts to ease.", "Lightning forks across the sky, followed by a bang of thunder. The rain starts to ease."),
            new Tuple<string, string>("A flash of lighting then a long pause before the rumble of thunder is heard.", "A flash of lighting then a long pause before the rumble of thunder is heard."),
            new Tuple<string, string>("The rain is no longer so heavy, the only sound of thunder is off in the distance.", "The rain is no longer so heavy, the only sound of thunder is off in the distance."),

       };


        //TODO
        /*
         * Thunder to light rain
         * cloudy to light snow
         * light snow to heavy snow
         * cloudy to hail stones
         * heavy snow to blizard
         */


        public int weatherGoodToBadPos = 0;
       public int weatherBadToGoodPos = 0;
        public Weather(IDice dice, ITime time)
        {
            _time = time;
             _dice = dice;
        }

        public void UpdateWeather()
        {
            var season = "Autumn";

            switch (season)
            {
                

            }
        }

        public string SimulateWeatherTransitions()
        {

            var currentRoll = _dice.Roll(1, 1, 100);
            
            var states = new List<string>()
            {
                "Sunny",
                "SunnyToCloudy",
                "Cloudy",
                "CloudyToSunny",
                "CloudyToRain",
                "LightRain",
                "HeavyRain",
                "ThunderStorm"

            };

            var sunnyStates = new List<Tuple<string, string>>()
            {
                  new Tuple<string, string>("It's a beautiful clear blue sky", "It's a beautiful clear night sky with twinkling stars"),
                new Tuple<string, string>("The sun blazes brightly in the sky", "The moon illuminates the night sky"),
            };


            var weatherText = string.Empty;

            if (currentRoll <= 10 && WeatherState == "Sunny")
            {
                WeatherState = "SunnyToCloudy";
            }

            if (WeatherState == "Cloudy")
            {
                weatherText = _time.IsNightTime() ? CloudyStates[0].Item1 : CloudyStates[0].Item2;


                if (currentRoll <= 70)
                {
                    WeatherState = "Cloudy";
                }

                if (currentRoll > 70 && currentRoll < 85)
                {
                    WeatherState = "CloudyToSunny";
                }

                if (currentRoll >= 85 && currentRoll <= 100)
                {
                    WeatherState = "CloudyToRain";
                }

                return weatherText;
            }

            if (WeatherState == "LightRain")
            {
                weatherText = _time.IsNightTime() ? LightRainState[_dice.Roll(1, 0, 1)].Item1 : LightRainState[_dice.Roll(1, 0, 1)].Item2;

                if (currentRoll <= 25)
                {
                    WeatherState = "RainToCloudy";
                }

                if (currentRoll >= 75 )
                {
                    WeatherState = "RainToHeavyRain";
                }


                return weatherText;
            }

            if (WeatherState == "HeavyRain")
            {
                weatherText = _time.IsNightTime() ? HeavyRainState[_dice.Roll(1, 0, 1)].Item1 : HeavyRainState[_dice.Roll(1, 0, 1)].Item2;

                if (currentRoll <= 45)
                {
                    WeatherState = "HeavyToLightRain";
                }

                if (currentRoll >= 75)
                {
                    WeatherState = "HeavyRainToThunder";
                }


                return weatherText;
            }

            if (WeatherState == "Thunder")
            {
                weatherText = _time.IsNightTime() ? ThunderState[_dice.Roll(1, 0, 2)].Item1 : ThunderState[_dice.Roll(1, 0, 2)].Item2;

                if (currentRoll <= 35)
                {
                    WeatherState = "ThunderToLightRain";
                }


                return weatherText;
            }



            // transitions


            if (WeatherState == "SunnyToCloudy")
            {
               weatherText = WeatherTransition("Cloudy", sunnyToCloudyTransitionStates, ref SunnyToCloudyTransitionState);

                return weatherText;
            }
            if (WeatherState == "CloudyToSunny")
            {

                weatherText = WeatherTransition("Sunny", cloudyToSunnyTransitionStates, ref CloudyToSunnyTransitionState);

                return weatherText;
            }

            if (WeatherState == "CloudyToRain")
            {
                weatherText = WeatherTransition("LightRain", cloudyToRainyTransitionStates, ref CloudyToLightRainTransitionState);

                return weatherText;
            }

            if (WeatherState == "RainToCloudy")
            {
 
                weatherText = WeatherTransition("Cloudy", LightRainToCloudTransitionStates, ref LightRainToCloudyTransitionState);

                return weatherText;
            }

            if (WeatherState == "RainToHeavyRain")
            {

                weatherText = WeatherTransition("HeavyRain", LightRainToHeavyRainTransitionStates, ref LightRainToHeavyRainTransitionState);

                return weatherText;
            }

            if (WeatherState == "HeavyToLightRain")
            {

                weatherText = WeatherTransition("LightRain", HeavyRainToLightRainTransitionStates, ref HeavyRainToLightRainTransitionState);

                return weatherText;
            }

            if (WeatherState == "HeavyRainToThunder")
            {

                weatherText = WeatherTransition("Thunder", HeavyRainToThunderTransitionStates, ref HeavyRainToThunderTransitionState);

                return weatherText;
            }

            if (WeatherState == "ThunderToLightRain")
            {

                weatherText = WeatherTransition("LightRain", ThunderToLightRainTransitionStates, ref ThunderToLightRainTransitionState);

                return weatherText;
            }


            return _time.IsNightTime() ? "It's a beautiful clear blue sky" : "It's a beautiful clear night sky";

        }

        public string WeatherTransition(string NewState, List<Tuple<string, string>> transitions, ref int transitionCount)
        {
            var weatherText = _time.IsNightTime() ? transitions[transitionCount].Item1 : transitions[transitionCount].Item2;
           transitionCount += 1;

            if (transitionCount > transitions.Count - 1)
            {
                transitionCount = 0;
                WeatherState = NewState;
            }

            return weatherText;
        }

        //public string AutumnWeatherTransitions()
        //{

        //    var currentRoll = _dice.Roll(1, 1, 100);
        //    var state = "clearSkies";
            
        //    var lastRoll = this.LastRoll;

        //    if (LastRoll == 0)
        //    {
        //        this.LastRoll = currentRoll; 
        //    }

        //    var weatherStates = new List<Tuple<string, string>>()
        //    {
        //        "clearSkies",
        //        "ClearToCloudy",
        //        "CloudyToClear",
        //        "CloudyToRain",
        //    };


        //    var weatherGoodToBad = new List<Tuple<string, string>>()
        //    {
        //        "It's a beautiful clear blue sky",
        //        "The sky is blue with a few wispy clouds",
        //        "The sky begins to get more cloudy.",
        //        "More clouds roll in creating a blanket over the sky.",
        //        "The clouds cover the sky.",
        //        "The clouds appear darker in the sky",
        //        "Light rain patters on the ground around you.",
        //        "The light rain picks up a bit.",
        //        "The rain falls steadily forming small puddles here and there",
        //    };

        //    var weatherBadToGood = new List<Tuple<string, string>>()
        //    {
        //        "It's a beautiful clear blue sky",
        //        "A few of the clouds begin to move out leaving only a few clouds left behind.",
        //        "Some of the clouds begin to break.",
        //        "The clouds appear lighter and brighter",
        //         "The clouds cover the sky.",
        //         "The rain stops",
        //         "The rain reduces to a drizzle",
        //         "The rain slows to a light rain",
        //        "The rain falls steadily",


        //    };



        //    //var weatherBadToGood = new List<Tuple<string, string>>()
        //    //{
        //    //    "The rain falls steadily",
        //    //    "The rain slows to a light rain",
        //    //    "The rain reduces to a drizzle",
        //    //    "The rain stops",
        //    //    "The clouds cover the sky.",
        //    //    "The clouds appear lighter and brighter",
        //    //    "Some of the clouds begin to break.",
        //    //    "A few of the clouds begin to move out leaving only a few clouds left behind.",
        //    //    "A beautiful clear blue sky",
        //    //};


        //    //var CloudTransitionToLightRain = new List<Tuple<string, string>>()
        //    //{
        //    //    "The clouds cover the sky.",
        //    //    "The clouds appear darker in the sky",
        //    //    "Light rain patters on the ground around you.",
        //    //    "The light rain picks up a bit.",
        //    //    "The rain falls steadily forming small puddles here and there",
        //    //};


        //    //var CloudTransitionToClearSky = new List<Tuple<string, string>>()
        //    //{
        //    //    "The clouds cover the sky.",
        //    //    "The clouds appear lighter and brighter",
        //    //    "Some of the clouds begin to break.",
        //    //    "A few of the clouds begin to move out leaving only a few clouds left behind.",
        //    //    "A beautiful clear blue sky"
        //    //};

        //    //var ClearSkyTransitionToCloundy = new List<Tuple<string, string>>()
        //    //{
        //    //    "A beautiful clear blue sky",
        //    //    "The sky is blue with a few wispy clouds",
        //    //    "The sky begins to get more cloudy.",
        //    //    "More clouds roll in creating a blanket over the sky."
        //    //};


        //    if (currentRoll > lastRoll)
        //    {
        //        this.LastRoll = currentRoll;
        //        var weatherText = weatherBadToGood[Math.Abs(this.weatherGoodToBadPos)];
        //        this.weatherGoodToBadPos += 1;
        //        weatherBadToGoodPos -= 1;

        //        if (this.weatherGoodToBadPos >= 8)
        //        {
        //            this.weatherGoodToBadPos = 8;
        //        }

        //        if (this.weatherBadToGoodPos <= 0)
        //        {
        //            this.weatherBadToGoodPos =0;
        //        }



        //        return weatherText;
        //    }

        //    if (currentRoll < lastRoll)
        //        {
        //            this.LastRoll = currentRoll;
        //        var weather = weatherGoodToBad[Math.Abs(weatherBadToGoodPos)];
        //        weatherBadToGoodPos += 1;
        //        this.weatherGoodToBadPos -= 1;

        //        if (weatherBadToGoodPos >= 8)
        //        {
        //            weatherBadToGoodPos = 8;
        //        }

        //        if (weatherGoodToBadPos <= 0)
        //        {
        //            weatherGoodToBadPos = 0;
        //        }

        //        return weather;
        //        }
 

        //    return weatherGoodToBad[0];
        //}
 
    }
}
