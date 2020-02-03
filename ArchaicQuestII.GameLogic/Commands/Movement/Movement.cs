using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Movement
{
    public class Movement : IMovement
    {
        private readonly IWriteToClient _writeToClient;
        private readonly ICache _cache;
        public Movement(IWriteToClient writeToClient, ICache cache)
        {
            _writeToClient = writeToClient;
            _cache = cache;
        }
        public void Move(Room room, Player character, string direction)
        {
            var getExitToNextRoom = FindExit(room, direction);

            if (getExitToNextRoom == null)
            {
                _writeToClient.WriteLine("You can't go that way", character.ConnectionId);
            }

            var getNextRoom = _cache.GetRoom(getExitToNextRoom.AreaId);

            NotifyRoomLeft(room, character, direction);

            NotifyRoomEnter(getNextRoom, character, direction);

            UpdateCharactersLocation(getExitToNextRoom, character);
        }

        public Exit FindExit(Room room, string direction)
        {
            switch (direction)
            {
                case "North":
                    return room.Exits.North;
                case "North East":
                    return room.Exits.NorthEast;
                case "East":
                    return room.Exits.East;
                case "South East":
                    return room.Exits.SouthEast;
                case "South":
                    return room.Exits.South;
                case "South West":
                    return room.Exits.SouthWest;
                case "West":
                    return room.Exits.West;
                case "North West":
                    return room.Exits.NorthWest;
                default: {return null;}
            }
        }

        public void NotifyRoomLeft(Room room, Player character, string direction)
        {
            foreach (var player in room.Players)
            {
                if (character.Name != player.Name)
                {
                    _writeToClient.WriteLine($"{character.Name} walks {direction.ToLower()}.", player.ConnectionId);
                }
            }
        }

        public void NotifyRoomEnter(Room room, Player character, string direction)
        {
            foreach (var player in room.Players)
            {
                if (character.Name != player.Name)
                {
                    _writeToClient.WriteLine($"{character.Name} walks in.", player.ConnectionId);
                }            
            }
        }

        public void UpdateCharactersLocation(Exit exit, Player character)
        {
            character.RoomId = exit.AreaId;
        }

        public void MoveNorth(Room room, Player character)
        {
            throw new NotImplementedException();
        }
    }
}
