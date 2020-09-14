using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Movement
{
    public class Movement : IMovement
    {
        private readonly IWriteToClient _writeToClient;
        private readonly IRoomActions _roomActions;
        private readonly ICache _cache;
        private readonly IUpdateClientUI _updateUi;
        private readonly IDice _dice;



        public Movement(IWriteToClient writeToClient, ICache cache, IRoomActions roomActions, IUpdateClientUI updateUI, IDice dice)
        {
            _writeToClient = writeToClient;
            _cache = cache;
            _roomActions = roomActions;
            _updateUi = updateUI;
            _dice = dice;

        }
        public void Move(Room room, Player character, string direction)
        {
            switch (character.Status)
            {
                case CharacterStatus.Status.Fighting:
                case CharacterStatus.Status.Incapacitated:
                    _writeToClient.WriteLine("<p>NO WAY! you are fighting.</p>", character.ConnectionId);
                    return;
                case CharacterStatus.Status.Resting:
                    _writeToClient.WriteLine("<p>Nah... You feel too relaxed to do that..</p>", character.ConnectionId);
                    return;
                case CharacterStatus.Status.Sleeping:
                    _writeToClient.WriteLine("<p>In your dreams.</p>", character.ConnectionId);
                    return;
                case CharacterStatus.Status.Stunned:
                    _writeToClient.WriteLine("<p>You are too stunned to move.</p>", character.ConnectionId);
                    return;
            }

            if (CharacterCanMove(character) == false)
            {
                _writeToClient.WriteLine("<p>You are too exhausted to move.</p>", character.ConnectionId);
                return;
            }

            var getExitToNextRoom = FindExit(room, direction);

            if (getExitToNextRoom == null)
            {
                _writeToClient.WriteLine("<p>You can't go that way.</p>", character.ConnectionId);
                return;
            }
 
            var newRoomCoords = new Coordinates {
                X = getExitToNextRoom.Coords.X,
                Y = getExitToNextRoom.Coords.Y,
                Z = getExitToNextRoom.Coords.Z
            };
            var getNextRoom = _cache.GetRoom(getExitToNextRoom.AreaId, newRoomCoords);

            if (getNextRoom == null)
            {
                _writeToClient.WriteLine("<p>A mysterious force prevents you from going that way.</p>", character.ConnectionId);
                //TODO: log bug that the new room could not be found
                return;
            }

            //flee bug 
            character.Status = CharacterStatus.Status.Standing;

            NotifyRoomLeft(room, character, direction);

            NotifyRoomEnter(getNextRoom, character, direction);

            UpdateCharactersLocation(getExitToNextRoom, character);

             _roomActions.Look("", getNextRoom, character);

             _updateUi.GetMap(character, _cache.GetMap(getExitToNextRoom.AreaId));

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
                case "Down":
                    return room.Exits.Down;
                case "Up":
                    return room.Exits.Up;
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
            //Refactor?

            // remove player from room
            var oldRoom = _cache.GetRoom(character.RoomId);
            oldRoom.Players.Remove(character);

            //add player to room
            character.RoomId = exit.RoomId;
            var room = _cache.GetRoom(exit.RoomId);
            room.Players.Add(character);
        }

        public bool CharacterCanMove(Player character)
        {
            return character.Stats.MovePoints > 0;
        }

        public void Flee(Room room, Player character, string direction = "")
        {
            if (character.Status != CharacterStatus.Status.Fighting)
            {
                _writeToClient.WriteLine("<p>You're not in a fight.</p>", character.ConnectionId);
                return;
            }

            if (room.Exits.Down == null &&
                room.Exits.Up == null &&
                room.Exits.NorthWest == null &&
                room.Exits.North == null &&
                room.Exits.NorthEast == null &&
                room.Exits.East == null &&
                room.Exits.SouthEast == null &&
                room.Exits.South == null &&
                room.Exits.SouthWest == null &&
                room.Exits.West == null)
            {
                _writeToClient.WriteLine("<p>You have no where to go!</p>", character.ConnectionId);
                return;
            }

            var validExits = new List<Exit>();

            if (room.Exits.North != null)
            {
                validExits.Add(room.Exits.North);
            }
            if (room.Exits.NorthEast != null)
            {
                validExits.Add(room.Exits.NorthEast);
            }
            if (room.Exits.NorthWest != null)
            {
                validExits.Add(room.Exits.NorthWest);
            }
            if (room.Exits.East != null)
            {
                validExits.Add(room.Exits.East);
            }
            if (room.Exits.SouthEast != null)
            {
                validExits.Add(room.Exits.SouthEast);
            }
            if (room.Exits.South != null)
            {
                validExits.Add(room.Exits.South);
            }
            if (room.Exits.SouthWest != null)
            {
                validExits.Add(room.Exits.SouthWest);
            }
            if (room.Exits.West != null)
            {
                validExits.Add(room.Exits.West);
            }
            if (room.Exits.Up != null)
            {
                validExits.Add(room.Exits.Up);
            }
            if (room.Exits.Down != null)
            {
                validExits.Add(room.Exits.Down);
            }

            var getExitIndex = _dice.Roll(1, 0, validExits.Count - 1);

            character.Status = CharacterStatus.Status.Standing;
            _cache.RemoveCharFromCombat(character.Id.ToString());

            foreach (var mob in room.Mobs)
            {
                if (mob.Target == character.Name)
                {
                    character.Status = CharacterStatus.Status.Standing;
                    _cache.RemoveCharFromCombat(mob.Id.ToString());
                } 
            }

            //this could be buggy
            Move(room, character, validExits[getExitIndex].Name);

        }

        public void Sit(Player player, Room room, string target)
        {

            if (player.Status == CharacterStatus.Status.Sitting)
            {
                _writeToClient.WriteLine("<p>You are already sitting!</p>", player.ConnectionId);
                return;
            }

            if (target.Equals("sit", StringComparison.CurrentCultureIgnoreCase))
            {

                SetCharacterStatus(player, "is sitting here.", CharacterStatus.Status.Sitting);
                foreach (var pc in room.Players)
                {

                    if (pc.Id.Equals(player.Id))
                    {
                        _writeToClient.WriteLine("<p>You sit down.</p>", player.ConnectionId);
                    }
                    else
                    {
                        _writeToClient.WriteLine($"<p>{player.Name} sits down.</p>", pc.ConnectionId);
                    }
                }

            }
            else
            {

                var obj = room.Items.FirstOrDefault(x =>
                    x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase));

                if (obj == null)
                {
                    _writeToClient.WriteLine("<p>You can't sit on that.</p>", player.ConnectionId);
                    return;
                }

                SetCharacterStatus(player, $"is sitting down on {obj.Name.ToLower()}", CharacterStatus.Status.Resting);
                foreach (var pc in room.Players)
                {

                    if (pc.Id.Equals(player.Id))
                    {
                        _writeToClient.WriteLine($"<p>You sit down on {obj.Name.ToLower()}.</p>", player.ConnectionId);
                    }
                    else
                    {
                        _writeToClient.WriteLine($"<p>{player.Name} sits down on {obj.Name.ToLower()}.</p>",
                            pc.ConnectionId);
                    }
                }
            }

        }

        public void Stand(Player player, Room room, string target)
        {
            if (player.Status == CharacterStatus.Status.Standing)
            {
                _writeToClient.WriteLine("<p>You are already standing!</p>", player.ConnectionId);
                return;
            }

            var standMessage = "rises up.";
            if (player.Status == CharacterStatus.Status.Resting)
            {
                standMessage = $"arises from {(player.Gender == "Male" ? "his" : "her")} rest.";
            }
            else if (player.Status == CharacterStatus.Status.Sleeping)
            {
                standMessage = $"arises from {(player.Gender == "Male" ? "his" : "her")} slumber.";
            }

            SetCharacterStatus(player, "", CharacterStatus.Status.Standing);


            foreach (var pc in room.Players)
            {

                if (pc.Id.Equals(player.Id))
                {
                    _writeToClient.WriteLine("<p>You move quickly to your feet.</p>", player.ConnectionId);
                }
                else
                {
                    _writeToClient.WriteLine($"<p>{player.Name} {standMessage}</p>", pc.ConnectionId);
                }
            }
        }

        public void Sleep(Player player, Room room,  string target)
        {

            if (player.Status == CharacterStatus.Status.Sleeping)
            {
                _writeToClient.WriteLine("<p>You are already sleeping!</p>", player.ConnectionId);
                return;
            }

            SetCharacterStatus(player, "is sleeping nearby", CharacterStatus.Status.Sleeping);

            foreach (var pc in room.Players)
            {

                if (pc.Id.Equals(player.Id))
                {
                    _writeToClient.WriteLine("<p>You collapse into a deep sleep.</p>", player.ConnectionId);
                }
                else
                {
                    _writeToClient.WriteLine($"<p>{player.Name} collapses into a deep sleep.</p>", pc.ConnectionId);
                }
            }
        }

        public void Wake(Player player, Room room, string target)
        {
            if (player.Status != CharacterStatus.Status.Sleeping)
            {
                _writeToClient.WriteLine("<p>You are already standing!</p>", player.ConnectionId);
                return;
            }

            SetCharacterStatus(player, "", CharacterStatus.Status.Standing);

            foreach (var pc in room.Players)
            {

                if (pc.Id.Equals(player.Id))
                {
                    _writeToClient.WriteLine("<p>You move quickly to your feet.</p>", player.ConnectionId);
                }
                else
                {
                    _writeToClient.WriteLine($"<p>{player.Name} arises from {(player.Gender == "Male" ? "his" : "her")} slumber.</p>", pc.ConnectionId);
                }
            }
        }

        public void Rest(Player player, Room room, string target)
        {
            if (player.Status == CharacterStatus.Status.Resting)
            {
                _writeToClient.WriteLine("<p>You are already resting!</p>", player.ConnectionId);
                return;
            }

            SetCharacterStatus(player, "is sprawled out here", CharacterStatus.Status.Resting);

            foreach (var pc in room.Players)
            {

                if (pc.Id.Equals(player.Id))
                {
                    _writeToClient.WriteLine("<p>You sprawl out haphazardly.</p>", player.ConnectionId);
                }
                else
                {
                    _writeToClient.WriteLine($"<p>{player.Name} sprawls out haphazardly.</p>", pc.ConnectionId);
                }
            }
        }

        public void SetCharacterStatus(Player player, string longName, CharacterStatus.Status status)
        {
            player.Status = status;
            player.LongName = longName;
        }
    }
}
