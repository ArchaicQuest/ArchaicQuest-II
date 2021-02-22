using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Hubs;
using ArchaicQuestII.GameLogic.World.Area;
using ArchaicQuestII.GameLogic.World.Room;
using Microsoft.AspNetCore.SignalR;

namespace ArchaicQuestII.GameLogic.Core
{
   public class Core: ICore
    {
        private readonly ICache _cache;
        private readonly IWriteToClient _writeToClient;
        private readonly IDataBase _db;
        private readonly IUpdateClientUI _clientUi;
        private readonly IDice _dice;
        public Core(ICache cache, IWriteToClient writeToClient, IDataBase db, IUpdateClientUI clientUi, IDice dice)
        {
            _cache = cache;
            _writeToClient = writeToClient;
            _db = db;
            _clientUi = clientUi;
            _dice = dice;
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
                    $"<span class='player'>{pc.Value.Name}</span></li>");
            }

            sb.Append("</ul>");
            sb.Append($"<p>Players found: {_cache.GetPlayerCache().Count}</p>");

            _writeToClient.WriteLine(sb.ToString(), player.ConnectionId);


        }

        public void Save(Player player)
        {
            _db.Save(player, DataBase.Collections.Players);
            _writeToClient.WriteLine("Character saved.", player.ConnectionId);
        }

        public void Quit(Player player, Room room)
        {

            player.Buffer = new Queue<string>();
           Save(player);

           foreach (var pc in room.Players)
           {
               if (pc.Name.Equals(player.Name))
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
                    ));
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
    }
}
