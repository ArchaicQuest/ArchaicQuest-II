using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;

namespace ArchaicQuestII.GameLogic.World.Room
{
    public class RoomActions:IRoomActions
    {

        private readonly IWriteToClient _writeToClient;
        public RoomActions(IWriteToClient writeToClient)
        {
            _writeToClient = writeToClient;
        }
        /// <summary>
        /// Displays current room 
        /// </summary>
        public void Look(Room room, Player player)
        {

            var exits = FindValidExits(room);
            var items = DisplayItems(room);
            var mobs = DisplayMobs(room);
            var players = DisplayPlayers(room, player);

            var roomDesc = new StringBuilder();

            roomDesc
                .Append($"<p class=\"room-title\">{room.Title}<br /></p>")
                .Append($"<p class=\"room-description\">{room.Description}</p>")
                .Append(
                    $"<p class=\"room-exit\"> <span class=\"room-exits\">[</span>Exits: <span class=\"room-exits\">{exits}</span><span class=\"room-exits\">]</span></p>")
                .Append($"<p>{items}</p>")
                .Append($"<p>{mobs}</p>")
                .Append($"<p>{players}</p>");
               

           _writeToClient.WriteLine(roomDesc.ToString(), player.ConnectionId);
        }

        public string DisplayItems(Room room)
        {
            var items = room.Items.List();
            var x = string.Empty;
            foreach (var item in items)
            {
                if (!string.IsNullOrEmpty(item))
                {
                   
                        x += "<p class='item'>" + item + "</p>";
                }
             
            }

            return x;

        }

        public string DisplayMobs(Room room)
        {
            var mobs = string.Empty;

            foreach (var mob in room.Mobs)
            {
                if (!string.IsNullOrEmpty(mob.LongName))
                {
                    mobs += "<p class='mob'>" + mob.LongName + "</p>";
                }
                else
                {
                    mobs += "<p class='mob'>" + mob.Name + " is here.</p>";
                }
               
            }

            return mobs;

        }

        public string DisplayPlayers(Room room, Player player)
        {
            var players = string.Empty;

            foreach (var pc in room.Players)
            {
                if (pc.Name == player.Name)
                {
                    continue;
                }
                players += "<p class='player'>" + pc.Name + " is here.</p>";
            }

            return players;

        }


        /// <summary>
        /// Displays valid exits
        /// </summary>
        public string FindValidExits(Room room)
        {
            var exits = new List<string>();
            var exitList = string.Empty;

            if (room.Exits.NorthWest != null)
            {
                exits.Add(room.Exits.NorthWest.Name);
            }

            if (room.Exits.North != null)
            {
                exits.Add(room.Exits.North.Name);
            }

            if (room.Exits.NorthEast != null)
            {
                exits.Add(room.Exits.NorthEast.Name);
            }

            if (room.Exits.East != null)
            {
                exits.Add(room.Exits.East.Name);
            }

            if (room.Exits.SouthEast != null)
            {
                exits.Add(room.Exits.SouthEast.Name);
            }

            if (room.Exits.South != null)
            {
                exits.Add(room.Exits.South.Name);
            }

            if (room.Exits.SouthWest != null)
            {
                exits.Add(room.Exits.SouthWest.Name);
            }

            if (room.Exits.West != null)
            {
                exits.Add(room.Exits.West.Name);
            }

            if (exits.Count <= 0)
            {
                exits.Add("None");
            }

            foreach (var exit in exits)
            {
                exitList += exit + ", ";
            }

            exitList = exitList.Remove(exitList.Length - 2);


            return exitList;

        }


    }
}
