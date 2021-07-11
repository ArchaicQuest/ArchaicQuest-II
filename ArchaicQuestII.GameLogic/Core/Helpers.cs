using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Class;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Skill.Enum;
using ArchaicQuestII.GameLogic.World.Room;
using Newtonsoft.Json;

namespace ArchaicQuestII.GameLogic.Core
{
    /// <summary>
    /// Random functions to go here unless there is
    /// enough related random functions to move out to their own class
    /// </summary>
   public static class Helpers
    {
        public static Tuple<int, string> findNth(string command)
        {
            var splitCommand = command.Split('.');
           var isnumber = int.TryParse(splitCommand[0], out int nth);
            if (splitCommand[0].Equals(command) || !isnumber)
            {
                return new Tuple<int, string>(-1, command);
            }

           
            var target = splitCommand[1];

            return new Tuple<int, string>(nth, target);
        }


        public static Item.Item findRoomObject(Tuple<int, string> keyword, Room room)
        {
           return keyword.Item1 == -1 ? room.Items.FirstOrDefault(x => x.Name.Contains(keyword.Item2, StringComparison.CurrentCultureIgnoreCase)) :
                room.Items.FindAll(x => x.Name.Contains(keyword.Item2, StringComparison.CurrentCultureIgnoreCase)).Skip(keyword.Item1 - 1).FirstOrDefault();
        }

        public static Item.Item findObjectInInventory(Tuple<int, string> keyword, Player player)
        {
            return keyword.Item1 == -1 ? player.Inventory.FirstOrDefault(x => x.Name.Contains(keyword.Item2, StringComparison.CurrentCultureIgnoreCase)) :
                player.Inventory.FindAll(x => x.Name.Contains(keyword.Item2, StringComparison.CurrentCultureIgnoreCase)).Skip(keyword.Item1 - 1).FirstOrDefault();
        }

        /// <summary>
        /// Find matching player or mob
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="room"></param>
        /// <returns></returns>
        public static Player findPlayerObject(Tuple<int, string> keyword, Room room)
        {
            return keyword.Item1 == -1
                ? room.Players.FirstOrDefault(x =>
                    x.Name.Contains(keyword.Item2, StringComparison.CurrentCultureIgnoreCase))
                : room.Players.FindAll(x => x.Name.Contains(keyword.Item2, StringComparison.CurrentCultureIgnoreCase))
                    .Skip(keyword.Item1 - 1).FirstOrDefault() ?? (keyword.Item1 == null
                    ? room.Mobs.FirstOrDefault(x =>
                        x.Name.Contains(keyword.Item2, StringComparison.CurrentCultureIgnoreCase))
                    : room.Mobs
                        .FindAll(x => x.Name.Contains(keyword.Item2, StringComparison.CurrentCultureIgnoreCase))
                        .Skip(keyword.Item1 - 1).FirstOrDefault());
        }

        public static Player FindPlayer(Tuple<int, string> keyword, Room room)
        {
            return keyword.Item1 == -1
                ? room.Players.FirstOrDefault(x =>
                    x.Name.Contains(keyword.Item2, StringComparison.CurrentCultureIgnoreCase))
                : room.Players.FindAll(x => x.Name.Contains(keyword.Item2, StringComparison.CurrentCultureIgnoreCase))
                    .Skip(keyword.Item1 - 1).FirstOrDefault();
        }

        public static Player FindMob(Tuple<int, string> keyword, Room room)
        {

            return keyword.Item1 == -1
                ? room.Mobs.FirstOrDefault(x =>
                    x.Name.Contains(keyword.Item2, StringComparison.CurrentCultureIgnoreCase))
                : room.Mobs.FindAll(x => x.Name.Contains(keyword.Item2, StringComparison.CurrentCultureIgnoreCase))
                    .Skip(keyword.Item1 - 1).FirstOrDefault();
        }

        public static string ReturnRoomId(Room room)
        {
            return $"{room.AreaId}{room.Coords.X}{room.Coords.Y}{room.Coords.Z}";
        }

        public static SkillList FindSkill(string skillName, Player player)
        {

            return player.Skills.FirstOrDefault(x =>
                x.SkillName.Equals(skillName, StringComparison.CurrentCultureIgnoreCase));
        }


        /// <summary>
        /// Use to remove A / An from word
        /// </summary>
        /// <param name="word"></param>
        /// <returns> string</returns>
        public static string RemoveArticle(string word)
        {
            var wordWithOutArticle = word;
            var splitWord = word.Split(" ");

            if(splitWord[0].Equals("a", StringComparison.CurrentCultureIgnoreCase) || splitWord[0].Equals("an", StringComparison.CurrentCultureIgnoreCase))
            {
                wordWithOutArticle = word.Substring(word.IndexOf(" ") + 1);
            }

            return wordWithOutArticle;
        }

        /// <summary>
        /// Adds A / An to word, should work in most cases as
        /// we are dealing with nouns but this doesn't work for
        /// words like hour where it should be An Hour and not a
        /// hour.
        /// </summary>
        /// <param name="word"></param>
        /// <returns> string</returns>
        public static string AddArticle(string word)
        {

            var startsWithVowel = "aeiou".Contains(word[0], StringComparison.CurrentCultureIgnoreCase);
            var newWord = string.Empty;

            if(startsWithVowel)
            {
                newWord = "An " + word;
            }
            else
            {
                newWord = "A " + word;
            }
            return newWord;
        }

        /// <summary>
        /// Her / His
        /// </summary>
        /// <param name="gender"></param>
        /// <returns></returns>
        public static string GetPronoun(string gender)
        {
            return gender switch
            {
                "Female" => "her",
                "Male" => "his",
                _ => "their",
            };
        }
        /// <summary>
        /// She / He
        /// </summary>
        /// <param name="gender"></param>
        /// <returns></returns>
        public static string GetSubjectPronoun(string gender)
        {
            return gender switch
            {
                "Female" => "she",
                "Male" => "he",
                _ => "it",
            };
        }

        /// <summary>
        /// Her / Him
        /// </summary>
        /// <param name="gender"></param>
        /// <returns></returns>
        public static string GetObjectPronoun(string gender)
        {
            return gender switch
            {
                "Female" => "her",
                "Male" => "him",
                _ => "it",
            };
        }

        public static bool isCaster(string classname)
        {
            return classname switch
            {
                "Mage" => true,
                "Cleric" => true,
                "Druid" => true,
                _ => false,
            };
        }

        public static string DisplayDoor(Exit exit)
        {
            var exitName = "";
            if (exit.Door && exit.Closed)
            {
                exitName = $"[{exit.Name}]";
            }
            else
            {
                exitName = exit.Name;
            }
            return exitName;
        }

        public static Exit IsExit(string exit, Room room)
        {
            if (room.Exits.North != null && room.Exits.North.Name.StartsWith(exit, StringComparison.CurrentCultureIgnoreCase))
            {
                return room.Exits.North;
            }

            if (room.Exits.East != null && room.Exits.East.Name.StartsWith(exit, StringComparison.CurrentCultureIgnoreCase))
            {
                return room.Exits.East;
            }

            if (room.Exits.South != null && room.Exits.South.Name.StartsWith(exit, StringComparison.CurrentCultureIgnoreCase))
            {
                return room.Exits.South;
            }

            if (room.Exits.West != null && room.Exits.West.Name.StartsWith(exit, StringComparison.CurrentCultureIgnoreCase))
            {
                return room.Exits.West;
            }

            if (room.Exits.NorthWest != null && room.Exits.NorthWest.Name.StartsWith(exit, StringComparison.CurrentCultureIgnoreCase))
            {
                return room.Exits.NorthWest;
            }

            if (room.Exits.NorthEast != null && room.Exits.NorthEast.Name.StartsWith(exit, StringComparison.CurrentCultureIgnoreCase))
            {
                return room.Exits.NorthEast;
            }

            if (room.Exits.SouthEast != null && room.Exits.SouthEast.Name.StartsWith(exit, StringComparison.CurrentCultureIgnoreCase))
            {
                return room.Exits.SouthEast;
            }
            if (room.Exits.SouthWest != null && room.Exits.SouthWest.Name.StartsWith(exit, StringComparison.CurrentCultureIgnoreCase))
            {
                return room.Exits.SouthWest;
            }
            if (room.Exits.Down != null && room.Exits.Down.Name.StartsWith(exit, StringComparison.CurrentCultureIgnoreCase))
            {
                return room.Exits.Down;
            }
            if (room.Exits.Up != null && room.Exits.Up.Name.StartsWith(exit, StringComparison.CurrentCultureIgnoreCase))
            {
                return room.Exits.Up;
            }

            return null;
        }

        public static List<string> GetListOfExits(ExitDirections exits)
        {
            var exitList = new List<string>();

            if (exits.North != null)
            {
                exitList.Add(exits.North.Name);
            }

            if (exits.East != null)
            {
                exitList.Add(exits.East.Name);
            }

            if (exits.South != null)
            {
                exitList.Add(exits.South.Name);
            }

            if (exits.West != null)
            {
                exitList.Add(exits.West.Name);
            }

            if (exits.Up != null)
            {
                exitList.Add(exits.Up.Name);
            }

            if (exits.Down != null)
            {
                exitList.Add(exits.Down.Name);
            }

            return exitList;
        }

        public static async void PostToDiscord(string botToSay, string eventName, Config config)
        {
            if (!config.PostToDiscord)
            {
                return;
            }
            var client = new HttpClient();
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("content", botToSay)
            });

            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            switch (eventName)
            {
                case "event":
                    await client.PostAsync(config.EventsDiscordWebHookURL, content);
                    break;
                case "channels":
                    await client.PostAsync(config.ChannelDiscordWebHookURL, content);
                    break;
                case "error":
                    await client.PostAsync(config.ErrorDiscordWebHookURL, content);
                    break;
                default:
                    break;
            }

            client.Dispose();
        }


        public static int GetPercentage(int percentage, int value)
        {
            if (percentage == 0)
            {
                return percentage;
            }

            return (int)percentage * value / 100;
        }

 



    }


}
