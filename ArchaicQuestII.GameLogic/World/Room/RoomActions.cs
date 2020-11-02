using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using ArchaicQuestII.Core.World;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;

namespace ArchaicQuestII.GameLogic.World.Room
{
    public class RoomActions : IRoomActions
    {

        private readonly IWriteToClient _writeToClient;
        private readonly ITime _time;
        public RoomActions(IWriteToClient writeToClient, ITime time)
        {
            _writeToClient = writeToClient;
            _time = time;
        }
        /// <summary>
        /// Displays current room 
        /// </summary>
        public void Look(string target, Room room, Player player)
        {
            if (player.Status == CharacterStatus.Status.Sleeping)
            {
                _writeToClient.WriteLine("You can't do that while asleep.", player.ConnectionId);
                return;
            }

            if (!string.IsNullOrEmpty(target) && !target.Equals("look", StringComparison.CurrentCultureIgnoreCase) && !string.IsNullOrEmpty(target) && !target.Equals("l", StringComparison.CurrentCultureIgnoreCase))
            {
                LookObject(target, room, player);
                return;
            }

            var exits = FindValidExits(room);
            var items = DisplayItems(room, player);
            var mobs = DisplayMobs(room, player);
            var players = DisplayPlayers(room, player);

            var roomDesc = new StringBuilder();
            var isDark = RoomIsDark(room, player);

            roomDesc
                .Append($"<p class=\"room-title {(isDark ? "room-dark" : "")}\">{room.Title}<br /></p>")
                .Append($"<p class=\"room-description  {(isDark ? "room-dark" : "")}\">{room.Description}</p>")
                .Append(
                    $"<p class=\"room-exit  {(isDark ? "room-dark" : "")}\"> <span class=\"room-exits\">[</span>Exits: <span class=\"room-exits\">{exits}</span><span class=\"room-exits\">]</span></p>")
                .Append($"<p  class=\" {(isDark ? "room-dark" : "")}\">{items}</p>")
                .Append($"<p  class=\"{(isDark ? "room-dark" : "")}\">{mobs}</p>")
                .Append($"<p  class=\"  {(isDark ? "room-dark" : "")}\">{players}</p>");


            _writeToClient.WriteLine(roomDesc.ToString(), player.ConnectionId);
        }

        public void LookInContainer(string target, Room room, Player player)
        {

            //check room, then check player if no match
            if (player.Status == CharacterStatus.Status.Sleeping)
            {
                _writeToClient.WriteLine("You can't do that while asleep.", player.ConnectionId);
                return;
            }



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
                _writeToClient.WriteLine("<p>You need to open it first.", player.ConnectionId);
                return;
            }

            _writeToClient.WriteLine($"<p>You look inside {container.Name.ToLower()}:</p>", player.ConnectionId);
            if (container.Container.Items.Count == 0)
            {
                _writeToClient.WriteLine($"<p>Nothing.</p>", player.ConnectionId);
            }

            var isDark = RoomIsDark(room, player);
            foreach (var obj in container.Container.Items.List(false))
            {
                _writeToClient.WriteLine($"<p class='{(isDark ? "room-dark" : "")}'>{obj}</p>", player.ConnectionId);
            }

           

            foreach (var pc in room.Players)
            {
                if (pc.Name == player.Name)
                {
                    continue;
                }

                _writeToClient.WriteLine($"<p>{player.Name} looks inside {container.Name.ToLower()}.</p>", pc.ConnectionId);
            }
        }

        public void LookObject(string target, Room room, Player player)
        {
            if (player.Status == CharacterStatus.Status.Sleeping)
            {
                _writeToClient.WriteLine("You can't do that while asleep.", player.ConnectionId);
                return;
            }
            var isDark = RoomIsDark(room, player);

            var item =
                room.Items.FirstOrDefault(x => x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase)) ??
                player.Inventory.FirstOrDefault(x =>
                    x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase));

            var character =
                room.Mobs.FirstOrDefault(x => x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase)) ??
                room.Players.FirstOrDefault(x => x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase));


            RoomObject roomObjects = null;
            if (room.RoomObjects.Count >= 1 && room.RoomObjects[0].Name != null)
            {
                 roomObjects =
                    room.RoomObjects.FirstOrDefault(x =>
                        x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase));
            }

            if (target.Equals("self", StringComparison.CurrentCultureIgnoreCase))
            {
                character = player;
            }


            if (item == null && character == null && roomObjects == null)
            {
                _writeToClient.WriteLine("<p>You don't see that here.", player.ConnectionId);
                return;
            }

            if (item != null && character == null && roomObjects == null)
            {
                _writeToClient.WriteLine($"<p  class='{(isDark ? "room-dark" : "")}'>{item.Description.Look}", player.ConnectionId);

                foreach (var pc in room.Players)
                {
                    if (pc.Name == player.Name)
                    {
                        continue;
                    }

                    _writeToClient.WriteLine($"<p>{player.Name} looks at {item.Name.ToLower()}.</p>", pc.ConnectionId);
                }

                return;
            }

            if (roomObjects != null && character == null && item == null)
            {
              
                _writeToClient.WriteLine($"<p class='{(isDark ? "room-dark" : "")}'>{roomObjects.Look}", player.ConnectionId);

                foreach (var pc in room.Players)
                {
                    if (pc.Name == player.Name)
                    {
                        continue;
                    }

                    _writeToClient.WriteLine($"<p>{player.Name} looks at {roomObjects.Name.ToLower()}.</p>", pc.ConnectionId);
                }

                return;
            }

            if (character == null)
            {
                _writeToClient.WriteLine("<p>You don't see them here.", player.ConnectionId);
                return;
            }

        

            var sb = new StringBuilder();
            if (character.ConnectionId != "mob")
            {
                sb.Append(
                    $"<table class='char-look'><tr><td><span class='cell-title'>Eyes:</span> {character.Eyes}</td><td><span class='cell-title'>Hair:</span> {character.HairColour}</td></tr>");
                sb.Append(
                    $"<tr><td><span class='cell-title'>Skin:</span> {character.Skin}</td><td><span class='cell-title'>Hair Length:</span> {character.HairLength}</td></tr>");
                sb.Append(
                    $"<tr><td><span class='cell-title'>Build:</span> {character.Build}</td><td><span class='cell-title'>Hair Texture:</span> {character.HairTexture}</td></tr>");
                sb.Append(
                    $"<tr><td><span class='cell-title'>Face:</span> {character.Face}</td><td><span class='cell-title'>Hair Facial:</span> {character.FacialHair}</td></tr><table>");
            }

            _writeToClient.WriteLine($"{sb}<p class='{(isDark ? "room-dark" : "")}'>{character.Description}", player.ConnectionId);

            foreach (var pc in room.Players)
            {
                if (pc.Name == player.Name)
                {
                    continue;
                }

                _writeToClient.WriteLine($"<p>{player.Name} looks at {character.Name.ToLower()}.</p>", pc.ConnectionId);
            }




            //if (item.ItemType == Item.Item.ItemTypes.Container)
            //{
            //  LookInContainer(target, room, player);
            //}
        }

        public void ExamineObject(string target, Room room, Player player)
        {
            if (player.Status == CharacterStatus.Status.Sleeping)
            {
                _writeToClient.WriteLine("You can't do that while asleep.", player.ConnectionId);
                return;
            }


            var item = room.Items.FirstOrDefault(x => x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase)) ?? player.Inventory.FirstOrDefault(x => x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase));

            if (item == null)
            {
                _writeToClient.WriteLine("<p>You don't see that here.", player.ConnectionId);
                return;
            }

            var isDark = RoomIsDark(room, player);

            _writeToClient.WriteLine($"<p class='{(isDark ? "room-dark" : "")}'>{item.Description.Exam}", player.ConnectionId);

            foreach (var pc in room.Players)
            {
                if (pc.Name == player.Name)
                {
                    continue;
                }

                _writeToClient.WriteLine($"<p>{player.Name} examines {item.Name.ToLower()}.</p>", pc.ConnectionId);
            }

            //if (item.ItemType == Item.Item.ItemTypes.Container)
            //{
            //    _writeToClient.WriteLine($"<p>You look inside {item.Name}", player.ConnectionId);
            //    foreach (var obj in item.Container.Items.List(false))
            //    {
            //        _writeToClient.WriteLine($"<p>{obj}</p>", player.ConnectionId);
            //    }
            //}
        }

        public void SmellObject(string target, Room room, Player player)
        {
            if (player.Status == CharacterStatus.Status.Sleeping)
            {
                _writeToClient.WriteLine("You can't do that while asleep.", player.ConnectionId);
                return;
            }


            var item = room.Items.FirstOrDefault(x => x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase)) ?? player.Inventory.FirstOrDefault(x => x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase));

            if (item == null)
            {
                _writeToClient.WriteLine("<p>You don't see that here.", player.ConnectionId);
                return;
            }
            var isDark = RoomIsDark(room, player);

            _writeToClient.WriteLine($"<p class='{(isDark ? "room-dark" : "")}'>{item.Description.Smell}", player.ConnectionId);

            foreach (var pc in room.Players)
            {
                if (pc.Name == player.Name)
                {
                    continue;
                }

                _writeToClient.WriteLine($"<p>{player.Name} smells {item.Name.ToLower()}.</p>", pc.ConnectionId);
            }
        }

        public void TasteObject(string target, Room room, Player player)
        {
            if (player.Status == CharacterStatus.Status.Sleeping)
            {
                _writeToClient.WriteLine("You can't do that while asleep.", player.ConnectionId);
                return;
            }

            var item = room.Items.FirstOrDefault(x => x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase)) ?? player.Inventory.FirstOrDefault(x => x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase));

            if (item == null)
            {
                _writeToClient.WriteLine("<p>You don't see that here.", player.ConnectionId);
                return;
            }

            var isDark = RoomIsDark(room, player);

            _writeToClient.WriteLine($"<p class='{(isDark ? "room-dark" : "")}'>{item.Description.Taste}", player.ConnectionId);


            foreach (var pc in room.Players)
            {
                if (pc.Name == player.Name)
                {
                    continue;
                }

                _writeToClient.WriteLine($"<p>{player.Name} tastes {item.Name.ToLower()}.</p>", pc.ConnectionId);
            }

        }

        public void TouchObject(string target, Room room, Player player)
        {
            if (player.Status == CharacterStatus.Status.Sleeping)
            {
                _writeToClient.WriteLine("You can't do that while asleep.", player.ConnectionId);
                return;
            }


            var item = room.Items.FirstOrDefault(x => x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase)) ?? player.Inventory.FirstOrDefault(x => x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase));

            if (item == null)
            {
                _writeToClient.WriteLine("<p>You don't see that here.", player.ConnectionId);
                return;
            }

            var isDark = RoomIsDark(room, player);

            _writeToClient.WriteLine($"<p class='{(isDark ? "room-dark" : "")}'>{item.Description.Touch}", player.ConnectionId);

            foreach (var pc in room.Players)
            {
                if (pc.Name == player.Name)
                {
                    continue;
                }

                _writeToClient.WriteLine($"<p>{player.Name} feels {item.Name.ToLower()}.</p>", pc.ConnectionId);
            }

        }

        public string DisplayItems(Room room, Player player)
        {
            var isDark = RoomIsDark(room, player);
            var items = room.Items.List();
            var x = string.Empty;
            foreach (var item in items)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    x += $"<p class='item {(isDark ? "dark-room" : "")}'>{item}</p>";
                }

            }

            return x;

        }

        public string DisplayMobs(Room room, Player player)
        {
            var mobs = string.Empty;
            var isDark = RoomIsDark(room, player);

            foreach (var mob in room.Mobs)
            {
                if (!string.IsNullOrEmpty(mob.LongName))
                {
                    mobs += $"<p class='mob {(isDark ? "dark-room" : "")}'>" + mob.LongName + "</p>";
                }
                else
                {
                    mobs += $"<p class='mob {(isDark ? "dark-room" : "")}'>" + mob.Name + " is here.</p>";
                }

            }

            return mobs;

        }

        public bool RoomIsDark(Room room, Player player)
        {
            if (room.RoomLit)
            {
                return false;
            }

            if (room.Type == Room.RoomType.Inside || room.Type == Room.RoomType.Town || room.Type == Room.RoomType.Shop || room.Type == Room.RoomType.Guild)
            {
                return false;
            }

            if (player.Equipped.Light != null)
            {
                return false;
            }

            if (!_time.IsNightTime())
            {
                return true;
            }

            return false;
        }

        public string DisplayPlayers(Room room, Player player)
        {
            var players = string.Empty;
            var isNightTime = !_time.IsNightTime();

            foreach (var pc in room.Players)
            {
                if (pc.Name == player.Name)
                {
                    continue;
                }
                players += string.IsNullOrEmpty(pc.LongName) ? $"<p class='player {(isNightTime ? "dark-room" : "")}'>{pc.Name} is here.</p>" : $"<p class='player'>{pc.Name} {pc.LongName}.</p>";
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
