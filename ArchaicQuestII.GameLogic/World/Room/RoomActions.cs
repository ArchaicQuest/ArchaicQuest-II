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
        public void Look(string target, Room room, Player player)
        {

            if (!string.IsNullOrEmpty(target) && target != "look" || !string.IsNullOrEmpty(target) && target != "l")
            {
                LookObject(target, room, player);
                return;
            }

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

        public void LookInContainer(string target, Room room, Player player)
        {

            //check room, then check player if no match

 
            var container = room.Items.FirstOrDefault(x => x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase)) ?? player.Inventory.FirstOrDefault(x => x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase));

            if (container != null && container.ItemType != Item.Item.ItemTypes.Container)
            {
                _writeToClient.WriteLine($"<p>{container.Name} is not a container", player.ConnectionId);
                return;
            }

            if (container == null)
            {
                _writeToClient.WriteLine("<p>You don't see that here.", player.ConnectionId);
                return;
            }

            if (container.Container.IsOpen == false)
            {
                _writeToClient.WriteLine("<p>You need to open it first..", player.ConnectionId);
                return;
            }

            _writeToClient.WriteLine($"<p>You look inside {container.Name.ToLower()}:</p>", player.ConnectionId);
            foreach (var obj in container.Container.Items.List(false))
            {
                _writeToClient.WriteLine($"<p>{obj}</p>", player.ConnectionId);
            }

        }

        public void LookObject(string target, Room room, Player player)
        {

            var item = room.Items.FirstOrDefault(x => x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase)) ?? player.Inventory.FirstOrDefault(x => x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase));

            if (item == null)
            {
                _writeToClient.WriteLine("<p>You don't see that here.", player.ConnectionId);
                return;
            }

            _writeToClient.WriteLine($"<p>{item.Description.Look}", player.ConnectionId);

            if (item.ItemType == Item.Item.ItemTypes.Container)
            {
                _writeToClient.WriteLine($"<p>You look inside {item.Name}", player.ConnectionId);
                foreach (var obj in item.Container.Items.List(false))
                {
                    _writeToClient.WriteLine($"<p>{obj}</p>", player.ConnectionId);
                }
            }
        }

        public void ExamineObject(string target, Room room, Player player)
        {

            var item = room.Items.FirstOrDefault(x => x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase)) ?? player.Inventory.FirstOrDefault(x => x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase));

            if (item == null)
            {
                _writeToClient.WriteLine("<p>You don't see that here.", player.ConnectionId);
                return;
            }

            _writeToClient.WriteLine($"<p>{item.Description.Exam}", player.ConnectionId);

            if (item.ItemType == Item.Item.ItemTypes.Container)
            {
                _writeToClient.WriteLine($"<p>You look inside {item.Name}", player.ConnectionId);
                foreach (var obj in item.Container.Items.List(false))
                {
                    _writeToClient.WriteLine($"<p>{obj}</p>", player.ConnectionId);
                }
            }
        }

        public void SmellObject(string target, Room room, Player player)
        {

            var item = room.Items.FirstOrDefault(x => x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase)) ?? player.Inventory.FirstOrDefault(x => x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase));

            if (item == null)
            {
                _writeToClient.WriteLine("<p>You don't see that here.", player.ConnectionId);
                return;
            }

            _writeToClient.WriteLine($"<p>{item.Description.Smell}", player.ConnectionId);
        }

        public void TasteObject(string target, Room room, Player player)
        {

            var item = room.Items.FirstOrDefault(x => x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase)) ?? player.Inventory.FirstOrDefault(x => x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase));

            if (item == null)
            {
                _writeToClient.WriteLine("<p>You don't see that here.", player.ConnectionId);
                return;
            }

            _writeToClient.WriteLine($"<p>{item.Description.Taste}", player.ConnectionId);

        }

        public void TouchObject(string target, Room room, Player player)
        {

            var item = room.Items.FirstOrDefault(x => x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase)) ?? player.Inventory.FirstOrDefault(x => x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase));

            if (item == null)
            {
                _writeToClient.WriteLine("<p>You don't see that here.", player.ConnectionId);
                return;
            }

            _writeToClient.WriteLine($"<p>{item.Description.Touch}", player.ConnectionId);

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
