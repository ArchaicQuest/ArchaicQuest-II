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
    public static class RoomActions
    {
        /// <summary>
        /// Helper to get area from room
        /// </summary>
        /// <param name="room">Room to get area from</param>
        public static Area.Area GetArea(this Room room)
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
        public static string GetRoomTitle(this Exit exit)
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
        public static string ValidExits(this Room room, bool verbose)
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
                        ? $"<tr class='verbose-exit-wrapper'><td class='verbose-exit'>{Helpers.DisplayDoor(room.Exits.North)} </td><td style='text-align:center; color:#fff'> - </td><td class='verbose-exit-name'><a href='javascript:void(0)' onclick='{clickEvent}'>{room.Exits.North.GetRoomTitle()}</a></td></tr>"
                        : Helpers.DisplayDoor(room.Exits.North)
                );
            }

            if (room.Exits.East != null)
            {
                const string clickEvent =
                    "window.dispatchEvent(new CustomEvent(\"post-to-server\", {\"detail\":\"e\"}))";
                exits.Add(
                    verbose
                        ? $"<tr class='verbose-exit-wrapper'><td class='verbose-exit'>{Helpers.DisplayDoor(room.Exits.East)} </td><td style='text-align:center; color:#fff'> - </td><td class='verbose-exit-name'><a href='javascript:void(0)' onclick='{clickEvent}'>{room.Exits.East.GetRoomTitle()}</a></td></tr>"
                        : Helpers.DisplayDoor(room.Exits.East)
                );
            }

            if (room.Exits.South != null)
            {
                const string clickEvent =
                    "window.dispatchEvent(new CustomEvent(\"post-to-server\", {\"detail\":\"s\"}))";
                exits.Add(
                    verbose
                        ? $"<tr class='verbose-exit-wrapper'><td class='verbose-exit'>{Helpers.DisplayDoor(room.Exits.South)} </td><td style='text-align:center; color:#fff'> - </td><td class='verbose-exit-name'><a href='javascript:void(0)' onclick='{clickEvent}'>{room.Exits.South.GetRoomTitle()}</a></td></tr>"
                        : Helpers.DisplayDoor(room.Exits.South)
                );
            }

            if (room.Exits.West != null)
            {
                const string clickEvent =
                    "window.dispatchEvent(new CustomEvent(\"post-to-server\", {\"detail\":\"w\"}))";
                exits.Add(
                    verbose
                        ? $"<tr class='verbose-exit-wrapper'><td class='verbose-exit'>{Helpers.DisplayDoor(room.Exits.West)} </td><td style='text-align:center; color:#fff'> - </td><td class='verbose-exit-name'><a href='javascript:void(0)' onclick='{clickEvent}'>{room.Exits.West.GetRoomTitle()}</a></td></tr>"
                        : Helpers.DisplayDoor(room.Exits.West)
                );
            }

            if (room.Exits.NorthEast != null)
            {
                const string clickEvent =
                    "window.dispatchEvent(new CustomEvent(\"post-to-server\", {\"detail\":\"ne\"}))";
                exits.Add(
                    verbose
                        ? $"<tr class='verbose-exit-wrapper'><td class='verbose-exit'>{Helpers.DisplayDoor(room.Exits.NorthEast)}  </td><td style='text-align:center; color:#fff'> - </td><td class='verbose-exit-name'><a href='javascript:void(0)' onclick='{clickEvent}'>{room.Exits.NorthEast.GetRoomTitle()}</a></td></tr>"
                        : Helpers.DisplayDoor(room.Exits.NorthEast)
                );
            }

            if (room.Exits.SouthEast != null)
            {
                const string clickEvent =
                    "window.dispatchEvent(new CustomEvent(\"post-to-server\", {\"detail\":\"se\"}))";
                exits.Add(
                    verbose
                        ? $"<tr class='verbose-exit-wrapper'><td class='verbose-exit'>{Helpers.DisplayDoor(room.Exits.SouthEast)}  </td><td style='text-align:center; color:#fff'> - </td><td class='verbose-exit-name'><a href='javascript:void(0)' onclick='{clickEvent}'>{room.Exits.SouthEast.GetRoomTitle()}</a></td></tr>"
                        : Helpers.DisplayDoor(room.Exits.SouthEast)
                );
            }

            if (room.Exits.SouthWest != null)
            {
                const string clickEvent =
                    "window.dispatchEvent(new CustomEvent(\"post-to-server\", {\"detail\":\"sw\"}))";
                exits.Add(
                    verbose
                        ? $"<tr class='verbose-exit-wrapper'><td class='verbose-exit'>{Helpers.DisplayDoor(room.Exits.SouthWest)}  </td><td style='text-align:center; color:#fff'> - </td><td class='verbose-exit-name'><a href='javascript:void(0)' onclick='{clickEvent}'>{room.Exits.SouthWest.GetRoomTitle()}</a></td></tr>"
                        : Helpers.DisplayDoor(room.Exits.SouthWest)
                );
            }

            if (room.Exits.NorthWest != null)
            {
                const string clickEvent =
                    "window.dispatchEvent(new CustomEvent(\"post-to-server\", {\"detail\":\"nw\"}))";
                exits.Add(
                    verbose
                        ? $"<tr class='verbose-exit-wrapper'><td class='verbose-exit'>{Helpers.DisplayDoor(room.Exits.NorthWest)}  </td> <td style='text-align:center; color:#fff'> - </td><td class='verbose-exit-name'><a href='javascript:void(0)' onclick='{clickEvent}'>{room.Exits.NorthWest.GetRoomTitle()}</a></td></tr>"
                        : Helpers.DisplayDoor(room.Exits.NorthWest)
                );
            }

            if (room.Exits.Down != null)
            {
                const string clickEvent =
                    "window.dispatchEvent(new CustomEvent(\"post-to-server\", {\"detail\":\"d\"}))";
                exits.Add(
                    verbose
                        ? $"<tr class='verbose-exit-wrapper'><td class='verbose-exit'>{Helpers.DisplayDoor(room.Exits.Down)}  </td> <td style='text-align:center; color:#fff'> - </td><td class='verbose-exit-name'><a href='javascript:void(0)' onclick='{clickEvent}'>{room.Exits.Down.GetRoomTitle()}</a></td></tr>"
                        : Helpers.DisplayDoor(room.Exits.Down)
                );
            }

            if (room.Exits.Up != null)
            {
                const string clickEvent =
                    "window.dispatchEvent(new CustomEvent(\"post-to-server\", {\"detail\":\"u\"}))";
                exits.Add(
                    verbose
                        ? $"<tr class='verbose-exit-wrapper'><td class='verbose-exit'>{Helpers.DisplayDoor(room.Exits.Up)}  </td> <td style='text-align:center; color:#fff'> - </td><td class='verbose-exit-name'><a href='javascript:void(0)' onclick='{clickEvent}'>{room.Exits.Up.GetRoomTitle()}</a></td></tr>"
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

        public static void OnPlayerLeaveEvent(this Room room, Player character)
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

        public static async Task OnPlayerEnterEvent(this Room room, Player character)
        {
            // force the on enter event to fire after Look
            await Task.Delay(125);

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
                    Services.Instance.Writer.WriteLine($"{mob.Name} attacks you!", character);
                    Services.Instance.MobScripts.AttackPlayer(room, character, mob);
                }
            }
        }

        public static Exit GetExit(this Room room, string direction)
        {
            Exit getExitToNextRoom = null;

            switch (direction)
            {
                case "north":
                case "n":
                    getExitToNextRoom = room.Exits.North;
                    break;
                case "south":
                case "s":
                    getExitToNextRoom = room.Exits.South;
                    break;
                case "east":
                case "e":
                    getExitToNextRoom = room.Exits.East;
                    break;
                case "west":
                case "w":
                    getExitToNextRoom = room.Exits.West;
                    break;
                case "southeast":
                case "se":
                    getExitToNextRoom = room.Exits.SouthEast;
                    break;
                case "southwest":
                case "sw":
                    getExitToNextRoom = room.Exits.SouthWest;
                    break;
                case "northeast":
                case "ne":
                    getExitToNextRoom = room.Exits.NorthEast;
                    break;
                case "northwest":
                case "nw":
                    getExitToNextRoom = room.Exits.NorthWest;
                    break;
                case "down":
                case "d":
                    getExitToNextRoom = room.Exits.Down;
                    break;
                case "up":
                case "u":
                    getExitToNextRoom = room.Exits.Up;
                    break;
            }

            return getExitToNextRoom;
        }
    }
}
