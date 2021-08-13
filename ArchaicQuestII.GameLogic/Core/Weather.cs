using System;
using System.Collections.Generic;
using System.Text;

namespace ArchaicQuestII.GameLogic.Core
{
   public class Weather:IWeather {

       private IDice _dice;
       public int LastRoll = 0;
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
                "The rain falls steadily",
                "The rain slows to a light rain",
                "The rain reduces to a drizzle",
                "The rain stops",
                "The clouds cover the sky.",
                "The clouds appear lighter and brighter",
                "Some of the clouds begin to break.",
                "A few of the clouds begin to move out leaving only a few clouds left behind.",
                "A beautiful clear blue sky",
            };


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

                var weatherText = weatherBadToGood[Math.Abs(this.weatherGoodToBadPos)];
                this.weatherGoodToBadPos += 1;
                 

                if (this.weatherGoodToBadPos >= 8)
                {
                    this.weatherGoodToBadPos = 8;
                }



                return weatherText;
            }

            if (currentRoll < lastRoll)
                {
                   
                weatherBadToGoodPos += 1;
              
                if (weatherBadToGoodPos >= 8)
                {
                    weatherBadToGoodPos = 8;
                }
                
                return weatherGoodToBad[Math.Abs(weatherBadToGoodPos)];
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
