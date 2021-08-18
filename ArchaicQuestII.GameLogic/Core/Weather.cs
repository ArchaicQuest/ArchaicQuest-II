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
       public int ThunderToHeavyRainTransitionState = 0;

        public List<string> sunnyToCloudyTransitionStates = new List<string>()
       {
           "The sky is blue with a few wispy clouds.",
           "The sky begins to get more cloudy.",
           "More clouds roll in creating a blanket over the sky."
       };

       public List<string> cloudyToSunnyTransitionStates = new List<string>()
       {
           "The clouds appear lighter and brighter",
           "Some of the clouds begin to break.",
           "A few of the clouds begin to move out leaving only a few clouds left behind."
       };

       public List<string> CloudyStates = new List<string>()
       {
           "Clouds cover the sky",

       };

       public List<string> cloudyToRainyTransitionStates = new List<string>()
       {
           "The clouds appear darker in the sky.",
           "Light rain patters on the ground around you.",
           "The light rain picks up a bit."
       };

       public List<string> LightRainToCloudTransitionStates = new List<string>()
       {
           "The rain slows to a light rain.",
           "The rain reduces to a drizzle.",
           "The rain stops."
       };


       public List<string> LightRainState = new List<string>()
       {
           "The rain falls steady.",
           "The rain falls steadily forming small puddles here and there.",
       };


       public List<string> LightRainToHeavyRainTransitionStates = new List<string>()
       {
           "The rain begins to fall heavily.",
           "The rain falls heavily forming puddles here and there.",
           "The rain continues to fall heavily."
       };

       public List<string> HeavyRainState = new List<string>()
       {
           "The rain falls heavily.",
           "The rain continues to fall heavily.",
       };

       public List<string> HeavyRainToLightRainTransitionStates = new List<string>()
       {
           "The rain begins to slow down.",
           "The rain no longer pounds the ground and lessens some what.",
           "The rain slows to a light rain."
       };

       public List<string> HeavyRainToThunderTransitionStates = new List<string>()
       {
           "The sound of thunder rumbles in the distance.",
           "Lightning flashes in the sky, accompanied shortly by booming thunder.",
           "Lightning forks across the sky, followed by a bang of thunder.",
           
       };

       public List<string> ThunderState = new List<string>()
       {
           "Lightning forks across the sky, followed by a bang of thunder.",
           "The rain pours down on the ground. Lightning and thunder light up the sky and shake the ground.",
           "Thunder cracks, and lightning flashes in the sky as the heavy rain continues to fall."
       };

       public List<string> ThunderToLightRainTransitionStates = new List<string>()
       {
           "Lightning forks across the sky, followed by a bang of thunder. The rain starts to ease.",
           "A flash of lighting then a long pause before the rumble of thunder is heard.",
           "The rain is no longer so heavy, the only sound of thunder is off in the distance.",

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
        public Weather(IDice dice)
        {
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

            var sunnyStates = new List<string>()
            {
                "A beautiful clear blue sky",
                "The sun blazes brightly in the sky",
            };


            var weatherText = string.Empty;

            if (currentRoll <= 10 && WeatherState == "Sunny")
            {
                WeatherState = "SunnyToCloudy";
            }

            if (WeatherState == "Cloudy")
            {
                weatherText = CloudyStates[0];
                 
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
                weatherText = LightRainState[_dice.Roll(1,0,1)];

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
                weatherText = HeavyRainState[_dice.Roll(1, 0, 1)];

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
                weatherText = ThunderState[_dice.Roll(1, 0, 2)];

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


            return "A beautiful clear blue sky";

        }

        public string WeatherTransition(string NewState, List<string> transitions, ref int transitionCount)
        {
            var weatherText = transitions[transitionCount]; 
           transitionCount += 1;

            if (transitionCount > transitions.Count - 1)
            {
                transitionCount = 0;
                WeatherState = NewState;
            }

            return weatherText;
        }

        public string AutumnWeatherTransitions()
        {

            var currentRoll = _dice.Roll(1, 1, 100);
            var state = "clearSkies";
            
            var lastRoll = this.LastRoll;

            if (LastRoll == 0)
            {
                this.LastRoll = currentRoll;
            }

            var weatherStates = new List<string>()
            {
                "clearSkies",
                "ClearToCloudy",
                "CloudyToClear",
                "CloudyToRain",
            };


            var weatherGoodToBad = new List<string>()
            {
                "A beautiful clear blue sky",
                "The sky is blue with a few wispy clouds",
                "The sky begins to get more cloudy.",
                "More clouds roll in creating a blanket over the sky.",
                "The clouds cover the sky.",
                "The clouds appear darker in the sky",
                "Light rain patters on the ground around you.",
                "The light rain picks up a bit.",
                "The rain falls steadily forming small puddles here and there",
            };

            var weatherBadToGood = new List<string>()
            {
                "A beautiful clear blue sky",
                "A few of the clouds begin to move out leaving only a few clouds left behind.",
                "Some of the clouds begin to break.",
                "The clouds appear lighter and brighter",
                 "The clouds cover the sky.",
                 "The rain stops",
                 "The rain reduces to a drizzle",
                 "The rain slows to a light rain",
                "The rain falls steadily",


            };



            //var weatherBadToGood = new List<string>()
            //{
            //    "The rain falls steadily",
            //    "The rain slows to a light rain",
            //    "The rain reduces to a drizzle",
            //    "The rain stops",
            //    "The clouds cover the sky.",
            //    "The clouds appear lighter and brighter",
            //    "Some of the clouds begin to break.",
            //    "A few of the clouds begin to move out leaving only a few clouds left behind.",
            //    "A beautiful clear blue sky",
            //};


            //var CloudTransitionToLightRain = new List<string>()
            //{
            //    "The clouds cover the sky.",
            //    "The clouds appear darker in the sky",
            //    "Light rain patters on the ground around you.",
            //    "The light rain picks up a bit.",
            //    "The rain falls steadily forming small puddles here and there",
            //};


            //var CloudTransitionToClearSky = new List<string>()
            //{
            //    "The clouds cover the sky.",
            //    "The clouds appear lighter and brighter",
            //    "Some of the clouds begin to break.",
            //    "A few of the clouds begin to move out leaving only a few clouds left behind.",
            //    "A beautiful clear blue sky"
            //};

            //var ClearSkyTransitionToCloundy = new List<string>()
            //{
            //    "A beautiful clear blue sky",
            //    "The sky is blue with a few wispy clouds",
            //    "The sky begins to get more cloudy.",
            //    "More clouds roll in creating a blanket over the sky."
            //};


            if (currentRoll > lastRoll)
            {
                this.LastRoll = currentRoll;
                var weatherText = weatherBadToGood[Math.Abs(this.weatherGoodToBadPos)];
                this.weatherGoodToBadPos += 1;
                weatherBadToGoodPos -= 1;

                if (this.weatherGoodToBadPos >= 8)
                {
                    this.weatherGoodToBadPos = 8;
                }

                if (this.weatherBadToGoodPos <= 0)
                {
                    this.weatherBadToGoodPos =0;
                }



                return weatherText;
            }

            if (currentRoll < lastRoll)
                {
                    this.LastRoll = currentRoll;
                var weather = weatherGoodToBad[Math.Abs(weatherBadToGoodPos)];
                weatherBadToGoodPos += 1;
                this.weatherGoodToBadPos -= 1;

                if (weatherBadToGoodPos >= 8)
                {
                    weatherBadToGoodPos = 8;
                }

                if (weatherGoodToBadPos <= 0)
                {
                    weatherGoodToBadPos = 0;
                }

                return weather;
                }
 

            return weatherGoodToBad[0];
        }

        public string WeatherEffectAutumn(int value)
        {
            var currentState = "";

            switch (value)
            {
                case 1:
                case 2:
                    return "Thunderstorm";
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                    return currentState == "rain" ? "Snow" : "Rain";
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                case 19:
                case 20:
                    return "Heavy Clouds";
                case 21:
                case 22:
                case 23:
                case 24:
                case 25:
                case 26:
                case 27:
                case 28:
                case 29:
                case 30:
                case 31:
                case 32:
                case 33:
                case 34:
                case 35:
                case 36:
                case 37:
                case 38:
                case 39:
                case 40:
                case 41:
                case 42:
                case 43:
                case 44:
                case 45:
                case 46:
                case 47:
                case 48:
                case 49:
                case 50:
                    return "Light Clouds";
                case 51:
                case 52:
                case 53:
                case 54:
                case 55:
                case 56:
                case 57:
                case 58:
                case 59:
                case 60:
                case 61:
                case 62:
                case 63:
                case 64:
                case 65:
                case 66:
                case 67:
                case 68:
                case 69:
                case 70:
                    return "Clear skies";
                case 71:
                case 72:
                case 73:
                case 74:
                case 75:
                case 76:
                case 77:
                case 78:
                case 79:
                case 80:
                case 81:
                case 82:
                case 83:
                case 84:
                case 85:
                case 86:
                case 87:
                case 88:
                case 89:
                case 90:
                    return "High Winds";
                case 96:
                case 97:
                case 98:
                case 99:
                    return "Warm";
                case 100:
                    return "Scorching Heat";
                default:
                    return "clear skies";
            }

            return "";
        }
    }
}
