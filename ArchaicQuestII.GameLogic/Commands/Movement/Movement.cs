using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Combat;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.World.Room;
using MoonSharp.Interpreter;

namespace ArchaicQuestII.GameLogic.Commands.Movement
{
 
    public class Movement : IMovement
    {
        private readonly IWriteToClient _writeToClient;
        private readonly IRoomActions _roomActions;
        private readonly ICache _cache;
        private readonly IUpdateClientUI _updateUi;
        private readonly IDice _dice;
        private readonly IMobScripts _mobScripts;

        //test
        private readonly ICombat _combat;



        public Movement(IWriteToClient writeToClient, ICache cache, IRoomActions roomActions, IUpdateClientUI updateUI, IDice dice, ICombat combat, IMobScripts mobScripts)
        {
            _writeToClient = writeToClient;
            _cache = cache;
            _roomActions = roomActions;
            _updateUi = updateUI;
            _dice = dice;
            _combat = combat;
            _mobScripts = mobScripts;
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

            
            var nextRoomKey =
                $"{getExitToNextRoom.AreaId}{getExitToNextRoom.Coords.X}{getExitToNextRoom.Coords.Y}{getExitToNextRoom.Coords.Z}";
            var getNextRoom = _cache.GetRoom(nextRoomKey);

            if (getNextRoom == null)
            {
                _writeToClient.WriteLine("<p>A mysterious force prevents you from going that way.</p>", character.ConnectionId);
                //TODO: log bug that the new room could not be found
                return;
            }

            //flee bug 
            character.Status = CharacterStatus.Status.Standing;

            OnPlayerLeaveEvent(room, character);
            NotifyRoomLeft(room, character, direction);

            NotifyRoomEnter(getNextRoom, character, direction);

            UpdateCharactersLocation(getExitToNextRoom, character, room);

            character.Attributes.Attribute[EffectLocation.Moves] -= 1;

            if (character.Attributes.Attribute[EffectLocation.Moves] < 0)
            {
                character.Attributes.Attribute[EffectLocation.Moves] = 0;
            }

            if (character.ConnectionId == "mob")
            {
                return;

            }

            _roomActions.Look("", getNextRoom, character);

            OnPlayerEnterEvent(getNextRoom, character); 

         
            _updateUi.GetMap(character, _cache.GetMap(getExitToNextRoom.AreaId));
            _updateUi.UpdateMoves(character);

            if (character.Followers.Count >= 1)
            {
                foreach (var follower in character.Followers)
                {
                    if (room.Players.Contains(follower))
                    {
                        Move(room, follower, direction);
                    }
                }
            }

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
                default: { return null; }
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

        public void OnPlayerLeaveEvent(Room room, Player character)
        {
            foreach (var mob in room.Mobs)
            {

                if (!string.IsNullOrEmpty(mob.Events.Leave))
                {
                    UserData.RegisterType<MobScripts>();

                    Script script = new Script();

                    DynValue obj = UserData.Create(_mobScripts);
                    script.Globals.Set("obj", obj);
                    UserData.RegisterProxyType<MyProxy, Room>(r => new MyProxy(room));
                    UserData.RegisterProxyType<ProxyPlayer, Player>(r => new ProxyPlayer(character));


                    script.Globals["room"] = room;

                    script.Globals["player"] = character;
                    script.Globals["mob"] = mob;


                    DynValue res = script.DoString(mob.Events.Leave);
                }


            }
        }

        public void OnPlayerEnterEvent(Room room, Player character)
        {
            foreach (var mob in room.Mobs)
            {

                //             string scriptCode = @"    
                //                   -- defines a function
                //                   function greet (room, player, mob)

                //                            obj.updateInv(player)
                //                             obj.Say('hello', 0, room, player)
                //                             if obj.isInRoom(room, player) then obj.Say('I have a quest for you', 1000, room, player) end
                //                             obj.Say('you have to kill some goblins', 1000, room, player)
                //                             obj.Say('you have to kill some goblins', 10000, room, player)
                //obj.Say('What you say', 5000, room, player)
                //                  return ('Hello there ' .. obj.getName(player) .. ' check your inventory')
                //                   end

                //             return greet(room, player, mob)

                //                    ";

                if (!string.IsNullOrEmpty(mob.Events.Enter))
                {
                    UserData.RegisterType<MobScripts>();

                    Script script = new Script();

                    DynValue obj = UserData.Create(_mobScripts);
                    script.Globals.Set("obj", obj);
                    UserData.RegisterProxyType<MyProxy, Room>(r => new MyProxy(room));
                    UserData.RegisterProxyType<ProxyPlayer, Player>(r => new ProxyPlayer(character));


                    script.Globals["room"] = room;

                    script.Globals["player"] = character;
                    script.Globals["mob"] = mob;


                    DynValue res = script.DoString(mob.Events.Enter);
                }


            }
        }

        public void UpdateCharactersLocation(Exit exit, Player character, Room currentRoom)
        {
            //Refactor?
            if (character.ConnectionId != "mob")
            {
                // remove player from room
                var oldRoom = _cache.GetRoom($"{currentRoom.AreaId}{currentRoom.Coords.X}{currentRoom.Coords.Y}{currentRoom.Coords.Z}");
                oldRoom.Players.Remove(character);

                //add player to room
                character.RoomId = $"{exit.AreaId}{exit.Coords.X}{exit.Coords.Y}{exit.Coords.Z}"; 
                var room = _cache.GetRoom($"{exit.AreaId}{exit.Coords.X}{exit.Coords.Y}{exit.Coords.Z}");
                room.Players.Add(character);
            }
            else
            {
                // remove mob from room
                var oldRoom = _cache.GetRoom($"{currentRoom.AreaId}{currentRoom.Coords.X}{currentRoom.Coords.Y}{currentRoom.Coords.Z}");
                oldRoom.Mobs.Remove(character);

                //add mob to room
                character.RoomId = $"{exit.AreaId}{exit.Coords.X}{exit.Coords.Y}{exit.Coords.Z}"; 
                var room = _cache.GetRoom($"{exit.AreaId}{exit.Coords.X}{exit.Coords.Y}{exit.Coords.Z}");
                room.Mobs.Add(character);
            }
        }

        public bool CharacterCanMove(Player character)
        {
            return character.ConnectionId == "mob" || character.Attributes.Attribute[EffectLocation.Moves] > 0;
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

        public void Sleep(Player player, Room room, string target)
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

        public void Group(Player player, Room room, string target)
        {
            if((string.IsNullOrEmpty(target) || target.Equals("group", StringComparison.CurrentCultureIgnoreCase)) && !player.grouped)
            {
                _writeToClient.WriteLine($"<p>But you are not the member of a group!</p>", player.ConnectionId);
                return;
            }

            if (target.Equals(player.Name, StringComparison.CurrentCultureIgnoreCase))
            {
                _writeToClient.WriteLine($"<p>You can't group yourself.</p>", player.ConnectionId);
                return;
            }

            if (target.Equals("group", StringComparison.CurrentCultureIgnoreCase) && player.grouped)
            {
                _writeToClient.WriteLine($"<p>Grouped with:</p>", player.ConnectionId);

                var sb = new StringBuilder();
                sb.Append("<ul>");

                var foundLeader = _cache.GetPlayerCache()
          .FirstOrDefault(x => x.Value.Name.StartsWith(player.Following ?? player.Name, StringComparison.CurrentCultureIgnoreCase));
                sb.Append($"<li>Lvl: {foundLeader.Value.Level} {foundLeader.Value.Name} (Leader)  <span class='group-hp' title='Hit points'>{foundLeader.Value.Attributes.Attribute[EffectLocation.Hitpoints]}</span>/<span class='group-mana' title='Mana points'>{foundLeader.Value.Attributes.Attribute[EffectLocation.Mana]}</span>/<span class='group-moves' title='Move points'>{foundLeader.Value.Attributes.Attribute[EffectLocation.Moves]}</span></li>");

                foreach (var follower in foundLeader.Value.Followers.Where(x => x.grouped))
                {
                    sb.Append($"<li>Lvl: {follower.Level} {follower.Name} <span class='group-hp' title='Hit points'>{follower.Attributes.Attribute[EffectLocation.Hitpoints]}</span>/<span class='group-mana' title='Mana points'>{follower.Attributes.Attribute[EffectLocation.Mana]}</span>/<span class='group-moves' title='Move points'>{follower.Attributes.Attribute[EffectLocation.Moves]}</span></li>");
                }

                sb.Append("</ul>");
                _writeToClient.WriteLine(sb.ToString(), player.ConnectionId);

                return;
            }

            var foundPlayer = player.Followers
            .FirstOrDefault(x => x.Name.StartsWith(target, StringComparison.CurrentCultureIgnoreCase));

            if(foundPlayer == null)
            {
                _writeToClient.WriteLine("<p>They are not following you!</p>", player.ConnectionId);
                return;
            }

            if (foundPlayer == player)
            {
                _writeToClient.WriteLine("<p>You can't group with yourself!</p>", player.ConnectionId);
                return;
            }

            if (foundPlayer.grouped)
            {
                foundPlayer.grouped = false;
 
                _writeToClient.WriteLine($"<p>{foundPlayer.Name} is no longer a member of your group.</p>", player.ConnectionId);
                _writeToClient.WriteLine($"<p>You are no longer a member of {player.Name}'s group.</p>", foundPlayer.ConnectionId);
                return;

            }


            foundPlayer.grouped = true;
            player.grouped = true;
            _writeToClient.WriteLine($"<p>{foundPlayer.Name} is now a member of your group.</p>", player.ConnectionId);
            _writeToClient.WriteLine($"<p>You are now a member of {player.Name}'s group.</p>", foundPlayer.ConnectionId);

            foreach (var pc in room.Players)
            {
                if(pc.Id == player.Id || pc.Id == foundPlayer.Id)
                {
                    continue;
                }
                _writeToClient.WriteLine($"<p>{foundPlayer.Name} is now a member of {player.Name}'s group.</p>", pc.ConnectionId);

            }
 
        }
            public void Follow(Player player, Room room, string target)
        {
            if(target.Equals("self", StringComparison.CurrentCultureIgnoreCase))
            { 

                var leader = _cache.GetPlayerCache()
              .FirstOrDefault(x => x.Value.Name.StartsWith(player.Following ?? player.Name, StringComparison.CurrentCultureIgnoreCase));

                _writeToClient.WriteLine($"<p>You stop following {leader.Value.Name}.</p>", player.ConnectionId);
                _writeToClient.WriteLine($"<p>{player.Name} stops following you.</p>", leader.Value.ConnectionId);
                leader.Value.Followers.Remove(player);
                if (leader.Value.Followers.Count == 0)
                {
                    leader.Value.grouped = false;
                }
                player.Following = null;
                player.grouped = false;

              
                return;
            }

            var foundPlayer = room.Players
                .FirstOrDefault(x => x.Name.StartsWith(target, StringComparison.CurrentCultureIgnoreCase));

            if (foundPlayer == null)
            {
                _writeToClient.WriteLine("<p>You don't see them here.</p>", player.ConnectionId);
                return;
            }

            if(foundPlayer.Followers.Contains(player))
            {
                _writeToClient.WriteLine($"<p>You are already following {foundPlayer.Name}.</p>", player.ConnectionId);
                return;
            }

            _writeToClient.WriteLine($"<p>{player.Name} now follows you.</p>", foundPlayer.ConnectionId);
            _writeToClient.WriteLine($"<p>You are now following {foundPlayer.Name}.</p>", player.ConnectionId);
           
            player.Following = foundPlayer.Name;
            foundPlayer.Followers.Add(player);
        }

        public void SetCharacterStatus(Player player, string longName, CharacterStatus.Status status)
        {
            player.Status = status;
            player.LongName = longName;
        }
    }
}
