using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;

namespace ArchaicQuestII.GameLogic.World.Room
{
    public class RoomActions
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

            var roomDesc = new StringBuilder();

            roomDesc
                .Append($"<p class=\"room-title\">{room.Title}<br /></p>")
                .Append($"<p class=\"room-description\">{room.Description}</p>")
                .Append($"<p>{items}</p>")
                .Append($"<p>{mobs}</p>")
                .Append($"<p class=\"room-exits\">[Exits: {exits} ]</p>");

           _writeToClient.WriteLine(roomDesc.ToString(), player.ConnectionId);
        }

        public string DisplayItems(Room room)
        {
            var items = string.Empty;

            foreach (var item in room.Items)
            {
                items += item.Name + " lies here.";
            }

            return items;

        }

        public string DisplayMobs(Room room)
        {
            var mobs = string.Empty;

            foreach (var mob in room.Mobs)
            {
                mobs += mob.Name + " is here.";
            }

            return mobs;

        }


        /// <summary>
        /// Displays valid exits
        /// </summary>
        public string FindValidExits(Room room)
        {
            var exits = String.Empty;

            if (room.Exits.NorthWest != null)
            {
                exits += "North West";
            }

            if (room.Exits.North != null)
            {
                exits += " North";
            }

            if (room.Exits.NorthEast != null)
            {
                exits += " North East";
            }

            if (room.Exits.East != null)
            {
                exits += " East";
            }

            if (room.Exits.SouthEast != null)
            {
                exits += " South East";
            }

            if (room.Exits.South != null)
            {
                exits += " South";
            }

            if (room.Exits.SouthWest != null)
            {
                exits += " South West";
            }

            if (room.Exits.West != null)
            {
                exits += " West";
            }

            if (string.IsNullOrEmpty(exits))
            {
                exits = "None";
            }
 

            return exits;

        }


    }
}
