using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Combat;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.World.Area;
using ArchaicQuestII.GameLogic.World.Room;
using MoonSharp.Interpreter;

namespace ArchaicQuestII.GameLogic.Commands.Movement
{

    public class Movement : IMovement
    {
        private readonly IWriteToClient _writeToClient;
        private readonly IRoomActions _roomActions;
        private readonly IAreaActions _areaActions;
        private readonly ICache _cache;
        private readonly IUpdateClientUI _updateUi;
        private readonly IDice _dice;
        private readonly IMobScripts _mobScripts;

        //test
        private readonly ICombat _combat;



        public Movement(IWriteToClient writeToClient, ICache cache, IRoomActions roomActions, IAreaActions areaActions, IUpdateClientUI updateUI, IDice dice, ICombat combat, IMobScripts mobScripts)
        {
            _writeToClient = writeToClient;
            _cache = cache;
            _roomActions = roomActions;
            _areaActions = areaActions;
            _updateUi = updateUI;
            _dice = dice;
            _combat = combat;
            _mobScripts = mobScripts;
        }

        public void Move(Room room, Player character, string direction, bool silence = false, bool flee = false)
        {

            switch (character.Status)
            {
                case CharacterStatus.Status.Fighting:
                case CharacterStatus.Status.Incapacitated:
                    _writeToClient.WriteLine("<p>NO WAY! you are fighting.</p>", character.ConnectionId);
                    return;
                case CharacterStatus.Status.Resting:
                    _writeToClient.WriteLine("<p>Nah... You feel too relaxed to do that.</p>", character.ConnectionId);
                    return;
                case CharacterStatus.Status.Sitting:
                    _writeToClient.WriteLine("<p>You can't do that while sitting.</p>", character.ConnectionId);
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

            if (getExitToNextRoom.Closed)
            {
                _writeToClient.WriteLine("<p>The door is close.</p>", character.ConnectionId);
                //TODO: log bug that the new room could not be found
                return;
            }

            if (!flee)
            {
                if (room.Mobs.Any())
                {
                    OnPlayerLeaveEvent(room, character);
                }

                if (room.Players.Any() && !silence)
                {
                    NotifyRoomLeft(room, character, direction);
                }
            }

            if (getNextRoom.Players.Any() && !silence)
            {
                NotifyRoomEnter(getNextRoom, character, direction);
            }

            UpdateCharactersLocation(getExitToNextRoom, character, room);

            if (string.IsNullOrEmpty(character.Mounted.Name))
            {

                character.Attributes.Attribute[EffectLocation.Moves] -= 1;

                if (character.Attributes.Attribute[EffectLocation.Moves] < 0)
                {
                    character.Attributes.Attribute[EffectLocation.Moves] = 0;
                }
            }

            if (character.ConnectionId == "mob")
            {
                // do Aggro mob
                if (!getNextRoom.Players.Any()) return;
                if (character.Agro && character.Status != CharacterStatus.Status.Fighting)
                {
                    //character.Buffer.Clear();
                    _writeToClient.WriteLine($"{character.Name} attacks you!", getNextRoom.Players.FirstOrDefault()?.ConnectionId);
                    _mobScripts.AttackPlayer(getNextRoom, getNextRoom.Players.FirstOrDefault(), character);
                }

                return;

            }
            
            _updateUi.PlaySound("walk", character);

            _roomActions.Look("", getNextRoom, character, character.Config.Brief);

            if (getNextRoom.Mobs.Any())
            {
                OnPlayerEnterEvent(getNextRoom, character);
            }

  
            _updateUi.GetMap(character, _cache.GetMap($"{getExitToNextRoom.AreaId}{getExitToNextRoom.Coords.Z}"));
            _updateUi.UpdateMoves(character);

            if (character.Followers.Count >= 1)
            {
                foreach (var follower in character.Followers)
                {
                    if (room.Players.Contains(follower) || room.Mobs.Contains(follower))
                    {
                        Move(room, follower, direction);
                    }
                }
            }

            if (!string.IsNullOrEmpty(character.Mounted.Name))
            {
                var mountedMob = room.Mobs.FirstOrDefault(x => !string.IsNullOrEmpty(x.Mounted.MountedBy) && x.Mounted.MountedBy.Equals(character.Name));

                if (mountedMob != null)
                {
                    Move(room, mountedMob, direction, true);
                }
            }

            character.Pose = "";

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
                    _writeToClient.WriteLine(MovementAdjective(character, false, direction.ToLower()), player.ConnectionId);
                }
            }
        }

        public void NotifyRoomEnter(Room room, Player character, string direction)
        {
            foreach (var player in room.Players)
            {
                if (character.Name != player.Name)
                {
                    _writeToClient.WriteLine(MovementAdjective(character, true, Helpers.ReturnOpositeExitName(direction)), player.ConnectionId);
                }
            }


        }

        public string MovementAdjective(Player player, bool onEnter, string direction)
        {
            var showDirection = string.IsNullOrEmpty(direction) ? "" : $" {direction.ToLower()}";
            var enterMessage = player.EnterEmote;
            var leaveMessage = player.LeaveEmote;

            if (string.IsNullOrEmpty(enterMessage))
            {
                enterMessage = $"{player.Name} walks in from the";
            }
            if (string.IsNullOrEmpty(leaveMessage))
            {
                leaveMessage = $"{player.Name} walks";
            }
            var enter = onEnter ? enterMessage : leaveMessage;
            var isPlayer = player.ConnectionId != "mob";
            var moveType = $"<span class='{(isPlayer ? "player" : "mob")}'>{enter}{showDirection}.</span>";
            
            if (!string.IsNullOrEmpty(player.Mounted.Name))
            {
                enter = onEnter ? "enters from the" : "leaves";
                moveType = $"{player.Name} {enter}{showDirection} riding upon {player.Mounted.Name}.";
            }

            if (moveType.Contains("from the down."))
            {
                moveType = moveType.Replace("from the down", "from below");
                
            }
            
            if (moveType.Contains("from the up."))
            {
                moveType = moveType.Replace("from the up", "from above");
                
            }
            
            // A magic broom sweeping the floor [hovers in from the] [west]. 

            return moveType;
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


                if (!string.IsNullOrEmpty(mob.Events.Enter))
                {
                    try
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
                    catch (Exception)
                    {

                    }
                }

                if (mob.Agro && mob.Status != CharacterStatus.Status.Fighting && character.ConnectionId != "mob")
                {
                    _writeToClient.WriteLine($"{mob.Name} attacks you!", character.ConnectionId);
                    _mobScripts.AttackPlayer(room, character, mob);
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
                
                //player entered new area
                if(oldRoom.AreaId != room.AreaId)
                    _areaActions.AreaEntered(character, room);
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
                    mob.Status = CharacterStatus.Status.Standing;
                    _cache.RemoveCharFromCombat(mob.Id.ToString());
                }
            }

            var randomFleeMsg = new List<string>
            {
                $"{character.Name} turns and flees",
                $"{character.Name} screams and runs for their life",
                $"{character.Name} ducks and rolls before running away",
                $"{character.Name} retreats from combat."
            };

            var fleeString = randomFleeMsg[_dice.Roll(1, 0, randomFleeMsg.Count)];

            _writeToClient.WriteLine($"You flee {validExits[getExitIndex].Name}.",  character.ConnectionId);
            _writeToClient.WriteToOthersInRoom($"{fleeString}.", room, character);

            //this could be buggy
            Move(room, character, validExits[getExitIndex].Name, true);

        }

        public void Sit(Player player, Room room, string target)
        {
            if (!string.IsNullOrEmpty(player.Mounted.Name))
            {
                _writeToClient.WriteLine("<p>You can't do that while mounted.</p>", player.ConnectionId);
                return;
            }

            if (player.Status == CharacterStatus.Status.Sitting)
            {
                _writeToClient.WriteLine("<p>You are already sitting!</p>", player.ConnectionId);
                return;
            }

            if (target.Equals("sit", StringComparison.CurrentCultureIgnoreCase))
            {

                SetCharacterStatus(player, "is sitting here", CharacterStatus.Status.Sitting);
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

                SetCharacterStatus(player, $"is sitting down on {obj.Name.ToLower()}", CharacterStatus.Status.Sitting);
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
            if (!string.IsNullOrEmpty(player.Mounted.Name))
            {
                _writeToClient.WriteLine("<p>You can't do that while mounted.</p>", player.ConnectionId);
                return;
            }

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
            if (!string.IsNullOrEmpty(player.Mounted.Name))
            {
                _writeToClient.WriteLine("<p>You can't do that while mounted.</p>", player.ConnectionId);
                return;
            }

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
            if (!string.IsNullOrEmpty(player.Mounted.Name))
            {
                _writeToClient.WriteLine("<p>You can't do that while mounted.</p>", player.ConnectionId);
                return;
            }

            if (player.Status != CharacterStatus.Status.Sleeping)
            {
                _writeToClient.WriteLine("<p>You are already awake!</p>", player.ConnectionId);
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
            if (!string.IsNullOrEmpty(player.Mounted.Name))
            {
                _writeToClient.WriteLine("<p>You can't do that while mounted.</p>", player.ConnectionId);
                return;
            }

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


        public void ChangePlayerLocation(Player player, Room room)
        {
            player.RoomId = Helpers.ReturnRoomId(room);

            room.Players.Add(player);
        }

        public void RemovePlayerLocation(Player player, Room room)
        {

            room.Players.Remove(player);
        }



        public void Group(Player player, Room room, string target)
        {
            if ((string.IsNullOrEmpty(target) || target.Equals("group", StringComparison.CurrentCultureIgnoreCase)) && !player.grouped)
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

                Player foundLeader = null;

                if (player.grouped && player.Followers.Count > 0)
                {
                    foundLeader = player;
                }
                else
                {
                    foundLeader = _cache.GetPlayerCache()
                        .FirstOrDefault(x => x.Value.Name.StartsWith(player.Following, StringComparison.CurrentCultureIgnoreCase)).Value;
                }


                sb.Append($"<li>Lvl: {foundLeader.Level} {foundLeader.Name} (Leader)  <span class='group-hp' title='Hit points'>{foundLeader.Attributes.Attribute[EffectLocation.Hitpoints]}</span>/<span class='group-mana' title='Mana points'>{foundLeader.Attributes.Attribute[EffectLocation.Mana]}</span>/<span class='group-moves' title='Move points'>{foundLeader.Attributes.Attribute[EffectLocation.Moves]}</span></li>");

                foreach (var follower in foundLeader.Followers.Where(x => x.grouped))
                {
                    sb.Append($"<li>Lvl: {follower.Level} {follower.Name} <span class='group-hp' title='Hit points'>{follower.Attributes.Attribute[EffectLocation.Hitpoints]}</span>/<span class='group-mana' title='Mana points'>{follower.Attributes.Attribute[EffectLocation.Mana]}</span>/<span class='group-moves' title='Move points'>{follower.Attributes.Attribute[EffectLocation.Moves]}</span></li>");
                }

                sb.Append("</ul>");
                _writeToClient.WriteLine(sb.ToString(), player.ConnectionId);

                return;
            }

            var foundPlayer = player.Followers
            .FirstOrDefault(x => x.Name.StartsWith(target, StringComparison.CurrentCultureIgnoreCase));

            if (foundPlayer == null)
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
                if (pc.Id == player.Id || pc.Id == foundPlayer.Id)
                {
                    continue;
                }
                _writeToClient.WriteLine($"<p>{foundPlayer.Name} is now a member of {player.Name}'s group.</p>", pc.ConnectionId);

            }

        }

        public void Enter(Player player, Room room, string target)
        {
            if (string.IsNullOrEmpty(target))
            {
                _writeToClient.WriteLine("<p>You can't do that here.</p>", player.ConnectionId);
                return;
            }

            var nthItem = Helpers.findNth(target);
            var item = Helpers.findRoomObject(nthItem, room);

            if (item == null)
            {
                _writeToClient.WriteLine("<p>You don't see that here.</p>", player.ConnectionId);
                return;
            }

            if (item.ItemType != Item.Item.ItemTypes.Portal)
            {
                _writeToClient.WriteLine("<p>You can't enter that.</p>", player.ConnectionId);
                return;
            }

            foreach (var pc in room.Players)
            {
                if (player.Name == pc.Name)
                {
                    _writeToClient.WriteLine($"<p>You {item.Portal.EnterDescription}</p>", player.ConnectionId);
                    continue;
                }
                _writeToClient.WriteLine($"<p>{player.Name} {item.Portal.EnterDescription}</p>", pc.ConnectionId);
            }

            var newRoom = _cache.GetRoom(item.Portal.Destination);
            //Change player location
            ChangePlayerLocation(player, newRoom);
            RemovePlayerLocation(player, room);
            _roomActions.Look("", newRoom, player);

            foreach (var pc in newRoom.Players)
            {
                if (player.Name == pc.Name)
                {
                    continue;
                }
                _writeToClient.WriteLine($"<p>{player.Name} {item.Portal.EnterDescriptionRoom}</p>", pc.ConnectionId);
            }

            var rooms = _cache.GetMap($"{newRoom.AreaId}{newRoom.Coords.Z}");
            _updateUi.GetMap(player, rooms);

        }

        public void Follow(Player player, Room room, string target)
        {
            if (target.Equals("self", StringComparison.CurrentCultureIgnoreCase) || target.Equals(player.Name, StringComparison.CurrentCultureIgnoreCase))
            {

                var leader = _cache.GetPlayerCache()
              .FirstOrDefault(x => x.Value.Name.Equals(string.IsNullOrEmpty(player.Following) ? player.Name : player.Following, StringComparison.CurrentCultureIgnoreCase));

                _writeToClient.WriteLine($"<p>You stop following {leader.Value.Name}.</p>", player.ConnectionId);
                if (player.Name != leader.Value.Name)
                {
                    _writeToClient.WriteLine($"<p>{player.Name} stops following you.</p>", leader.Value.ConnectionId);
                }

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

            if (foundPlayer.Followers.Contains(player))
            {
                _writeToClient.WriteLine($"<p>You are already following {foundPlayer.Name}.</p>", player.ConnectionId);
                return;
            }
            
            if (foundPlayer.Following == player.Name)
            {
                _writeToClient.WriteLine($"<p>You can't follow someone following you. Lest you be running around in circles indefinitely.</p>", player.ConnectionId);
                return;
            }
            
            if (foundPlayer.Config.CanFollow == false)
            {
                _writeToClient.WriteLine($"<p>{foundPlayer.Name} doesn't want to be followed.</p>", player.ConnectionId);
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
            player.Pose = "";
        }

        public void UpdateLightCondition(Player player, Room room)
        {
            var lightSource = player.Equipped.Light;

            if (lightSource == null)
            {
                return;
            }

            lightSource.Condition -= _dice.Roll(1, 1, 5);

            if (lightSource.Condition <= 0)
            {
                lightSource.Condition = 0;

                _writeToClient.WriteLine($"<p>Your {lightSource.Name} flickers and fades out.</p>");
            }
            else
            {
                _writeToClient.WriteLine($"<p>Your {lightSource.Name} flickers.</p>");
            }
        }
    }
}
