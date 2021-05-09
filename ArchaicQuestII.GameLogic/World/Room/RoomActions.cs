using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Encodings.Web;
using ArchaicQuestII.Core.World;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Item;
using Newtonsoft.Json;

namespace ArchaicQuestII.GameLogic.World.Room
{
    public class RoomActions : IRoomActions
    {

        private readonly IWriteToClient _writeToClient;
        private readonly ITime _time;
        private readonly ICache _cache;
        public RoomActions(IWriteToClient writeToClient, ITime time, ICache cache)
        {
            _writeToClient = writeToClient;
            _time = time;
            _cache = cache;
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

            if (player.Affects.Blind)
            {
                _writeToClient.WriteLine("<p>You are blind and can't see a thing!</p>", player.ConnectionId);
                return;
            }

            if (!string.IsNullOrEmpty(target) && !target.Equals("look", StringComparison.CurrentCultureIgnoreCase) && !string.IsNullOrEmpty(target) && !target.Equals("l", StringComparison.CurrentCultureIgnoreCase))
            {
                LookObject(target, room, player);
                return;
            }

            var showVerboseExits = player.Config.VerboseExits;
            string exits = FindValidExits(room, showVerboseExits);
            
            var items = DisplayItems(room, player);
            var mobs = DisplayMobs(room, player);
            var players = DisplayPlayers(room, player);

            var roomDesc = new StringBuilder();
            var isDark = RoomIsDark(room, player);
          
            roomDesc
                .Append($"<p class=\"room-title {(isDark ? "room-dark" : "")}\">{room.Title}<br /></p>")
                .Append($"<p class=\"room-description  {(isDark ? "room-dark" : "")}\">{room.Description}</p>");

            if (!showVerboseExits)
            {
                roomDesc.Append(
                    $"<p class=\"room-exit  {(isDark ? "room-dark" : "")}\"> <span class=\"room-exits\">[</span>Exits: <span class=\"room-exits\">{exits}</span><span class=\"room-exits\">]</span></p>");
            }
            else
            {
                roomDesc.Append(
                    $"<div class=\" {(isDark ? "room-dark" : "")}\">Obvious exits: <table class=\"room-exits\"><tbody>{exits}</tbody></table></div>");
            }
 
            roomDesc.Append($"<p  class=\" {(isDark ? "room-dark" : "")}\">{items}</p>")
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

            var nthTarget = Helpers.findNth(target);
            var container = Helpers.findRoomObject(nthTarget, room) ?? Helpers.findObjectInInventory(nthTarget, player);

            if (container != null && (container.ItemType != Item.Item.ItemTypes.Container && container.ItemType != Item.Item.ItemTypes.Cooking))
            {
                if (container.ItemType == Item.Item.ItemTypes.Portal)
                {
                    LookInPortal(container, room, player);
                    return;
                }

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

            _writeToClient.WriteLine($"<p>{container.Name} contains:</p>", player.ConnectionId);
            if (container.Container.Items.Count == 0)
            {
                _writeToClient.WriteLine($"<p>Nothing.</p>", player.ConnectionId);
            }

            var isDark = RoomIsDark(room, player);
            foreach (var obj in container.Container.Items.List(false))
            {
                _writeToClient.WriteLine($"<p class='item {(isDark ? "room-dark" : "")}'>{obj.Name}</p>", player.ConnectionId);
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

        public void LookInPortal(Item.Item portal, Room room, Player player)
        {
            var getPortalLocation = _cache.GetRoom(portal.Portal.Destination);

            if (getPortalLocation == null)
            {
                //Log error
                _writeToClient.WriteLine("<p>The dark abyss, I wouldn't enter if I were you.</p>", player.ConnectionId);
                return;
            }

            Look("", getPortalLocation, player);
        }

        public void LookObject(string target, Room room, Player player)
        {
            if (player.Status == CharacterStatus.Status.Sleeping)
            {
                _writeToClient.WriteLine("You can't do that while asleep.", player.ConnectionId);
                return;
            }
            var isDark = RoomIsDark(room, player);
            var nthTarget = Helpers.findNth(target);

            var item = Helpers.findRoomObject(nthTarget, room) ?? Helpers.findObjectInInventory(nthTarget, player);
            var character = Helpers.FindMob(nthTarget, room) ?? Helpers.FindPlayer(nthTarget, room);


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

            if (item != null && character == null)
            {
                _writeToClient.WriteLine($"<p  class='{(isDark ? "room-dark" : "")}'>{item.Description.Look}", player.ConnectionId);

                if (item.Container != null && !item.Container.CanOpen && item.Container.Items.Any())
                {
                    _writeToClient.WriteLine($"<p  class='{(isDark ? "room-dark" : "")}'>{item.Name} contains:", player.ConnectionId);

                    var listOfContainerItems = new StringBuilder();
                    foreach (var containerItem in item.Container.Items.List())
                    {
                        listOfContainerItems.Append($"<p class='{(isDark ? "room-dark" : "")}'>{containerItem.Name.Replace(" lies here.", "")}</p>");
                    }

                    _writeToClient.WriteLine(listOfContainerItems.ToString(), player.ConnectionId);

                }

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
            //for player?
            if (roomObjects != null && character == null)
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

            var nthTarget = Helpers.findNth(target);

            var item = Helpers.findRoomObject(nthTarget, room) ?? Helpers.findObjectInInventory(nthTarget, player);
            var character = Helpers.FindMob(nthTarget, room) ?? Helpers.FindPlayer(nthTarget, room);

            if (character != null)
            {
                LookObject(target, room, player);
                return;
            }
            RoomObject roomObjects = null;
       
            if (item == null)
            {
                _writeToClient.WriteLine("<p>You don't see that here.", player.ConnectionId);
                return;
            }

            var isDark = RoomIsDark(room, player);
            var examMessage = item.Description.Exam == "You don't see anything special."
                ? $"On closer inspection you don't see anything special to note to what you already see. {item.Description.Look}"
                : item.Description.Exam;
            _writeToClient.WriteLine($"<p class='{(isDark ? "room-dark" : "")}'>{examMessage}", player.ConnectionId);

            foreach (var pc in room.Players)
            {
                if (pc.Name == player.Name)
                {
                    continue;
                }

                _writeToClient.WriteLine($"<p>{player.Name} examines {item.Name.ToLower()}.</p>", pc.ConnectionId);
            }

         

            if (item == null && room.RoomObjects.Count >= 1 && room.RoomObjects[0].Name != null)
            {
                roomObjects =
                    room.RoomObjects.FirstOrDefault(x =>
                        x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase));

                _writeToClient.WriteLine($"<p class='{(isDark ? "room-dark" : "")}'>{roomObjects.Examine}", player.ConnectionId);
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

            var nthTarget = Helpers.findNth(target);
            var item = Helpers.findRoomObject(nthTarget, room) ?? Helpers.findObjectInInventory(nthTarget, player);


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

            var nthTarget = Helpers.findNth(target);
            var item = Helpers.findRoomObject(nthTarget, room) ?? Helpers.findObjectInInventory(nthTarget, player);


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


            var nthTarget = Helpers.findNth(target);
            var item = Helpers.findRoomObject(nthTarget, room) ?? Helpers.findObjectInInventory(nthTarget, player);

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

        public Item.Item GetItemAttributes(int index, Room room)
        {

            return room.Items.FirstOrDefault(x => x.Id.Equals(index));
        }

        public string DisplayItems(Room room, Player player)
        {
            var isDark = RoomIsDark(room, player);
            var items = room.Items.List();
            var x = string.Empty;
            int index = 0;
            foreach (var item in items)
            {
                if (!string.IsNullOrEmpty(item.Name))
                {

                    var i = GetItemAttributes(item.Id, room);
                    var keyword = i.Name.Split(" ");

                    var data = $"{{detail: {{name: \" {HtmlEncoder.Default.Encode(i.Name)}\", desc: \"{HtmlEncoder.Default.Encode(i.Description.Look)}\", type: \"{i.ItemType}\", canOpen: \"{i.Container.CanOpen}\", isOpen: \"{i.Container.IsOpen}\", keyword: \"{HtmlEncoder.Default.Encode(keyword[keyword.Length - 1])}\"}}}}";

                    var clickEvent = $"window.dispatchEvent(new CustomEvent(\"open-detail\", {data}))";
                    x += $"<p onClick='{clickEvent}' class='item {(isDark ? "dark-room" : "")}' >{item.Name}</p>";
                }
                index++;

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


        public string GetRoom(Exit exit)
        {
            if (exit == null)
            {
                return "";
            }

            
            var RoomId = $"{exit.AreaId}{exit.Coords.X}{exit.Coords.Y}{exit.Coords.Z}";
            var room = _cache.GetRoom(RoomId);

            return room.Title;
        }


        /// <summary>
        /// Displays valid exits
        /// </summary>
        public string FindValidExits(Room room, bool verbose)
        {
            var exits = new List<string>();
            var exitList = string.Empty;

        /* TODO: Click event for simple exit view */

            if (room.Exits.North != null)
            {
                
                var clickEvent = "window.dispatchEvent(new CustomEvent(\"post-to-server\", {\"detail\":\"n\"}))";
                exits.Add(verbose
                    ? $"<tr class='verbose-exit-wrapper'><td class='verbose-exit'>{Helpers.DisplayDoor(room.Exits.North)} </td><td style='text-align:center; color:#fff'> - </td><td class='verbose-exit-name'><a href='javascript:void(0)' onclick='{clickEvent}'>{GetRoom(room.Exits.North)}</a></td></tr>"
                    : Helpers.DisplayDoor(room.Exits.North));
            }

          

            if (room.Exits.East != null && room.Exits.East.Coords != null)
            {
                var clickEvent = "window.dispatchEvent(new CustomEvent(\"post-to-server\", {\"detail\":\"e\"}))";
                exits.Add(verbose
                    ? $"<tr class='verbose-exit-wrapper'><td class='verbose-exit'>{Helpers.DisplayDoor(room.Exits.East)} </td><td style='text-align:center; color:#fff'> - </td><td class='verbose-exit-name'><a href='javascript:void(0)' onclick='{clickEvent}'>{GetRoom(room.Exits.East)}</a></td></tr>"
                    : Helpers.DisplayDoor(room.Exits.East));
            }

           

            if (room.Exits.South != null)
            {
                var clickEvent = "window.dispatchEvent(new CustomEvent(\"post-to-server\", {\"detail\":\"s\"}))";
                exits.Add(verbose
                    ? $"<tr class='verbose-exit-wrapper'><td class='verbose-exit'>{Helpers.DisplayDoor(room.Exits.South)} </td><td style='text-align:center; color:#fff'> - </td><td class='verbose-exit-name'><a href='javascript:void(0)' onclick='{clickEvent}'>{GetRoom(room.Exits.South)}</a></td></tr>"
                    : Helpers.DisplayDoor(room.Exits.South));
            }

            if (room.Exits.West != null)
            {
                var clickEvent = "window.dispatchEvent(new CustomEvent(\"post-to-server\", {\"detail\":\"w\"}))";
                exits.Add(verbose
                    ? $"<tr class='verbose-exit-wrapper'><td class='verbose-exit'>{Helpers.DisplayDoor(room.Exits.West)} </td><td style='text-align:center; color:#fff'> - </td><td class='verbose-exit-name'><a href='javascript:void(0)' onclick='{clickEvent}'>{GetRoom(room.Exits.West)}</a></td></tr>"
                    : Helpers.DisplayDoor(room.Exits.West));
            }

            if (room.Exits.NorthEast != null)
            {
                var clickEvent = "window.dispatchEvent(new CustomEvent(\"post-to-server\", {\"detail\":\"ne\"}))";
                exits.Add(verbose
                    ? $"<tr class='verbose-exit-wrapper'><td class='verbose-exit'>{Helpers.DisplayDoor(room.Exits.NorthEast)}  </td><td style='text-align:center; color:#fff'> - </td><td class='verbose-exit-name'><a href='javascript:void(0)' onclick='{clickEvent}'>{GetRoom(room.Exits.NorthEast)}</a></td></tr>"
                    : Helpers.DisplayDoor(room.Exits.NorthEast));
            }

            if (room.Exits.SouthEast != null)
            {
                var clickEvent = "window.dispatchEvent(new CustomEvent(\"post-to-server\", {\"detail\":\"se\"}))";
                exits.Add(verbose
                    ? $"<tr class='verbose-exit-wrapper'><td class='verbose-exit'>{Helpers.DisplayDoor(room.Exits.SouthEast)}  </td><td style='text-align:center; color:#fff'> - </td><td class='verbose-exit-name'><a href='javascript:void(0)' onclick='{clickEvent}'>{GetRoom(room.Exits.SouthEast)}</a></td></tr>"
                    : Helpers.DisplayDoor(room.Exits.SouthEast));
            }

            if (room.Exits.SouthWest != null)
            {
                var clickEvent = "window.dispatchEvent(new CustomEvent(\"post-to-server\", {\"detail\":\"sw\"}))";
                exits.Add(verbose
                    ? $"<tr class='verbose-exit-wrapper'><td class='verbose-exit'>{Helpers.DisplayDoor(room.Exits.SouthWest)}  </td><td style='text-align:center; color:#fff'> - </td><td class='verbose-exit-name'><a href='javascript:void(0)' onclick='{clickEvent}'>{GetRoom(room.Exits.SouthWest)}</a></td></tr>"
                    : Helpers.DisplayDoor(room.Exits.SouthWest));
            }

            if (room.Exits.NorthWest != null)
            {
                var clickEvent = "window.dispatchEvent(new CustomEvent(\"post-to-server\", {\"detail\":\"nw\"}))";
                exits.Add(verbose
                    ? $"<tr class='verbose-exit-wrapper'><td class='verbose-exit'>{Helpers.DisplayDoor(room.Exits.NorthWest)}  </td> <td style='text-align:center; color:#fff'> - </td><td class='verbose-exit-name'><a href='javascript:void(0)' onclick='{clickEvent}'>{GetRoom(room.Exits.NorthWest)}</a></td></tr>"
                    : Helpers.DisplayDoor(room.Exits.NorthWest));
            }

            if (room.Exits.Down != null)
            {
                var clickEvent = "window.dispatchEvent(new CustomEvent(\"post-to-server\", {\"detail\":\"d\"}))";
                exits.Add(verbose
                    ? $"<tr class='verbose-exit-wrapper'><td class='verbose-exit'>{Helpers.DisplayDoor(room.Exits.Down)}  </td> <td style='text-align:center; color:#fff'> - </td><td class='verbose-exit-name'><a href='javascript:void(0)' onclick='{clickEvent}'>{GetRoom(room.Exits.Down)}</a></td></tr>"
                    : Helpers.DisplayDoor(room.Exits.Down));
            }

            if (room.Exits.Up != null)
            {
                var clickEvent = "window.dispatchEvent(new CustomEvent(\"post-to-server\", {\"detail\":\"u\"}))";
                exits.Add(verbose
                    ? $"<tr class='verbose-exit-wrapper'><td class='verbose-exit'>{Helpers.DisplayDoor(room.Exits.Up)}  </td> <td style='text-align:center; color:#fff'> - </td><td class='verbose-exit-name'><a href='javascript:void(0)' onclick='{clickEvent}'>{GetRoom(room.Exits.Up)}</a></td></tr>"
                    : Helpers.DisplayDoor(room.Exits.Up));
            }



            if (exits.Count <= 0)
            {
                exits.Add("None");
            }


            foreach (var exit in exits)
            {
                if (!verbose)
                {
                    exitList += exit + ", ";
                }
                else
                {
                    exitList += exit;
                }

            }
            if (!verbose)
            {
                exitList = exitList.Remove(exitList.Length - 2);

            }
            return  exitList;

        }


    }
}
