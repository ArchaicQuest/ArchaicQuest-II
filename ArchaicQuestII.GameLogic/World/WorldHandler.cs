using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Client;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;
using MoonSharp.Interpreter;

namespace ArchaicQuestII.GameLogic.World;

public class WorldHandler : IWorldHandler
{
    private readonly IClientHandler _clientHandler;
    private readonly ICharacterHandler _characterHandler;

    private readonly IDataBase _dataBase;
    
    private readonly Weather _weather;

    public Time Time { get; }

    private readonly ConcurrentDictionary<string, Room.Room> _originalRoomCache = new();
    private readonly ConcurrentDictionary<string, Room.Room> _roomCache = new();
    private readonly ConcurrentDictionary<string, string> _mapCache = new();

    public WorldHandler(ICoreHandler coreHandler)
    {
        _clientHandler = coreHandler.Client;
        _characterHandler = coreHandler.Character;
        _dataBase = coreHandler.Db;
        
        Time = new Time();
        _weather = new Weather();
    }

    #region CACHE

    public void ClearCache()
    {
        _roomCache.Clear();
        _originalRoomCache.Clear();
        _mapCache.Clear();
    }

    public void AddMap(string areaId, string map)
    {
        _mapCache.TryAdd(areaId, map);
    }

    public string GetMap(string areaId)
    {
        _mapCache.TryGetValue(areaId, out var map);

        return map;
    }

    public bool AddRoom(string id, Room.Room room)
    {
        return _roomCache.TryAdd(id, room);
    }

    public bool AddOriginalRoom(string id, Room.Room room)
    {
        return _originalRoomCache.TryAdd(id, room);
    }
    
    public Room.Room GetRoom(string id)
    {
        _roomCache.TryGetValue(id, out var room);

        return room;
    }

    public Room.Room GetOriginalRoom(string id)
    {
        _originalRoomCache.TryGetValue(id, out var room);

        return room;
    }

    public List<Room.Room> GetAllRooms()
    {
        var room = _roomCache.Values.ToList();

        return room;
    }

    public List<Room.Room> GetOriginalRooms()
    {
        return _originalRoomCache.Values.ToList();
    }


    public List<Room.Room> GetAllRoomsInArea(int id)
    {
        var room = _roomCache.Values.Where(x => x.AreaId.Equals(id)).ToList();

        return room;
    }

    public List<Room.Room> GetAllRoomsToRepop()
    {
        var room = _roomCache.Values.ToList();

        return room;
    }
    
    public bool UpdateRoom(string id, Room.Room room, Player player)
    {
        var existingRoom = room;
        var newRoom = room;
        newRoom.Players.Add(player);


        return _roomCache.TryUpdate(id, existingRoom, newRoom);
    }
    
    #endregion

    #region AREA
    
    /// <summary>
    /// Display notice when player enters a new area
    /// </summary>
    /// <param name="player">Player entering area</param>
    /// <param name="room">Room that was entered</param>
    public void AreaEntered(Player player, Room.Room room)
    {
        var area = _dataBase.GetCollection<Area.Area>(DataBase.Collections.Area).FindById(room.AreaId);

        _clientHandler.WriteLine($"<p>You have traversed into <b>{area.Title}</b>.", player.ConnectionId);
    }


    public List<Area.Area> GetAllAreas()
    {
        return _dataBase.GetCollection<Area.Area>(DataBase.Collections.Area).FindAll().ToList();
    }
    
    
    #endregion

    #region ROOMS

    public Room.Room MapRoom(Room.Room room)
        {
            var newRoom = new Room.Room
            {
                Title = room.Title,
                Description = room.Description,
                AreaId = room.AreaId,
                Coords = new Coordinates
                {
                    X = room.Coords.X,
                    Y = room.Coords.Y,
                    Z = room.Coords.Z
                },
                Exits = room.Exits,
                Emotes = room.Emotes,
                InstantRePop = room.InstantRePop,
                UpdateMessage = room.UpdateMessage,
                Items = room.Items,
                Mobs = room.Mobs,
                RoomObjects = room.RoomObjects,
                Type = room.Type,
                Terrain = room.Terrain,
                DateUpdated = DateTime.Now,
                DateCreated = DateTime.Now,

            };

            MapRoomId(newRoom);

            if (room.Id != -1)
            {
                newRoom.Id = room.Id;
            }

            return newRoom;

        }
        public void MapRoomId(Room.Room room)
        {
            var northRoom = room.Exits.North?.Coords;
            if (northRoom != null)
            {
                room.Exits.North.RoomId = GetRoomFromCoords(northRoom, room.AreaId) != null ? GetRoomFromCoords(northRoom, room.AreaId).Id : -1;
            }

            var eastRoom = room.Exits.East?.Coords;
            if (eastRoom != null)
            {
                room.Exits.East.RoomId = GetRoomFromCoords(eastRoom, room.AreaId) != null ? GetRoomFromCoords(eastRoom, room.AreaId).Id : -1;
            }

            var southRoom = room.Exits.South?.Coords;
            if (southRoom != null)
            {
                room.Exits.South.RoomId = GetRoomFromCoords(southRoom, room.AreaId) != null ? GetRoomFromCoords(southRoom, room.AreaId).Id : -1;
            }

            var westRoom = room.Exits.West?.Coords;
            if (westRoom != null)
            {
                room.Exits.West.RoomId = GetRoomFromCoords(westRoom, room.AreaId) != null ? GetRoomFromCoords(westRoom, room.AreaId).Id : -1;
            }

            var NWRoom = room.Exits.NorthWest?.Coords;
            if (NWRoom != null)
            {
                room.Exits.NorthWest.RoomId = GetRoomFromCoords(NWRoom, room.AreaId) != null ? GetRoomFromCoords(NWRoom, room.AreaId).Id : -1;
            }

            var NERoom = room.Exits.NorthEast?.Coords;
            if (NERoom != null)
            {
                room.Exits.NorthEast.RoomId = GetRoomFromCoords(NERoom, room.AreaId) != null ? GetRoomFromCoords(NERoom, room.AreaId).Id : -1;
            }

            var SERoom = room.Exits.SouthEast?.Coords;
            if (SERoom != null)
            {
                room.Exits.SouthEast.RoomId = GetRoomFromCoords(SERoom, room.AreaId) != null ? GetRoomFromCoords(SERoom, room.AreaId).Id : -1;
            }

            var SWRoom = room.Exits.SouthWest?.Coords;
            if (SWRoom != null)
            {
                room.Exits.SouthWest.RoomId = GetRoomFromCoords(SWRoom, room.AreaId) != null ? GetRoomFromCoords(SWRoom, room.AreaId).Id : -1;
            }

            var DRoom = room.Exits.Down?.Coords;
            if (DRoom != null)
            {
                room.Exits.Down.RoomId = GetRoomFromCoords(DRoom, room.AreaId) != null ? GetRoomFromCoords(DRoom, room.AreaId).Id : -1;
            }

            var URoom = room.Exits.Up?.Coords;
            if (URoom != null)
            {
                room.Exits.Up.RoomId = GetRoomFromCoords(URoom, room.AreaId) != null ? GetRoomFromCoords(URoom, room.AreaId).Id : -1;
            }
        }

        public Room.Room GetRoomFromCoords(Coordinates coords, int areaId)
        {
            return _dataBase.GetCollection<Room.Room>(DataBase.Collections.Room).FindOne(x => x.AreaId.Equals(areaId) && x.Coords.X.Equals(coords.X) && x.Coords.Y.Equals(coords.Y) && x.Coords.Z.Equals(coords.Z));
        }
    
    public bool RoomIsDark(Player player, Room.Room room)
    {
        if (room.IsLit)
            return false;
        
        if (player.Affects.DarkVision)
            return false;
        
        if (player.Equipped.Light != null)
            return false;
        
        if (room.Players.Any(pc => pc.Equipped.Light != null))
        {
            return false;
        }

        return room.Type is Room.Room.RoomType.Underground or Room.Room.RoomType.Inside || IsNightTime();
    }

    /// <summary>
    /// Helper to get area from room
    /// </summary>
    /// <param name="room">Room to get area from</param>
    public Area.Area GetRoomArea(Room.Room room)
    {
        return _dataBase.GetCollection<Area.Area>(DataBase.Collections.Area).FindById(room.AreaId);
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
        var room = GetRoom(roomId);

        return room.Title;
    }

    /// <summary>
    /// Displays valid exits
    /// </summary>
    public string FindValidExits(Room.Room room, bool verbose)
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
        
        if (room.Exits.East != null)
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
    public void RoomChange(Player player, Room.Room oldRoom, Room.Room newRoom)
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

        _clientHandler.GetMap(player, GetMap($"{newRoom.AreaId}{newRoom.Coords.Z}"));
        _clientHandler.UpdateMoves(player);
        
        player.Buffer.Enqueue("look");
    }

    /// <summary>
    /// Updates the characters location
    /// </summary>
    /// <param name="character"></param>
    /// <param name="oldRoom"></param>
    /// <param name="newRoom"></param>
    private void UpdateCharactersLocation(Player character, Room.Room oldRoom, Room.Room newRoom)
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

    private void EnterRoom(Player character, Room.Room toRoom, Room.Room fromRoom)
    {
        var direction = "from nowhere";
        var movement = "appears";

        if (toRoom.Exits != null)
        {
            if (toRoom.Exits.Down?.RoomId == fromRoom.Id)
                direction = "down";
            if (toRoom.Exits.Up?.RoomId == fromRoom.Id)
                direction = "up";
            if (toRoom.Exits.North?.RoomId == fromRoom.Id)
                direction = "in from the north";
            if (toRoom.Exits.South?.RoomId == fromRoom.Id)
                direction = "in form the south";
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
            case CharacterStatus.Status.Fleeing:
                movement = "flees";
                character.Status = CharacterStatus.Status.Standing;
                break;
            case CharacterStatus.Status.Standing:
                _clientHandler.PlaySound("walk", character);
                movement = "walks";
                break;
        }

        foreach (var p in fromRoom.Players.Where(p => character.Name != p.Name))
        {
            _clientHandler.WriteLine(
                $"<span class='{(character.ConnectionId != "mob" ? "player" : "mob")}'>{character.Name} {movement} {direction}.</span>",
                p.ConnectionId);
        }
    }

    private void ExitRoom(Player characterBase, Room.Room toRoom, Room.Room fromRoom)
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
            _clientHandler.WriteLine(
                $"<span class='{(characterBase.ConnectionId != "mob" ? "player" : "mob")}'>{characterBase.Name} {movement} {direction}.</span>",
                p.ConnectionId);
        }
    }

    private void OnPlayerLeaveEvent(Room.Room room, Player character)
    {
        foreach (var mob in room.Mobs.Where(mob => !string.IsNullOrEmpty(mob.Events.Leave)))
        {
            UserData.RegisterType<MobScripts>();

            var script = new Script();

            var obj = UserData.Create(_characterHandler.MobScripts);
            script.Globals.Set("obj", obj);
            UserData.RegisterProxyType<MyProxy, Room.Room>(r => new MyProxy(room));
            UserData.RegisterProxyType<ProxyPlayer, Player>(r => new ProxyPlayer(character));
                
            script.Globals["room"] = room;
            script.Globals["player"] = character;
            script.Globals["mob"] = mob;
                
            var res = script.DoString(mob.Events.Leave);
        }
    }

    private void OnPlayerEnterEvent(Room.Room room, Player character)
    {
        foreach (var mob in room.Mobs)
        {
            if (!string.IsNullOrEmpty(mob.Events.Enter))
            {
                try
                {
                    UserData.RegisterType<MobScripts>();

                    var script = new Script();

                    var obj = UserData.Create(_characterHandler.MobScripts);
                    script.Globals.Set("obj", obj);
                    UserData.RegisterProxyType<MyProxy, Room.Room>(r => new MyProxy(room));
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
                _clientHandler.WriteLine($"{mob.Name} attacks you!", character.ConnectionId);
                _characterHandler.MobScripts.AttackPlayer(room, character, mob);
            }
        }
    }

    #endregion

    #region WEATHER

    public string SimulateWeatherTransitions()
    {
        return _weather.SimulateWeatherTransitions(IsNightTime());
    }

    #endregion

    #region TIME

    public void DisplayTimeOfDayMessage()
    {
        var players = _characterHandler.GetPlayerCache();
        var tickMessage = Time.UpdateTime();

        foreach (var pc in players.Values)
        {
            var room = GetRoom(pc.RoomId);

            if (room == null)
            {
                return;
            }

            if (room.Terrain != Room.Room.TerrainType.Inside && room.Terrain != Room.Room.TerrainType.Underground && !string.IsNullOrEmpty(tickMessage))
            {
                _clientHandler.WriteLine($"<span class='time-of-day'>{tickMessage}</span>", pc.ConnectionId);
            }
        }
    }
    
    public bool IsNightTime()
    {
        return Convert.ToInt32(Math.Floor(Time.GameTime.Hours)) >= 6 && Convert.ToInt32(Math.Floor(Time.GameTime.Hours)) <= 18;
    }

    #endregion
}