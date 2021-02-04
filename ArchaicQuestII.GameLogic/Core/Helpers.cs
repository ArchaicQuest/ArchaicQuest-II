using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Skill.Enum;
using ArchaicQuestII.GameLogic.World.Room;

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


    }


}
