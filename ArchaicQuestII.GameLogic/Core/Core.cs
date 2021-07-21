using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Class;
using ArchaicQuestII.GameLogic.Character.Gain;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Hubs;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.World.Area;
using ArchaicQuestII.GameLogic.World.Room;
using Microsoft.AspNetCore.SignalR;

namespace ArchaicQuestII.GameLogic.Core
{
    public class Core : ICore
    {
        private readonly ICache _cache;
        private readonly IWriteToClient _writeToClient;
        private readonly IDataBase _db;
        private readonly IUpdateClientUI _clientUi;
        private readonly IDice _dice;
        private readonly IGain _gain;
        public Core(ICache cache, IWriteToClient writeToClient, IDataBase db, IUpdateClientUI clientUi, IDice dice, IGain gain)
        {
            _cache = cache;
            _writeToClient = writeToClient;
            _db = db;
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
            var hasBenefit = false;
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

      
    }
}
