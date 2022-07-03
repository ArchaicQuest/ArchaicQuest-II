using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Class;
using ArchaicQuestII.GameLogic.Character.Gain;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.World.Area;
using ArchaicQuestII.GameLogic.World.Room;
using Markdig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArchaicQuestII.GameLogic.Core
{
    public class Core : ICore
    {
        private readonly ICache _cache;
        private readonly IWriteToClient _writeToClient;
        private readonly IDataBase _db;
        private readonly IPlayerDataBase _pdb;
        private readonly IUpdateClientUI _clientUi;
        private readonly IDice _dice;
        private readonly IGain _gain;
        public Core(ICache cache, IWriteToClient writeToClient, IDataBase db, IPlayerDataBase pdb, IUpdateClientUI clientUi, IDice dice, IGain gain)
        {
            _cache = cache;
            _writeToClient = writeToClient;
            _db = db;
            _pdb = pdb;
            _clientUi = clientUi;
            _dice = dice;
            _gain = gain;
        }
        public void Who(Player player)
        {

            var sb = new StringBuilder();
            sb.Append("<ul>");
            foreach (var pc in _cache.GetPlayerCache())
            {
                sb.Append(
                    $"<li>[{pc.Value.Level} {pc.Value.Race} {pc.Value.ClassName}] ");
                sb.Append(
                    $"<span class='player'>{pc.Value.Name}, {pc.Value.Title}</span></li>");
            }

            sb.Append("</ul>");
            sb.Append($"<p>Players found: {_cache.GetPlayerCache().Count}</p>");

            _writeToClient.WriteLine(sb.ToString(), player.ConnectionId);


        }

        public void SetTitle(Player player, string title)
        {

            var titleText = title.Remove(0, 5);
            if (string.IsNullOrEmpty(titleText))
            {
                _writeToClient.WriteLine("Change your title to what?", player.ConnectionId);
                return;
            }

            player.Title = new string(titleText.Take(55).ToArray());
            _writeToClient.WriteLine($"Title changes to {player.Title}", player.ConnectionId);

        }


        /// <summary>
        ///  /teleport 1000 (area id,X,Y,Z) || /teleport Malleus
        /// </summary>
        /// <param name="player"></param>
        /// <param name="room"></param>
        /// <param name="location"></param>
        public void ImmTeleport(Player player, Room room, string location)
        {

            var roomId = 0000;
            bool locationIsRoomId = int.TryParse(location, out roomId);

            if (locationIsRoomId)
            {

                var newRoom = _cache.GetRoom(roomId.ToString());

                if (newRoom != null)
                {

                    room.Players.Remove(player);
                    player.RoomId = roomId.ToString();


                    foreach (var pc in room.Players)
                    {
                        if (pc.Id == player.Id)
                        {
                            continue;
                        }

                        _writeToClient.WriteLine($"There's a brilliant white flash and {player.Name} is gone.", pc.ConnectionId);
                    }

                    foreach (var pc in newRoom.Players)
                    {
                        if (pc.Id == player.Id)
                        {
                            continue;
                        }

                        _writeToClient.WriteLine($"There's a brilliant white flash and {player.Name} appears.", pc.ConnectionId);
                    }

                    newRoom.Players.Add(player);

                    _writeToClient.WriteLine($"You are now in {newRoom.Title}.", player.ConnectionId);


                    player.Buffer.Enqueue("look");

                    var rooms = _cache.GetMap($"{newRoom.AreaId}{newRoom.Coords.Z}");
                    _clientUi.GetMap(player, rooms);


                }
                else
                {
                    _writeToClient.WriteLine($"that room does not exist", player.ConnectionId);
                }
            }
            else
            {
                Player foundPlayer = null;
                foreach (var checkRoom in _cache.GetAllRooms())
                {
                    if (foundPlayer != null)
                    {
                        break;
                    }
                    foreach (var checkRoomPlayer in checkRoom.Players)
                    {
                        if (foundPlayer != null)
                        {
                            break;
                        }
                        if (checkRoomPlayer.Name.StartsWith(location, StringComparison.CurrentCultureIgnoreCase))
                        {
                            foundPlayer = checkRoomPlayer;
                        }
                    }
                }

                if (foundPlayer == null)
                {
                    _writeToClient.WriteLine("They're not here.", player.ConnectionId);
                    return;
                }
                room.Players.Remove(player);
                player.RoomId = foundPlayer.RoomId;
                var newRoom = _cache.GetRoom(foundPlayer.RoomId);

                foreach (var pc in newRoom.Players)
                {
                    if (pc.Id == player.Id)
                    {
                        continue;
                    }

                    _writeToClient.WriteLine($"There's a brilliant white flash and {player.Name} appears.", pc.ConnectionId);
                }

                newRoom.Players.Add(player);

                _writeToClient.WriteLine($"You are now in {newRoom.Title}.", player.ConnectionId);

                player.Buffer.Enqueue("look");

                var rooms = _cache.GetMap($"{newRoom.AreaId}{newRoom.Coords.Z}");
                _clientUi.GetMap(player, rooms);

            }

        }

        public void Emote(Player player, Room room, string emote)
        {
            var emoteText = emote.Remove(0, 5);
            var emoteMessage = $"{player.Name} {emoteText}";

            foreach (var players in room.Players)
            {
                _writeToClient.WriteLine(emoteMessage, players.ConnectionId);
            }
        }

        public void Pmote(Player player, Room room, string emote)
        {
            var emoteText = emote.Remove(0, 5);
            var emoteMessage = emoteText;

            foreach (var players in room.Players)
            {
                emoteMessage = emoteMessage.Replace(players.Name, "you", StringComparison.CurrentCultureIgnoreCase);

                _writeToClient.WriteLine(player.Name + " " + emoteMessage, players.ConnectionId);
            }
        }

        public void Pose(Player player, string pose)
        {
            var poseText = pose.Remove(0, 4);
            if (string.IsNullOrEmpty(poseText))
            {
                player.Pose = poseText;
            }


            player.Pose = $", {poseText}";
            _writeToClient.WriteLine("Pose set.", player.ConnectionId);
        }


        public void CheckPose(Player player)
        {

            var poseText = string.Empty;

            if (string.IsNullOrEmpty(player.LongName))
            {
                poseText = $"{ player.Name}";
            }
            else
            {
                poseText = $"{ player.Name} {player.LongName}";
            }

            if (!string.IsNullOrEmpty(player.Mounted.Name))
            {
                poseText += $", is riding {player.Mounted.Name}";
            }
            else if (string.IsNullOrEmpty(player.LongName))
            {
                poseText += " is here";

            }

            poseText += player.Pose;

            _writeToClient.WriteLine(poseText, player.ConnectionId);
        }

        public void Scan(Player player, Room room, string direction)
        {

            if (!string.IsNullOrEmpty(direction) && direction != "scan")
            {
                ScanDirection(player, room, direction);
                return;
            }

            var sb = new StringBuilder();

            sb.Append($"<span>Right here:</span>");

            foreach (var obj in room.Mobs)
            {

                sb.Append($"<p class='mob'>{obj.Name} is right here.</p>");

            }

            foreach (var obj in room.Players)
            {

                sb.Append($"<p class='player'>{obj.Name} is right here.</p>");

            }

            if (!room.Mobs.Any() && !room.Players.Any())
            {

                sb.Append($"<p>There is nobody here.</p>");

            }

            foreach (var exit in Helpers.GetListOfExits(room.Exits))
            {
                var getRoomCoords = Helpers.IsExit(exit, room);

                var getRoomObj = _cache.GetRoom($"{getRoomCoords.AreaId}{getRoomCoords.Coords.X}{getRoomCoords.Coords.Y}{getRoomCoords.Coords.Z}");

                sb.Append($"<span>{exit}:</span>");

                foreach (var obj in getRoomObj.Mobs)
                {
                    if (exit.Equals("down", StringComparison.CurrentCultureIgnoreCase))
                    {
                        sb.Append($"<p class='mob'>{obj.Name} is below you.</p>");
                    }
                    else if (exit.Equals("up", StringComparison.CurrentCultureIgnoreCase))
                    {
                        sb.Append($"<p class='mob'>{obj.Name} is above you.</p>");
                    }
                    else
                    {
                        sb.Append($"<p class='mob'>{obj.Name} is to the {exit}.</p>");
                    }
                }

                foreach (var obj in getRoomObj.Players)
                {
                    if (exit.Equals("down", StringComparison.CurrentCultureIgnoreCase))
                    {
                        sb.Append($"<p class='player'>{obj.Name} is below you.</p>");
                    }
                    else if (exit.Equals("up", StringComparison.CurrentCultureIgnoreCase))
                    {
                        sb.Append($"<p class='player'>{obj.Name} is above you.</p>");
                    }
                    else
                    {
                        sb.Append($"<p class='player'>{obj.Name} is to the {exit}.</p>");
                    }
                }

                if (!getRoomObj.Mobs.Any() && !getRoomObj.Players.Any())
                {

                    sb.Append($"<p>There is nobody there.</p>");

                }

            }
            _writeToClient.WriteLine(sb.ToString(), player.ConnectionId);
        }

        public void ScanDirection(Player player, Room room, string direction)
        {
            var directions = new List<string>()
           {
               "North",
               "East",
               "South",
               "West",
               "North West",
               "North East",
               "South East",
               "South West",
               "Up",
               "Down"
           };

            var getDirection = directions.FirstOrDefault(x => x.StartsWith(direction, StringComparison.CurrentCultureIgnoreCase));

            if (getDirection == null)
            {
                _writeToClient.WriteLine("You can't look in that direction.", player.ConnectionId);
            }

            var getRoomCoords = Helpers.IsExit(getDirection, room);

            var getRoomObj = _cache.GetRoom($"{getRoomCoords.AreaId}{getRoomCoords.Coords.X}{getRoomCoords.Coords.Y}{getRoomCoords.Coords.Z}");
            var sb = new StringBuilder();

            sb.Append($"<span>You peer intently {getDirection}</span>");

            foreach (var obj in getRoomObj.Mobs)
            {
                if (getDirection.Equals("down", StringComparison.CurrentCultureIgnoreCase))
                {
                    sb.Append($"<p class='mob'>{obj.Name} is below you.</p>");
                }
                else if (getDirection.Equals("up", StringComparison.CurrentCultureIgnoreCase))
                {
                    sb.Append($"<p class='mob'>{obj.Name} is above you.</p>");
                }
                else
                {
                    sb.Append($"<p class='mob'>{obj.Name} is to the {getDirection}.</p>");
                }
            }

            foreach (var obj in getRoomObj.Players)
            {
                if (getDirection.Equals("down", StringComparison.CurrentCultureIgnoreCase))
                {
                    sb.Append($"<p class='player'>{obj.Name} is below you.</p>");
                }
                else if (getDirection.Equals("up", StringComparison.CurrentCultureIgnoreCase))
                {
                    sb.Append($"<p class='player'>{obj.Name} is above you.</p>");
                }
                else
                {
                    sb.Append($"<p class='player'>{obj.Name} is to the {getDirection}.</p>");
                }
            }

            if (!getRoomObj.Mobs.Any() && !getRoomObj.Players.Any())
            {

                sb.Append($"<p>There is nobody there.</p>");

            }

            _writeToClient.WriteLine(sb.ToString(), player.ConnectionId);
        }


        public void Save(Player player)
        {

            _pdb.Save(player, PlayerDataBase.Collections.Players);
            _writeToClient.WriteLine("Character saved.", player.ConnectionId);
        }

        public void Quit(Player player, Room room)
        {

            player.Buffer = new Queue<string>();
            var lastLoginTime = player.LastLoginTime;
            var playTime = DateTime.Now.Subtract(lastLoginTime).TotalMinutes;
            player.PlayTime += (int)DateTime.Now.Subtract(lastLoginTime).TotalMinutes;

            var account = _pdb.GetById<Account.Account>(player.AccountId, PlayerDataBase.Collections.Account);
            account.Stats.TotalPlayTime += playTime;

            _pdb.Save(account, PlayerDataBase.Collections.Account);

            Save(player);

            foreach (var pc in room.Players)
            {
                if (player.Name.Equals(player.Name))
                {
                    _writeToClient.WriteLine("You wave goodbye and vanish.", pc.ConnectionId);
                    continue;
                }
                _writeToClient.WriteLine($"{player.Name} waves goodbye and vanishes.", pc.ConnectionId);
            }

            room.Players.Remove(player);
            _writeToClient.WriteLine($"We await your return {player.Name}. If you enjoyed your time here, help spread the word by tweeting, writing a blog posts or posting reviews online.", player.ConnectionId);
            Helpers.PostToDiscord($"{player.Name} quit after playing for {Math.Floor(DateTime.Now.Subtract(player.LastLoginTime).TotalMinutes)} minutes.", "event", _cache.GetConfig());
            _cache.RemovePlayer(player.ConnectionId);


        }

        public void Where(Player player, Room room)
        {
            var area = _cache.GetAllRoomsInArea(room.AreaId);
            var areaName = _db.GetCollection<Area>(DataBase.Collections.Area).FindById(room.AreaId);

            var sb = new StringBuilder();

            sb.Append($"<p>{areaName.Title}</p><p>Players near you:</p>");
            sb.Append("<ul>");
            foreach (var rm in area)
            {
                foreach (var pc in rm.Players)
                {
                    sb.Append(
                        $"<li>{pc.Name} - {rm.Title}");
                }

            }

            sb.Append("</ul>");

            _writeToClient.WriteLine(sb.ToString(), player.ConnectionId);
        }

        public void QuestLog(Player player)
        {
            var sb = new StringBuilder();

            foreach (var q in player.QuestLog)
            {
                sb.Append($"<div class='quest-block'><h3>{q.Title}</h3><p>{q.Area}</p><p>Kill:</p><ol>");

                foreach (var mob in q.MobsToKill)
                {
                    sb.Append($"<li>{mob.Name} {mob.Current}/{mob.Count}</li>");
                }

                sb.Append($"</ol><p>{q.Description}</p><p>Reward:</p><ul><li>{q.ExpGain} Experience points</li><li>{q.GoldGain} Gold</li>");

                foreach (var i in q.ItemGain)
                {
                    sb.Append($"<li>{i.Name}</li>");
                }

                sb.Append("</ul></div>");
            }

            _writeToClient.WriteLine(sb.ToString(), player.ConnectionId);

        }

        public void Recall(Player player, Room room)
        {
            if ((player.Status & CharacterStatus.Status.Sleeping) != 0)
            {
                _writeToClient.WriteLine("In your dreams, or what?", player.ConnectionId);
                return;
            }
            if ((player.Status & CharacterStatus.Status.Sitting) != 0)
            {
                _writeToClient.WriteLine("Better stand up first.", player.ConnectionId);
                return;
            }
            if ((player.Status & CharacterStatus.Status.Resting) != 0)
            {
                _writeToClient.WriteLine("Nah... You feel too relaxed...", player.ConnectionId);
                return;
            }

            foreach (var pc in room.Players)
            {
                if (pc.Id == player.Id)
                {
                    _writeToClient.WriteLine("You glow a bright light before vanishing.", pc.ConnectionId);
                    continue;
                }
                _writeToClient.WriteLine($"{player.Name} glows a bright light before vanishing.", pc.ConnectionId);
            }

            room.Players.Remove(player);

            var recallRoom = _cache.GetRoom(player.RecallId);

            if (!recallRoom.Players.Any(a => a.Id == player.Id))
            {
                recallRoom.Players.Add(player);
                player.RoomId = player.RecallId;
            }

            player.Buffer.Clear();

            foreach (var pc in recallRoom.Players)
            {
                if (pc.Id == player.Id)
                {
                    continue;
                }
                _writeToClient.WriteLine($"{player.Name} suddenly appears in a flash of bright light.", pc.ConnectionId);
            }

            player.Buffer.Enqueue("l");

            player.Attributes.Attribute[EffectLocation.Moves] = player.Attributes.Attribute[EffectLocation.Moves] / 2;

            if (player.Attributes.Attribute[EffectLocation.Moves] < 1)
            {
                player.Attributes.Attribute[EffectLocation.Moves] = 0;
            }

            _clientUi.UpdateScore(player);
            _clientUi.UpdateMoves(player);
            _clientUi.GetMap(player, _cache.GetMap($"{recallRoom.AreaId}{recallRoom.Coords.Z}"));

        }

        public void Train(Player player, Room room, string stat)
        {
            if ((player.Status & CharacterStatus.Status.Sleeping) != 0)
            {
                _writeToClient.WriteLine("In your dreams, or what?", player.ConnectionId);
                return;
            }

            if (room.Mobs.Find(x => x.Trainer) == null)
            {
                _writeToClient.WriteLine("You can't do that here.", player.ConnectionId);
                return;
            }

            if (player.Trains <= 0)
            {
                _writeToClient.WriteLine("You have no training sessions left.", player.ConnectionId);
                return;
            }

            if (string.IsNullOrEmpty(stat) || stat == "train")
            {

                _writeToClient.WriteLine(
                    ($"<p>You have {player.Trains} training session{(player.Trains > 1 ? "s" : "")} remaining.<br />You can train: str dex con int wis cha hp mana move.</p>"
                    ), player.ConnectionId);
            }
            else
            {
                var statName = GetStatName(stat);
                if (string.IsNullOrEmpty(statName.Item1))
                {
                    _writeToClient.WriteLine(
                        ($"<p>{stat} not found. Please choose from the following. <br /> You can train: str dex con int wis cha hp mana move.</p>"
                        ), player.ConnectionId);
                    return;
                }

                player.Trains -= 1;
                if (player.Trains < 0)
                {
                    player.Trains = 0;
                }

                if (statName.Item1 == "hit points" || statName.Item1 == "moves" || statName.Item1 == "mana")
                {
                    var hitDie = _cache.GetClass(player.ClassName);
                    var roll = _dice.Roll(1, hitDie.HitDice.DiceMinSize, hitDie.HitDice.DiceMaxSize);

                    player.MaxAttributes.Attribute[statName.Item2] += roll;
                    player.Attributes.Attribute[statName.Item2] += roll;

                    _writeToClient.WriteLine(
                        ($"<p class='gain'>Your {statName.Item1} increases by {roll}.</p>"
                        ), player.ConnectionId);

                    _clientUi.UpdateHP(player);
                    _clientUi.UpdateMana(player);
                    _clientUi.UpdateMoves(player);
                }
                else
                {
                    player.MaxAttributes.Attribute[statName.Item2] += 1;
                    player.Attributes.Attribute[statName.Item2] += 1;

                    _writeToClient.WriteLine(
                        ($"<p class='gain'>Your {statName.Item1} increases by 1.</p>"
                        ), player.ConnectionId);
                }




                _clientUi.UpdateScore(player);


            }
        }

        public string UpdateAffect(Player player, Item.Item item, Affect affect)
        {
            string modBenefits = String.Empty;

            if (item.Modifier.Strength != 0)
            {

                affect.Duration = 5;
                player.Attributes.Attribute[EffectLocation.Strength] += item.Modifier.Strength;

                affect.Modifier.Strength = item.Modifier.Strength;
                modBenefits = $"modifies STR by {item.Modifier.Strength} for { affect.Duration} minutes<br />";

            }

            if (item.Modifier.Dexterity != 0)
            {


                affect.Duration = 5;
                player.Attributes.Attribute[EffectLocation.Dexterity] += item.Modifier.Dexterity;

                affect.Modifier.Dexterity = item.Modifier.Dexterity;
                modBenefits = $"modifies DEX by {item.Modifier.Dexterity} for { affect.Duration} minutes<br />";

            }

            if (item.Modifier.Constitution != 0)
            {


                affect.Duration = 5;
                player.Attributes.Attribute[EffectLocation.Constitution] += item.Modifier.Constitution;

                affect.Modifier.Constitution = item.Modifier.Constitution;
                modBenefits = $"modifies CON by {item.Modifier.Constitution} for { affect.Duration} minutes<br />";

            }

            if (item.Modifier.Intelligence != 0)
            {

                affect.Duration = 5;
                player.Attributes.Attribute[EffectLocation.Intelligence] += item.Modifier.Intelligence;
                affect.Modifier.Intelligence = item.Modifier.Intelligence;
                modBenefits = $"modifies INT by {item.Modifier.Intelligence} for { affect.Duration} minutes<br />";

            }

            if (item.Modifier.Wisdom != 0)
            {

                affect.Duration = 5;
                player.Attributes.Attribute[EffectLocation.Wisdom] += item.Modifier.Wisdom;

                affect.Modifier.Wisdom = item.Modifier.Wisdom;
                modBenefits = $"modifies WIS by {item.Modifier.Wisdom} for { affect.Duration} minutes<br />";
            }

            if (item.Modifier.Charisma != 0)
            {


                affect.Duration = 5;
                player.Attributes.Attribute[EffectLocation.Charisma] += item.Modifier.Charisma;

                affect.Modifier.Charisma = item.Modifier.Charisma;
                modBenefits = $"modifies CHA by {item.Modifier.Charisma} for { affect.Duration} minutes<br />";

            }

            if (item.Modifier.HP != 0)
            {

                player.Attributes.Attribute[EffectLocation.Hitpoints] += item.Modifier.HP;

                if (player.Attributes.Attribute[EffectLocation.Hitpoints] >
                    player.MaxAttributes.Attribute[EffectLocation.Hitpoints])
                {
                    player.Attributes.Attribute[EffectLocation.Hitpoints] =
                        player.MaxAttributes.Attribute[EffectLocation.Hitpoints];
                }

            }

            if (item.Modifier.Mana != 0)
            {

                player.Attributes.Attribute[EffectLocation.Mana] += item.Modifier.Mana;

                if (player.Attributes.Attribute[EffectLocation.Mana] >
                    player.MaxAttributes.Attribute[EffectLocation.Mana])
                {
                    player.Attributes.Attribute[EffectLocation.Mana] =
                        player.MaxAttributes.Attribute[EffectLocation.Mana];
                }

            }

            if (item.Modifier.Moves != 0)
            {

                player.Attributes.Attribute[EffectLocation.Moves] += item.Modifier.Moves;

                if (player.Attributes.Attribute[EffectLocation.Moves] >
                    player.MaxAttributes.Attribute[EffectLocation.Moves])
                {
                    player.Attributes.Attribute[EffectLocation.Moves] =
                        player.MaxAttributes.Attribute[EffectLocation.Moves];
                }

            }

            if (item.Modifier.HitRoll != 0)
            {


                affect.Duration = 5;
                player.Attributes.Attribute[EffectLocation.HitRoll] += item.Modifier.HitRoll;
                affect.Modifier.HitRoll = item.Modifier.HitRoll;

                modBenefits = $"modifies Hit Roll by {item.Modifier.HitRoll} for { affect.Duration} minutes<br />";
            }

            if (item.Modifier.DamRoll != 0)
            {


                affect.Duration = 5;
                player.Attributes.Attribute[EffectLocation.DamageRoll] += item.Modifier.DamRoll;

                affect.Modifier.DamRoll = item.Modifier.DamRoll;
                modBenefits = $"modifies Dam Roll by {item.Modifier.DamRoll} for { affect.Duration} minutes<br />";

            }

            // saves / saving spell

            return modBenefits;
        }

        public void Eat(Player player, Room room, string obj)
        {
            var findNth = Helpers.findNth(obj);
            var food = Helpers.findObjectInInventory(findNth, player);

            if (food == null)
            {
                _writeToClient.WriteLine("You have no food of that name", player.ConnectionId);
                return;
            }

            if (player.Hunger >= 4)
            {
                _writeToClient.WriteLine("You are too full to eat more.", player.ConnectionId);
                return;
            }

            player.Hunger++;

            player.Inventory.Remove(food);

            _writeToClient.WriteLine($"You eat {food.Name.ToLower()}.", player.ConnectionId);


            foreach (var pc in room.Players)
            {
                if (pc.Name == player.Name)
                {
                    continue;
                }

                _writeToClient.WriteLine($"{player.Name} eats {food.Name.ToLower()}.", pc.ConnectionId);

            }

            var benefits = new StringBuilder().Append("<table>");
            var modBenefits = "";
            var hasEffect = player.Affects.Custom.FirstOrDefault(x => x.Name.Equals(food.Name));
            var newEffect = new Affect()
            {
                Modifier = new Modifier()
            };



            if (hasEffect == null)
            {

                modBenefits = UpdateAffect(player, food, newEffect);

                benefits.Append(
                    $"<tr><td>Food:</td><td>{food.Name}<br />{modBenefits}</td></td>");
                benefits.Append("</table>");

                newEffect.Name = food.Name;
                newEffect.Benefits = benefits.ToString();

                player.Affects.Custom.Add(newEffect);
            }
            else
            {


                modBenefits = UpdateAffect(player, food, hasEffect);


                benefits.Append(
                    $"<tr><td>Food:</td><td>{food.Name}<br />{modBenefits}</td></td>");
                benefits.Append("</table>");


                hasEffect.Benefits = benefits.ToString();
            }


            if (player.Hunger >= 4)
            {
                _writeToClient.WriteLine("You are no longer hungry.", player.ConnectionId);

            }

            _clientUi.UpdateAffects(player);
            _clientUi.UpdateScore(player);
            _clientUi.UpdateMoves(player);
            _clientUi.UpdateHP(player);
            _clientUi.UpdateMana(player);
            _clientUi.UpdateInventory(player);


        }

        public void Drink(Player player, Room room, string obj)
        {
            var findNth = Helpers.findNth(obj);
            var drink = Helpers.findObjectInInventory(findNth, player) ??
                        Helpers.findRoomObject(findNth, room);

            if (drink == null)
            {
                _writeToClient.WriteLine("You can't find that.", player.ConnectionId);
                return;
            }

            if (drink.ItemType != Item.Item.ItemTypes.Drink)
            {
                _writeToClient.WriteLine($"You can't drink from {drink.Name.ToLower()}.", player.ConnectionId);
                return;
            }

            _writeToClient.WriteLine($"You drink from {drink.Name.ToLower()}.", player.ConnectionId);

            foreach (var pc in room.Players)
            {
                if (pc.Name == player.Name)
                {
                    continue;
                }
                _writeToClient.WriteLine($"{player.Name} drink from {drink.Name.ToLower()}.", player.ConnectionId);

            }

        }

        /// <summary>
        /// for testing
        /// </summary>
        /// <param name="player"></param>
        public void TrainSkill(Player player)
        {
            foreach (var skill in player.Skills)
            {
                skill.Proficiency = 85;
            }
        }


        /// <summary>
        /// for testing
        /// </summary>
        /// <param name="player"></param>
        public void RestorePlayer(Player player)
        {
            player.Attributes.Attribute[EffectLocation.Hitpoints] = player.MaxAttributes.Attribute[EffectLocation.Hitpoints];
            player.Attributes.Attribute[EffectLocation.Mana] = player.MaxAttributes.Attribute[EffectLocation.Mana];
            player.Attributes.Attribute[EffectLocation.Moves] = player.MaxAttributes.Attribute[EffectLocation.Moves];
            _clientUi.UpdateHP(player);
            _clientUi.UpdateMoves(player);
            _clientUi.UpdateMana(player);

            _writeToClient.WriteLine("You are restored.");
        }

        public void Dismount(Player player, Room room)
        {
            if (string.IsNullOrEmpty(player.Mounted.Name))
            {
                _writeToClient.WriteLine("You are not using a mount");
                return;
            }

            var getMount = player.Pets.FirstOrDefault(x => x.Name.Equals(player.Mounted.Name));

            if (getMount != null)
            {
                player.Pets.Remove(getMount);
                getMount.Mounted.MountedBy = String.Empty;
                player.Mounted.Name = string.Empty;

                _writeToClient.WriteLine($"You dismount {getMount.Name}.", player.ConnectionId);

                foreach (var pc in room.Players)
                {
                    if (pc.Id == player.Id)
                    {
                        continue;
                    }

                    _writeToClient.WriteLine($"{player.Name} dismounts {getMount.Name}.", pc.ConnectionId);
                }
            }

        }


        public Tuple<string, EffectLocation> GetStatName(string name)
        {
            return name switch
            {
                "str" => new Tuple<string, EffectLocation>("strength", EffectLocation.Strength),
                "dex" => new Tuple<string, EffectLocation>("dexterity", EffectLocation.Dexterity),
                "con" => new Tuple<string, EffectLocation>("constitution", EffectLocation.Constitution),
                "int" => new Tuple<string, EffectLocation>("intelligence", EffectLocation.Intelligence),
                "wis" => new Tuple<string, EffectLocation>("wisdom", EffectLocation.Wisdom),
                "cha" => new Tuple<string, EffectLocation>("charisma", EffectLocation.Charisma),
                "hp" => new Tuple<string, EffectLocation>("hit points", EffectLocation.Hitpoints),
                "move" => new Tuple<string, EffectLocation>("moves", EffectLocation.Moves),
                "mana" => new Tuple<string, EffectLocation>("mana", EffectLocation.Mana),
                _ => new Tuple<string, EffectLocation>("", EffectLocation.None)
            };
        }

        /// <summary>
        /// is basic skill successful?
        /// </summary>
        /// <param name="skill"></param>
        /// <returns></returns>
        public bool SkillCheckSuccesful(SkillList skill)
        {
            var proficiency = skill.Proficiency;
            var success = _dice.Roll(1, 1, 100);

            if (success == 1 || success == 101)
            {
                return false;
            }

            return proficiency >= success;
        }

        public void GainSkillProficiency(SkillList foundSkill, Player player)
        {

            var getSkill = _cache.GetSkill(foundSkill.SkillId);

            if (getSkill == null)
            {
                var skill = _cache.GetAllSkills().FirstOrDefault(x => x.Name.Equals(foundSkill.SkillName, StringComparison.CurrentCultureIgnoreCase));
                foundSkill.SkillId = skill.Id;
            }


            if (foundSkill.Proficiency == 100)
            {
                return;
            }

            var increase = _dice.Roll(1, 1, 5);

            foundSkill.Proficiency += increase;

            _gain.GainExperiencePoints(player, 100 * foundSkill.Level / 4, false);

            _clientUi.UpdateExp(player);

            _writeToClient.WriteLine(
                $"<p class='improve'>You learn from your mistakes and gain {100 * foundSkill.Level / 4} experience points.</p>" +
                $"<p class='improve'>Your knowledge of {foundSkill.SkillName} increases by {increase}%.</p>",
                player.ConnectionId, 0);
        }

        public void Affects(Player player)
        {
            var sb = new StringBuilder();

            sb.Append("<p>You are affected by the following effects:</p><table class='simple'><thead><tr><td>Skill</td><td>Affect</td></tr></thead>");

            foreach (var affect in player.Affects.Custom)
            {
                sb.Append($"<tr> <td> {affect.Name}<div>{affect.Benefits}</div </td>");
                sb.Append("<td>");

                if (affect.Modifier.Armour != 0)
                {
                    sb.Append($"<p>modifies armour by {affect.Modifier.Armour}<br />{affect.Duration}cycles, ({affect.Duration / 2} hours)</p> ");
                }

                if (affect.Modifier.DamRoll != 0)
                {
                    sb.Append($"<p>modifies damage roll by {affect.Modifier.DamRoll}<br />{affect.Duration}cycles, ({affect.Duration / 2} hours)</p> ");
                }

                if (affect.Modifier.HitRoll != 0)
                {
                    sb.Append($"<p>modifies hit roll by {affect.Modifier.HitRoll}<br />{affect.Duration}cycles, ({affect.Duration / 2} hours)</p> ");
                }

                if (affect.Modifier.Saves != 0)
                {
                    sb.Append($"<p>modifies saves by {affect.Modifier.Saves}<br />{affect.Duration}cycles, ({affect.Duration / 2} hours)</p> ");
                }

                if (affect.Modifier.HP != 0)
                {
                    sb.Append($"<p>modifies hit points by {affect.Modifier.HP}<br />{affect.Duration}cycles, ({affect.Duration / 2} hours)</p> ");
                }

                if (affect.Modifier.Mana != 0)
                {
                    sb.Append($"<p>modifies mana by {affect.Modifier.Mana}<br />{affect.Duration}cycles, ({affect.Duration / 2} hours)</p> ");
                }

                if (affect.Modifier.Moves != 0)
                {
                    sb.Append($"<p>modifies moves by {affect.Modifier.Moves}<br />{affect.Duration}cycles, ({affect.Duration / 2} hours)</p> ");
                }

                if (affect.Modifier.SpellDam != 0)
                {
                    sb.Append($"<p>modifies spell damage by {affect.Modifier.SpellDam}<br />{affect.Duration}cycles, ({affect.Duration / 2} hours)</p> ");
                }

                if (affect.Modifier.Strength != 0)
                {
                    sb.Append($"<p>modifies strength by {affect.Modifier.Strength}<br />{affect.Duration}cycles, ({affect.Duration / 2} hours)</p> ");
                }

                if (affect.Modifier.Dexterity != 0)
                {
                    sb.Append($"<p>modifies dexterity by {affect.Modifier.Dexterity}<br />{affect.Duration}cycles, ({affect.Duration / 2} hours)</p> ");
                }

                if (affect.Modifier.Constitution != 0)
                {
                    sb.Append($"<p>modifies constitution by {affect.Modifier.Constitution}<br />{affect.Duration}cycles, ({affect.Duration / 2} hours)</p> ");
                }

                if (affect.Modifier.Intelligence != 0)
                {
                    sb.Append($"<p>modifies intelligence by {affect.Modifier.Intelligence}<br />{affect.Duration}cycles, ({affect.Duration / 2} hours)</p> ");
                }

                if (affect.Modifier.Wisdom != 0)
                {
                    sb.Append($"<p>modifies wisdom by {affect.Modifier.Wisdom}<br />{affect.Duration}cycles, ({affect.Duration / 2} hours)</p> ");
                }

                if (affect.Modifier.Charisma != 0)
                {
                    sb.Append($"<p>modifies charisma by {affect.Modifier.Charisma}<br />{affect.Duration}cycles, ({affect.Duration / 2} hours)</p> ");
                }
                sb.Append("</td></tr>");
            }

            sb.Append("</tr></table>");

            _writeToClient.WriteLine(sb.ToString(), player.ConnectionId);


        }

        public void Practice(Player player, Room room, string skillname)
        {
            if ((player.Status & CharacterStatus.Status.Sleeping) != 0)
            {
                _writeToClient.WriteLine("In your dreams, or what?", player.ConnectionId);
                return;
            }

            if (room.Mobs.Find(x => x.Trainer) == null)
            {
                _writeToClient.WriteLine("You can't do that here.", player.ConnectionId);
                return;
            }

            var trainerName = room.Mobs.Find(x => x.Trainer).Name;

            var skillName = skillname == "prac" || skillname == "practice" ? "" : skillname;

            if (string.IsNullOrEmpty(skillName))
            {

                _writeToClient.WriteLine($"You have {player.Practices} practice{(player.Practices <= 1 ? "" : "s")} left.", player.ConnectionId);

                var sb = new StringBuilder();

                sb.Append("<table class='simple'><thead><tr><th></th><th></th><th colspan='2'>Skills</th><th></th><th></th></tr></thead><tbody>");

                var i = 0;
                foreach (var skill in player.Skills.OrderBy(x => x.SkillName))
                {
                    if (i == 0)
                    {
                        sb.Append("<tr>");
                    }

                    if (i <= 2)
                    {
                        sb.Append($"<td>{skill.SkillName}</td><td>{skill.Proficiency}%</td>");
                    }

                    if (i == 2)
                    {
                        sb.Append($"</tr>");
                        i = 0;
                        continue;
                    }
                    i++;

                };

                sb.Append("</tbody></table>");

                //if (player.Skills.Where(x => x.IsSpell == true).Any())
                //{

                //    sb.Append("<table class='simple'><thead><tr><th></th><th></th><th colspan='2'>Spells</th><th></th><th></th></tr></thead><tbody>");

                //    var j = 0;
                //    foreach (var skill in player.Skills.Where(x => x.IsSpell == true).OrderBy(x => x.SkillName))
                //    {
                //        if (j == 0)
                //        {
                //            sb.Append("<tr>");
                //        }

                //        if (j <= 2)
                //        {
                //            sb.Append($"<td>{skill.SkillName}</td><td>{skill.Proficiency}%</td>");
                //        }

                //        if (j == 2)
                //        {
                //            sb.Append($"</tr>");
                //            i = 0;
                //            continue;
                //        }


                //        j++;

                //        if (player.Skills.Where(x => x.IsSpell == true).Count() == 2 && j == player.Skills.Where(x => x.IsSpell == true).Count())
                //        {
                //            if (j == 2)
                //            {
                //                sb.Append($"<td>&nbsp;</td>&nbsp;<td></td>");
                //                sb.Append($"<td>&nbsp;</td><td>&nbsp;</td>");
                //            }
                //        }
                //    };

                //    sb.Append("</tbody></table>");

                //}
                _writeToClient.WriteLine(sb.ToString(), player.ConnectionId);
                return;
            }

            var foundSkill = player.Skills.Find(x => x.SkillName.StartsWith(skillName, StringComparison.OrdinalIgnoreCase));

            if (foundSkill == null)
            {
                _writeToClient.WriteLineMobSay(trainerName, $"You don't have that skill to practice.", player.ConnectionId);
                return;
            }

            if (player.Practices == 0)
            {
                _writeToClient.WriteLineMobSay(trainerName, $"You have no practices left.", player.ConnectionId);
                return;
            }

            if (foundSkill.Proficiency == 100)
            {
                _writeToClient.WriteLineMobSay(trainerName, $"You have already mastered {foundSkill.SkillName}.", player.ConnectionId);
                return;
            }

            if (foundSkill.Proficiency >= 75)
            {
                _writeToClient.WriteLineMobSay(trainerName, $"I've taught you everything I can about {foundSkill.SkillName}.", player.ConnectionId);
                return;
            }

            var maxGain = player.Attributes.Attribute[EffectLocation.Intelligence];
            var minGain = player.Attributes.Attribute[EffectLocation.Intelligence] / 2;
            var gain = _dice.Roll(1, minGain, maxGain);

            foundSkill.Proficiency += gain;
            player.Practices -= 1;

            if (foundSkill.Proficiency >= 75)
            {
                foundSkill.Proficiency = 75;
                _writeToClient.WriteLine($"You practice for some time. Your proficiency with {foundSkill.SkillName} is now {foundSkill.Proficiency}%", player.ConnectionId);
                _writeToClient.WriteLineMobSay(trainerName, $"You'll have to practice it on your own now...", player.ConnectionId);
                return;
            }

            _writeToClient.WriteLine($"You practice for some time. Your proficiency with {foundSkill.SkillName} is now {foundSkill.Proficiency}%", player.ConnectionId);


        }

        public void SetEvent(Player player, string eventName, string value)
        {
            if (eventName.Equals("/setevent", StringComparison.CurrentCultureIgnoreCase) || string.IsNullOrEmpty(eventName))
            {
                foreach (var ev in player.EventState)
                {
                    _writeToClient.WriteLine($"{ev.Key} - {ev.Value}", player.ConnectionId);
                }

                return;
            }

            if (player.EventState.ContainsKey(eventName))
            {
                player.EventState[eventName] = Int32.Parse(value);
                _writeToClient.WriteLine($"{eventName} state changed to {player.EventState[eventName]}", player.ConnectionId);
                return;
            }

            _writeToClient.WriteLine($"Invalid Event state", player.ConnectionId);
        }

        public void Read(Player player, string book, string pageNum, string fullCommand)
        {

            var splitCommand = fullCommand.Split(" ");
            pageNum = splitCommand.Length == 4 ? splitCommand[3] : pageNum;
            // Read Book Page 1
            if (book == "read")
            {
                _writeToClient.WriteLine("Read what?", player.ConnectionId);
                return;
            }

            var nthTarget = Helpers.findNth(book);
            var item = Helpers.findObjectInInventory(nthTarget, player);

            if (item == null)
            {
                if (book.Contains("sign") || book.Contains("note") || book.Contains("letter") || book.Contains("board"))
                {
                    _writeToClient.WriteLine("To read signs or notes just look at them instead.", player.ConnectionId);
                    return;
                }
                _writeToClient.WriteLine("You can't find that.", player.ConnectionId);
                return;
            }

            if (item.ItemType != Item.Item.ItemTypes.Book)
            {
                _writeToClient.WriteLine($"{item.Name} is not a book.", player.ConnectionId);
                return;
            }

            if (String.IsNullOrEmpty(pageNum))
            {
                _writeToClient.WriteLine($"{item.Name} <br /> {item.Description.Look}<br /> To read the pages enter: 'Read {book} 1' to view page 1.", player.ConnectionId);
                return;
            }
            int.TryParse(pageNum, out var n);
            if (n != 0)
            {
                n--;
            }

            if (n < 0)
            {
                n = 0;
            }
            if (n == item.Book.Pages.Count)
            {
                _writeToClient.WriteLine($"That exeeds the page count of {item.Book.Pages.Count}", player.ConnectionId);
                return;
            }

            if (n >= item.Book.PageCount)
            {

                _writeToClient.WriteLine($"{item.Name} does not contain that many pages.", player.ConnectionId);

                return;
            }

            if (string.IsNullOrEmpty(item.Book.Pages[n]))
            {
                _writeToClient.WriteLine($"This page is blank.", player.ConnectionId);
                return;
            }

            var result = Markdown.ToHtml(item.Book.Pages[n]);
            _writeToClient.WriteLine($"{result}", player.ConnectionId);
        }

        public void Write(Player player, string book, string pageNum, string fullCommand)
        {
            var splitCommand = fullCommand.Split(" ");

            pageNum = splitCommand.Length == 4 ? splitCommand[3] : pageNum;

            var isTitle = splitCommand[2] == "title" ? true : false;

            if (book == "write")
            {
                _writeToClient.WriteLine("Write in what?", player.ConnectionId);
                return;
            }

            var nthTarget = Helpers.findNth(book);
            var item = Helpers.findObjectInInventory(nthTarget, player);

            if (item == null)
            {
                _writeToClient.WriteLine("You can't find that.", player.ConnectionId);
                return;
            }

            if (item.ItemType != Item.Item.ItemTypes.Book)
            {
                _writeToClient.WriteLine($"{item.Name} is not a book.", player.ConnectionId);
                return;
            }

            if (isTitle)
            {
                // in this context pageNum would be the title
                // yes this is dumb, future Liam will curse at
                // this no doubt -_-

                var title = fullCommand.Remove(0, Helpers.GetNthIndex(fullCommand, ' ', 3));


                _writeToClient.WriteLine($"{item.Name} has now been titled {title}.", player.ConnectionId);
                item.Name = title;

                _clientUi.UpdateInventory(player);

                return;
            }

            int.TryParse(pageNum, out var n);

            if (n != 0)
            {
                n--;

            }

            if (n < 0)
            {
                n = 0;
            }

            if (n >= item.Book.PageCount)
            {

                _writeToClient.WriteLine($"{item.Name} does not contain that many pages.", player.ConnectionId);

                return;
            }

            if (item.Book.PageCount > item.Book.Pages.Count)
            {
                var diff = item.Book.PageCount - item.Book.Pages.Count;

                for (int i = 0; i < diff; i++)
                {
                    item.Book.Pages.Add(String.Empty);
                }
            }

            var bookContent = new WriteBook()
            {
                Title = item.Name,
                Description = item.Book.Pages[n],
                PageNumber = n
            };

            _clientUi.UpdateContentPopUp(player, bookContent);

            _writeToClient.WriteLine($"You begin to writing in your book.", player.ConnectionId);

        }

        public static string Replace(string source, string oldString,
                             string newString, StringComparison comparison,
                             bool recursive = false)
        {
            int index = source.IndexOf(oldString, comparison);

            while (index > -1)
            {
                source = source.Remove(index, oldString.Length);
                source = source.Insert(index, newString);

                if (!recursive)
                {
                    return source;
                }
                index = source.IndexOf(oldString, index + newString.Length, comparison);
            }

            return source;
        }

        public List<string> Hints()
        {
            var hints = new List<string>()
           {
               "If haven't already, join the community on discord https://discord.gg/Cc86jB4U49",
               "If you're new and unsure on what to do read the guide https://www.archaicquest.com/guide",
               "Need help?  join the community on discord https://discord.gg/Cc86jB4U49",
               "To quickly see what's near you can use the scan command.",
               "This MUD is in progress and still being worked on.",
               "Some mobs drop randomly generated loot, if you're lucky",
               "Pay attention to room emotes, you may discover a secret",
               "Enter score to view your chacater information",
               "Don't forget to enter a description for your character, this makes the game more immersive for others",
               "The Academy is a playground for new players to explore, there are multiple quests, secrets and areas to explore",
               "If you like AQ let people know about it in r/mud or on the discord",
              "To get items from a container the syntax is get <item name> <container>. example: Get bread bag",
              "If there are several things the same that you want to look at you can target them using 2.<keyword> for example get 5.sword. will get the 5th sword in the room.",
               "ArchaicQuest is a role-play encouraged MUD so you must stay IC (in character) and have a name and description that matches the setting. The academy area is OOC(out of character) to help you learn the game.",
               "If you're enjoying your time, bring a friend next time and share on social media, lets get more folks playing",
               "ArchaicQuest is a PvE MUD with optional PvP clans for those that enjoy player Vs player combat.",
               "ArchaicQuest features, Cooking and crafting. The cooking is inspired by BOTW",
               "At the moment you can't turn hints off, that will be coming soon."
           };

            return hints;
        }

        public void DBDumpToJSON(Player player)
        {
            _db.ExportDBToJSON();
        }
    }
}
