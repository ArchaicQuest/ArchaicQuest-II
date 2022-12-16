using System;
using System.Collections.Generic;
using System.Linq;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using MoonSharp.Interpreter;

namespace ArchaicQuestII.GameLogic.World.Room
{
    public class RoomActions : IRoomActions
    {
        private readonly IWriteToClient _writeToClient;
        private readonly ITime _time;
        private readonly ICache _cache;
        private readonly IUpdateClientUI _updateClient;
        private readonly IMobScripts _mobScripts;
        private readonly IDataBase _database;

        public RoomActions(
            IWriteToClient writeToClient, 
            ITime time, 
            ICache cache,
            IUpdateClientUI updateClient, 
            IMobScripts mobScripts, 
            IDataBase database)
        {
            _writeToClient = writeToClient;
            _time = time;
            _cache = cache;
            _updateClient = updateClient;
            _mobScripts = mobScripts;
            _database = database;
        }
        
        /// <summary>
        /// Helper to get area from room
        /// </summary>
        /// <param name="room">Room to get area from</param>
        public Area.Area GetRoomArea(Room room)
        {
            return _database.GetCollection<Area.Area>(DataBase.Collections.Area).FindById(room.AreaId);
        }

        /// <summary>
        /// Checks if the room is dark
        /// </summary>
        /// <param name="room"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool RoomIsDark(Room room, Player player)
        {
            if (room.RoomLit)
            {
                return false;
            }

            if (room.Type is Room.RoomType.Inside or Room.RoomType.Town or Room.RoomType.Shop or Room.RoomType.Guild)
            {
                return false;
            }

            if (room.Terrain == Room.TerrainType.Inside)
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

        /// <summary>
        /// Get a room based on exit
        /// </summary>
        /// <param name="exit"></param>
        /// <returns></returns>
        public string GetRoom(Exit exit)
        {
            if (exit == null)
            {
                return "";
            }

            var roomId = $"{exit.AreaId}{exit.Coords.X}{exit.Coords.Y}{exit.Coords.Z}";
            var room = _cache.GetRoom(roomId);

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
                const string clickEvent = "window.dispatchEvent(new CustomEvent(\"post-to-server\", {\"detail\":\"n\"}))";
                exits.Add(verbose
                    ? $"<tr class='verbose-exit-wrapper'><td class='verbose-exit'>{Helpers.DisplayDoor(room.Exits.North)} </td><td style='text-align:center; color:#fff'> - </td><td class='verbose-exit-name'><a href='javascript:void(0)' onclick='{clickEvent}'>{GetRoom(room.Exits.North)}</a></td></tr>"
                    : Helpers.DisplayDoor(room.Exits.North));
            }
            
            if (room.Exits.East is { Coords: { } })
            {
                const string clickEvent = "window.dispatchEvent(new CustomEvent(\"post-to-server\", {\"detail\":\"e\"}))";
                exits.Add(verbose
                    ? $"<tr class='verbose-exit-wrapper'><td class='verbose-exit'>{Helpers.DisplayDoor(room.Exits.East)} </td><td style='text-align:center; color:#fff'> - </td><td class='verbose-exit-name'><a href='javascript:void(0)' onclick='{clickEvent}'>{GetRoom(room.Exits.East)}</a></td></tr>"
                    : Helpers.DisplayDoor(room.Exits.East));
            }
            
            if (room.Exits.South != null)
            {
                const string clickEvent = "window.dispatchEvent(new CustomEvent(\"post-to-server\", {\"detail\":\"s\"}))";
                exits.Add(verbose
                    ? $"<tr class='verbose-exit-wrapper'><td class='verbose-exit'>{Helpers.DisplayDoor(room.Exits.South)} </td><td style='text-align:center; color:#fff'> - </td><td class='verbose-exit-name'><a href='javascript:void(0)' onclick='{clickEvent}'>{GetRoom(room.Exits.South)}</a></td></tr>"
                    : Helpers.DisplayDoor(room.Exits.South));
            }

            if (room.Exits.West != null)
            {
                const string clickEvent = "window.dispatchEvent(new CustomEvent(\"post-to-server\", {\"detail\":\"w\"}))";
                exits.Add(verbose
                    ? $"<tr class='verbose-exit-wrapper'><td class='verbose-exit'>{Helpers.DisplayDoor(room.Exits.West)} </td><td style='text-align:center; color:#fff'> - </td><td class='verbose-exit-name'><a href='javascript:void(0)' onclick='{clickEvent}'>{GetRoom(room.Exits.West)}</a></td></tr>"
                    : Helpers.DisplayDoor(room.Exits.West));
            }

            if (room.Exits.NorthEast != null)
            {
                const string clickEvent = "window.dispatchEvent(new CustomEvent(\"post-to-server\", {\"detail\":\"ne\"}))";
                exits.Add(verbose
                    ? $"<tr class='verbose-exit-wrapper'><td class='verbose-exit'>{Helpers.DisplayDoor(room.Exits.NorthEast)}  </td><td style='text-align:center; color:#fff'> - </td><td class='verbose-exit-name'><a href='javascript:void(0)' onclick='{clickEvent}'>{GetRoom(room.Exits.NorthEast)}</a></td></tr>"
                    : Helpers.DisplayDoor(room.Exits.NorthEast));
            }

            if (room.Exits.SouthEast != null)
            {
                const string clickEvent = "window.dispatchEvent(new CustomEvent(\"post-to-server\", {\"detail\":\"se\"}))";
                exits.Add(verbose
                    ? $"<tr class='verbose-exit-wrapper'><td class='verbose-exit'>{Helpers.DisplayDoor(room.Exits.SouthEast)}  </td><td style='text-align:center; color:#fff'> - </td><td class='verbose-exit-name'><a href='javascript:void(0)' onclick='{clickEvent}'>{GetRoom(room.Exits.SouthEast)}</a></td></tr>"
                    : Helpers.DisplayDoor(room.Exits.SouthEast));
            }

            if (room.Exits.SouthWest != null)
            {
                const string clickEvent = "window.dispatchEvent(new CustomEvent(\"post-to-server\", {\"detail\":\"sw\"}))";
                exits.Add(verbose
                    ? $"<tr class='verbose-exit-wrapper'><td class='verbose-exit'>{Helpers.DisplayDoor(room.Exits.SouthWest)}  </td><td style='text-align:center; color:#fff'> - </td><td class='verbose-exit-name'><a href='javascript:void(0)' onclick='{clickEvent}'>{GetRoom(room.Exits.SouthWest)}</a></td></tr>"
                    : Helpers.DisplayDoor(room.Exits.SouthWest));
            }

            if (room.Exits.NorthWest != null)
            {
                const string clickEvent = "window.dispatchEvent(new CustomEvent(\"post-to-server\", {\"detail\":\"nw\"}))";
                exits.Add(verbose
                    ? $"<tr class='verbose-exit-wrapper'><td class='verbose-exit'>{Helpers.DisplayDoor(room.Exits.NorthWest)}  </td> <td style='text-align:center; color:#fff'> - </td><td class='verbose-exit-name'><a href='javascript:void(0)' onclick='{clickEvent}'>{GetRoom(room.Exits.NorthWest)}</a></td></tr>"
                    : Helpers.DisplayDoor(room.Exits.NorthWest));
            }

            if (room.Exits.Down != null)
            {
                const string clickEvent = "window.dispatchEvent(new CustomEvent(\"post-to-server\", {\"detail\":\"d\"}))";
                exits.Add(verbose
                    ? $"<tr class='verbose-exit-wrapper'><td class='verbose-exit'>{Helpers.DisplayDoor(room.Exits.Down)}  </td> <td style='text-align:center; color:#fff'> - </td><td class='verbose-exit-name'><a href='javascript:void(0)' onclick='{clickEvent}'>{GetRoom(room.Exits.Down)}</a></td></tr>"
                    : Helpers.DisplayDoor(room.Exits.Down));
            }

            if (room.Exits.Up != null)
            {
                const string clickEvent = "window.dispatchEvent(new CustomEvent(\"post-to-server\", {\"detail\":\"u\"}))";
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

            return exitList;
        }

        /// <summary>
        /// Used to Change Player room
        /// </summary>
        /// <param name="player"></param>
        /// <param name="oldRoom"></param>
        /// <param name="newRoom"></param>
        public void RoomChange(Player player, Room oldRoom, Room newRoom)
        {
            player.Pose = string.Empty;
            
            if (oldRoom.Mobs.Any())
            {
                OnPlayerLeaveEvent(oldRoom, player);
            }

            ExitRoom(player, oldRoom, newRoom);
            
            UpdateCharactersLocation(player, oldRoom, newRoom);
            
            EnterRoom(player, newRoom, oldRoom);
            
            if (newRoom.Mobs.Any())
            {
                OnPlayerEnterEvent(newRoom, player);
            }

            _updateClient.GetMap(player, _cache.GetMap($"{newRoom.AreaId}{newRoom.Coords.Z}"));
            _updateClient.UpdateMoves(player);
            
            player.Buffer.Enqueue("look");
        }

        /// <summary>
        /// Updates the characters location
        /// </summary>
        /// <param name="character"></param>
        /// <param name="oldRoom"></param>
        /// <param name="newRoom"></param>
        private void UpdateCharactersLocation(Player character, Room oldRoom, Room newRoom)
        {
            if (character.ConnectionId != "mob")
            {
                // remove player from room
                oldRoom.Players.Remove(character);

                //add player to room
                character.RoomId = $"{newRoom.AreaId}{newRoom.Coords.X}{newRoom.Coords.Y}{newRoom.Coords.Z}";
                newRoom.Players.Add(character);

                //player entered new area TODO: Add area announce
                //if(oldRoom.AreaId != newRoom.AreaId)
                //    _areaActions.AreaEntered(player, newRoom);
            }
            else
            {
                // remove mob from room
                oldRoom.Mobs.Remove(character);

                //add mob to room
                character.RoomId = $"{newRoom.AreaId}{newRoom.Coords.X}{newRoom.Coords.Y}{newRoom.Coords.Z}";
                newRoom.Mobs.Add(character);
            }
        }

        private void EnterRoom(Player character, Room toRoom, Room fromRoom)
        {
            var direction = "from nowhere";
            var movement = "appears";

            if (toRoom.Exits.Down.RoomId == fromRoom.Id)
                direction = "down";
            if (toRoom.Exits.Up.RoomId == fromRoom.Id)
                direction = "up";
            if (toRoom.Exits.North.RoomId == fromRoom.Id)
                direction = "in from the north";
            if (toRoom.Exits.South.RoomId == fromRoom.Id)
                direction = "in form the south";
            if (toRoom.Exits.East.RoomId == fromRoom.Id)
                direction = "in from the east";
            if (toRoom.Exits.West.RoomId == fromRoom.Id)
                direction = "in from the west";
            if (toRoom.Exits.NorthEast.RoomId == fromRoom.Id)
                direction = "in from the northeast";
            if (toRoom.Exits.NorthWest.RoomId == fromRoom.Id)
                direction = "in from the northwest";
            if (toRoom.Exits.SouthEast.RoomId == fromRoom.Id)
                direction = "in from the southeast";
            if (toRoom.Exits.SouthWest.RoomId == fromRoom.Id)
                direction = "in from the southwest";

            switch (character.Status)
            {
                case CharacterStatus.Status.Floating:
                    movement = "floats";
                    break;
                case CharacterStatus.Status.Mounted:
                    movement = "rides";
                    break;
                case CharacterStatus.Status.Fleeing:
                    movement = "flees";
                    character.Status = CharacterStatus.Status.Standing;
                    break;
                case CharacterStatus.Status.Standing:
                    _updateClient.PlaySound("walk", character);
                    movement = "walks";
                    break;
            }

            foreach (var p in fromRoom.Players.Where(p => character.Name != p.Name))
            {
                _writeToClient.WriteLine(
                    $"<span class='{(character.ConnectionId != "mob" ? "player" : "mob")}'>{character.Name} {movement} {direction}.</span>",
                    p.ConnectionId);
            }
        }

        private void ExitRoom(Player characterBase, Room toRoom, Room fromRoom)
        {
            var direction = "to thin air";
            var movement = "vanishes";

            if (fromRoom.Exits.Down.RoomId == toRoom.Id)
                direction = "down";
            if (fromRoom.Exits.Up.RoomId == toRoom.Id)
                direction = "up";
            if (fromRoom.Exits.North.RoomId == toRoom.Id)
                direction = "to the north";
            if (fromRoom.Exits.South.RoomId == toRoom.Id)
                direction = "to the south";
            if (fromRoom.Exits.East.RoomId == toRoom.Id)
                direction = "to the east";
            if (fromRoom.Exits.West.RoomId == toRoom.Id)
                direction = "to the west";
            if (fromRoom.Exits.NorthEast.RoomId == toRoom.Id)
                direction = "to the northeast";
            if (fromRoom.Exits.NorthWest.RoomId == toRoom.Id)
                direction = "to the northwest";
            if (fromRoom.Exits.SouthEast.RoomId == toRoom.Id)
                direction = "to the southeast";
            if (fromRoom.Exits.SouthWest.RoomId == toRoom.Id)
                direction = "to the southwest";

            switch (characterBase.Status)
            {
                case CharacterStatus.Status.Floating:
                    movement = "floats";
                    break;
                case CharacterStatus.Status.Mounted:
                    movement = "rides";
                    break;
                case CharacterStatus.Status.Fleeing:
                    movement = "flees";
                    characterBase.Status = CharacterStatus.Status.Standing;
                    break;
                case CharacterStatus.Status.Standing:
                    movement = "walks";
                    break;
            }

            foreach (var p in fromRoom.Players.Where(p => characterBase.Name != p.Name))
            {
                _writeToClient.WriteLine(
                    $"<span class='{(characterBase.ConnectionId != "mob" ? "player" : "mob")}'>{characterBase.Name} {movement} {direction}.</span>",
                    p.ConnectionId);
            }
        }

        private void OnPlayerLeaveEvent(Room room, Player character)
        {
            foreach (var mob in room.Mobs.Where(mob => !string.IsNullOrEmpty(mob.Events.Leave)))
            {
                UserData.RegisterType<MobScripts>();

                var script = new Script();

                var obj = UserData.Create(_mobScripts);
                script.Globals.Set("obj", obj);
                UserData.RegisterProxyType<MyProxy, Room>(r => new MyProxy(room));
                UserData.RegisterProxyType<ProxyPlayer, Player>(r => new ProxyPlayer(character));
                    
                script.Globals["room"] = room;
                script.Globals["player"] = character;
                script.Globals["mob"] = mob;
                    
                var res = script.DoString(mob.Events.Leave);
            }
        }

        private void OnPlayerEnterEvent(Room room, Player character)
        {
            foreach (var mob in room.Mobs)
            {
                if (!string.IsNullOrEmpty(mob.Events.Enter))
                {
                    try
                    {
                        UserData.RegisterType<MobScripts>();

                        var script = new Script();

                        var obj = UserData.Create(_mobScripts);
                        script.Globals.Set("obj", obj);
                        UserData.RegisterProxyType<MyProxy, Room>(r => new MyProxy(room));
                        UserData.RegisterProxyType<ProxyPlayer, Player>(r => new ProxyPlayer(character));

                        script.Globals["room"] = room;
                        script.Globals["player"] = character;
                        script.Globals["mob"] = mob;

                        var res = script.DoString(mob.Events.Enter);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }

                if (mob.Agro && mob.Status != CharacterStatus.Status.Fighting && character.ConnectionId != "mob")
                {
                    _writeToClient.WriteLine($"{mob.Name} attacks you!", character.ConnectionId);
                    _mobScripts.AttackPlayer(room, character, mob);
                }
            }
        }
    }
}
