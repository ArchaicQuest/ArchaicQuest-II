using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Utilities;
using MoonSharp.Interpreter;

namespace ArchaicQuestII.GameLogic.World.Room
{
    public class RoomActions : IRoomActions
    {
        public bool RoomIsDark(Player player, Room room)
        {
            if (room.IsLit)
                return false;

            if (player.Affects.DarkVision)
                return false;

            if (player.Equipped.Light != null)
                return false;

            foreach (var pc in room.Players)
            {
                if (pc.Equipped.Light != null)
                    return false;
            }

            if (room.Type is Room.RoomType.Underground or Room.RoomType.Inside)
                return true;

            return Services.Instance.Time.IsNightTime();
        }

        /// <summary>
        /// Helper to get area from room
        /// </summary>
        /// <param name="room">Room to get area from</param>
        public Area.Area GetRoomArea(Room room)
        {
            return Services.Instance.DataBase
                .GetCollection<Area.Area>(DataBase.Collections.Area)
                .FindById(room.AreaId);
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
            var room = Services.Instance.Cache.GetRoom(roomId);

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
                const string clickEvent =
                    "window.dispatchEvent(new CustomEvent(\"post-to-server\", {\"detail\":\"n\"}))";
                exits.Add(
                    verbose
                        ? $"<tr class='verbose-exit-wrapper'><td class='verbose-exit'>{Helpers.DisplayDoor(room.Exits.North)} </td><td style='text-align:center; color:#fff'> - </td><td class='verbose-exit-name'><a href='javascript:void(0)' onclick='{clickEvent}'>{GetRoom(room.Exits.North)}</a></td></tr>"
                        : Helpers.DisplayDoor(room.Exits.North)
                );
            }

            if (room.Exits.East != null)
            {
                const string clickEvent =
                    "window.dispatchEvent(new CustomEvent(\"post-to-server\", {\"detail\":\"e\"}))";
                exits.Add(
                    verbose
                        ? $"<tr class='verbose-exit-wrapper'><td class='verbose-exit'>{Helpers.DisplayDoor(room.Exits.East)} </td><td style='text-align:center; color:#fff'> - </td><td class='verbose-exit-name'><a href='javascript:void(0)' onclick='{clickEvent}'>{GetRoom(room.Exits.East)}</a></td></tr>"
                        : Helpers.DisplayDoor(room.Exits.East)
                );
            }

            if (room.Exits.South != null)
            {
                const string clickEvent =
                    "window.dispatchEvent(new CustomEvent(\"post-to-server\", {\"detail\":\"s\"}))";
                exits.Add(
                    verbose
                        ? $"<tr class='verbose-exit-wrapper'><td class='verbose-exit'>{Helpers.DisplayDoor(room.Exits.South)} </td><td style='text-align:center; color:#fff'> - </td><td class='verbose-exit-name'><a href='javascript:void(0)' onclick='{clickEvent}'>{GetRoom(room.Exits.South)}</a></td></tr>"
                        : Helpers.DisplayDoor(room.Exits.South)
                );
            }

            if (room.Exits.West != null)
            {
                const string clickEvent =
                    "window.dispatchEvent(new CustomEvent(\"post-to-server\", {\"detail\":\"w\"}))";
                exits.Add(
                    verbose
                        ? $"<tr class='verbose-exit-wrapper'><td class='verbose-exit'>{Helpers.DisplayDoor(room.Exits.West)} </td><td style='text-align:center; color:#fff'> - </td><td class='verbose-exit-name'><a href='javascript:void(0)' onclick='{clickEvent}'>{GetRoom(room.Exits.West)}</a></td></tr>"
                        : Helpers.DisplayDoor(room.Exits.West)
                );
            }

            if (room.Exits.NorthEast != null)
            {
                const string clickEvent =
                    "window.dispatchEvent(new CustomEvent(\"post-to-server\", {\"detail\":\"ne\"}))";
                exits.Add(
                    verbose
                        ? $"<tr class='verbose-exit-wrapper'><td class='verbose-exit'>{Helpers.DisplayDoor(room.Exits.NorthEast)}  </td><td style='text-align:center; color:#fff'> - </td><td class='verbose-exit-name'><a href='javascript:void(0)' onclick='{clickEvent}'>{GetRoom(room.Exits.NorthEast)}</a></td></tr>"
                        : Helpers.DisplayDoor(room.Exits.NorthEast)
                );
            }

            if (room.Exits.SouthEast != null)
            {
                const string clickEvent =
                    "window.dispatchEvent(new CustomEvent(\"post-to-server\", {\"detail\":\"se\"}))";
                exits.Add(
                    verbose
                        ? $"<tr class='verbose-exit-wrapper'><td class='verbose-exit'>{Helpers.DisplayDoor(room.Exits.SouthEast)}  </td><td style='text-align:center; color:#fff'> - </td><td class='verbose-exit-name'><a href='javascript:void(0)' onclick='{clickEvent}'>{GetRoom(room.Exits.SouthEast)}</a></td></tr>"
                        : Helpers.DisplayDoor(room.Exits.SouthEast)
                );
            }

            if (room.Exits.SouthWest != null)
            {
                const string clickEvent =
                    "window.dispatchEvent(new CustomEvent(\"post-to-server\", {\"detail\":\"sw\"}))";
                exits.Add(
                    verbose
                        ? $"<tr class='verbose-exit-wrapper'><td class='verbose-exit'>{Helpers.DisplayDoor(room.Exits.SouthWest)}  </td><td style='text-align:center; color:#fff'> - </td><td class='verbose-exit-name'><a href='javascript:void(0)' onclick='{clickEvent}'>{GetRoom(room.Exits.SouthWest)}</a></td></tr>"
                        : Helpers.DisplayDoor(room.Exits.SouthWest)
                );
            }

            if (room.Exits.NorthWest != null)
            {
                const string clickEvent =
                    "window.dispatchEvent(new CustomEvent(\"post-to-server\", {\"detail\":\"nw\"}))";
                exits.Add(
                    verbose
                        ? $"<tr class='verbose-exit-wrapper'><td class='verbose-exit'>{Helpers.DisplayDoor(room.Exits.NorthWest)}  </td> <td style='text-align:center; color:#fff'> - </td><td class='verbose-exit-name'><a href='javascript:void(0)' onclick='{clickEvent}'>{GetRoom(room.Exits.NorthWest)}</a></td></tr>"
                        : Helpers.DisplayDoor(room.Exits.NorthWest)
                );
            }

            if (room.Exits.Down != null)
            {
                const string clickEvent =
                    "window.dispatchEvent(new CustomEvent(\"post-to-server\", {\"detail\":\"d\"}))";
                exits.Add(
                    verbose
                        ? $"<tr class='verbose-exit-wrapper'><td class='verbose-exit'>{Helpers.DisplayDoor(room.Exits.Down)}  </td> <td style='text-align:center; color:#fff'> - </td><td class='verbose-exit-name'><a href='javascript:void(0)' onclick='{clickEvent}'>{GetRoom(room.Exits.Down)}</a></td></tr>"
                        : Helpers.DisplayDoor(room.Exits.Down)
                );
            }

            if (room.Exits.Up != null)
            {
                const string clickEvent =
                    "window.dispatchEvent(new CustomEvent(\"post-to-server\", {\"detail\":\"u\"}))";
                exits.Add(
                    verbose
                        ? $"<tr class='verbose-exit-wrapper'><td class='verbose-exit'>{Helpers.DisplayDoor(room.Exits.Up)}  </td> <td style='text-align:center; color:#fff'> - </td><td class='verbose-exit-name'><a href='javascript:void(0)' onclick='{clickEvent}'>{GetRoom(room.Exits.Up)}</a></td></tr>"
                        : Helpers.DisplayDoor(room.Exits.Up)
                );
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
        /// <param name="isFlee"></param>
        public async void RoomChange(Player player, Room oldRoom, Room newRoom, bool isFlee)
        {
            player.Pose = string.Empty;

            if (oldRoom.Mobs.Any())
            {
                OnPlayerLeaveEvent(oldRoom, player);
            }

            ExitRoom(player, newRoom, oldRoom, isFlee);

            UpdateCharactersLocation(player, oldRoom, newRoom);

            EnterRoom(player, newRoom, oldRoom, isFlee);

            Services.Instance.UpdateClient.GetMap(
                player,
                Services.Instance.Cache.GetMap($"{newRoom.AreaId}{newRoom.Coords.Z}")
            );
            Services.Instance.UpdateClient.UpdateMoves(player);
            player.Buffer.Enqueue("look");

            if (newRoom.Mobs.Any())
            {
                // force the on enter event to fire after Look
                await Task.Delay(125);
                OnPlayerEnterEvent(newRoom, player);
            }
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
                character.RoomId =
                    $"{newRoom.AreaId}{newRoom.Coords.X}{newRoom.Coords.Y}{newRoom.Coords.Z}";
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
                character.RoomId =
                    $"{newRoom.AreaId}{newRoom.Coords.X}{newRoom.Coords.Y}{newRoom.Coords.Z}";
                newRoom.Mobs.Add(character);
            }
        }

        private void EnterRoom(Player character, Room toRoom, Room fromRoom, bool isFlee)
        {
            var direction = "from nowhere";
            var movement = "appears";

            if (toRoom.Exits != null)
            {
                if (toRoom.Exits.Down?.RoomId == fromRoom.Id)
                    direction = "in from below";
                if (toRoom.Exits.Up?.RoomId == fromRoom.Id)
                    direction = "in from above";
                if (toRoom.Exits.North?.RoomId == fromRoom.Id)
                    direction = "in from the north";
                if (toRoom.Exits.South?.RoomId == fromRoom.Id)
                    direction = "in from the south";
                if (toRoom.Exits.East?.RoomId == fromRoom.Id)
                    direction = "in from the east";
                if (toRoom.Exits.West?.RoomId == fromRoom.Id)
                    direction = "in from the west";
                if (toRoom.Exits.NorthEast?.RoomId == fromRoom.Id)
                    direction = "in from the northeast";
                if (toRoom.Exits.NorthWest?.RoomId == fromRoom.Id)
                    direction = "in from the northwest";
                if (toRoom.Exits.SouthEast?.RoomId == fromRoom.Id)
                    direction = "in from the southeast";
                if (toRoom.Exits.SouthWest?.RoomId == fromRoom.Id)
                    direction = "in from the southwest";
            }

            switch (character.Status)
            {
                case CharacterStatus.Status.Floating:
                    movement = "floats";
                    break;
                case CharacterStatus.Status.Mounted:
                    movement = "rides";
                    break;
                case CharacterStatus.Status.Standing:
                    Services.Instance.UpdateClient.PlaySound("walk", character);
                    movement = "walks";
                    break;
            }

            if (isFlee)
            {
                Services.Instance.UpdateClient.PlaySound("flee", character);
                movement = "rushes";
            }

            foreach (var p in toRoom.Players.Where(p => character.Name != p.Name))
            {
                Services.Instance.Writer.WriteLine(
                    $"<span class='{(character.ConnectionId != "mob" ? "player" : "mob")}'>{character.Name} {movement} {direction}.</span>",
                    p.ConnectionId
                );
            }
        }

        private void ExitRoom(Player characterBase, Room toRoom, Room fromRoom, bool isFlee)
        {
            var direction = "to thin air";
            var movement = "vanishes";

            if (fromRoom.Exits != null)
            {
                if (fromRoom.Exits.Down?.RoomId == toRoom.Id)
                    direction = "down";
                if (fromRoom.Exits.Up?.RoomId == toRoom.Id)
                    direction = "up";
                if (fromRoom.Exits.North?.RoomId == toRoom.Id)
                    direction = "to the north";
                if (fromRoom.Exits.South?.RoomId == toRoom.Id)
                    direction = "to the south";
                if (fromRoom.Exits.East?.RoomId == toRoom.Id)
                    direction = "to the east";
                if (fromRoom.Exits.West?.RoomId == toRoom.Id)
                    direction = "to the west";
                if (fromRoom.Exits.NorthEast?.RoomId == toRoom.Id)
                    direction = "to the northeast";
                if (fromRoom.Exits.NorthWest?.RoomId == toRoom.Id)
                    direction = "to the northwest";
                if (fromRoom.Exits.SouthEast?.RoomId == toRoom.Id)
                    direction = "to the southeast";
                if (fromRoom.Exits.SouthWest?.RoomId == toRoom.Id)
                    direction = "to the southwest";
            }

            switch (characterBase.Status)
            {
                case CharacterStatus.Status.Floating:
                    movement = "floats";
                    break;
                case CharacterStatus.Status.Mounted:
                    movement = "rides";
                    break;
                case CharacterStatus.Status.Standing:
                    movement = "walks";
                    break;
            }

            if (isFlee)
            {
                movement = "flee";
            }

            foreach (var p in fromRoom.Players.Where(p => characterBase.Name != p.Name))
            {
                Services.Instance.Writer.WriteLine(
                    $"<span class='{(characterBase.ConnectionId != "mob" ? "player" : "mob")}'>{characterBase.Name} {movement} {direction}.</span>",
                    p.ConnectionId
                );
            }
        }

        private void OnPlayerLeaveEvent(Room room, Player character)
        {
            foreach (var mob in room.Mobs.Where(mob => !string.IsNullOrEmpty(mob.Events.Leave)))
            {
                UserData.RegisterType<MobScripts>();

                var script = new Script();

                var obj = UserData.Create(Services.Instance.MobScripts);
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
            foreach (var mob in room.Mobs.ToList())
            {
                if (!string.IsNullOrEmpty(mob.Events.Enter))
                {
                    try
                    {
                        UserData.RegisterType<MobScripts>();

                        var script = new Script();

                        var obj = UserData.Create(Services.Instance.MobScripts);
                        script.Globals.Set("obj", obj);
                        UserData.RegisterProxyType<MyProxy, Room>(r => new MyProxy(room));
                        UserData.RegisterProxyType<ProxyPlayer, Player>(
                            r => new ProxyPlayer(character)
                        );

                        script.Globals["room"] = room;
                        script.Globals["player"] = character;
                        script.Globals["mob"] = mob;

                        var res = script.DoString(mob.Events.Enter);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("RoomActions.cs: " + ex);
                    }
                }

                if (
                    mob.Aggro
                    && mob.Status != CharacterStatus.Status.Fighting
                    && character.ConnectionId != "mob"
                )
                {
                    Services.Instance.Writer.WriteLine(
                        $"{mob.Name} attacks you!",
                        character.ConnectionId
                    );
                    Services.Instance.MobScripts.AttackPlayer(room, character, mob);
                }
            }
        }
    }
}
