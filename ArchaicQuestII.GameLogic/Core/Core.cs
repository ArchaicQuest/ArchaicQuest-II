using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Class;
using ArchaicQuestII.GameLogic.Character.Gain;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.World.Room;
using Markdig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Combat;

namespace ArchaicQuestII.GameLogic.Core
{
    public class Core : ICore
    {
        public ICache Cache { get; }
        public IWriteToClient Writer { get; }
        public IDataBase DataBase { get; }
        public IPlayerDataBase PlayerDataBase { get; }
        public IUpdateClientUI UpdateClient { get; }
        public IDice Dice { get; }
        public IGain Gain { get; }
        public ICombat Combat { get; }
        public IRoomActions RoomActions { get; }
        public IMobScripts MobScripts { get; }
        
        public Core(ICache cache, 
            IWriteToClient writeToClient, 
            IDataBase dataBase, 
            IUpdateClientUI updateClient, 
            IDice dice, 
            IGain gain, 
            ICombat combat, 
            IPlayerDataBase playerDataBase, 
            IRoomActions roomActions,
            IMobScripts mobScripts)
        {
            Cache = cache;
            Writer = writeToClient;
            DataBase = dataBase;
            UpdateClient = updateClient;
            Dice = dice;
            Gain = gain;
            Combat = combat;
            PlayerDataBase = playerDataBase;
            RoomActions = roomActions;
            MobScripts = mobScripts;
        }

        public void QuestLog(Player player)
        {
            var sb = new StringBuilder();

            foreach (var q in player.QuestLog)
            {
                sb.Append($"<div class='quest-block'><h3>{q.Title}</h3><p>{q.Area}</p>");

                if (q.Type == QuestTypes.Kill)
                {
                    sb.Append("<p>Kill:</p>");
                }

                sb.Append("<ol>");
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

            Writer.WriteLine(sb.ToString(), player.ConnectionId);

        }

        public void Recall(Player player, Room room)
        {
            if ((player.Status & CharacterStatus.Status.Sleeping) != 0)
            {
                Writer.WriteLine("In your dreams, or what?", player.ConnectionId);
                return;
            }
            if ((player.Status & CharacterStatus.Status.Sitting) != 0)
            {
                Writer.WriteLine("Better stand up first.", player.ConnectionId);
                return;
            }
            if ((player.Status & CharacterStatus.Status.Resting) != 0)
            {
                Writer.WriteLine("Nah... You feel too relaxed...", player.ConnectionId);
                return;
            }

            foreach (var pc in room.Players)
            {
                if (pc.Id == player.Id)
                {
                    Writer.WriteLine("You glow a bright light before vanishing.", pc.ConnectionId);
                    continue;
                }
                Writer.WriteLine($"{player.Name} glows a bright light before vanishing.", pc.ConnectionId);
            }

            room.Players.Remove(player);

            var recallRoom = Cache.GetRoom(player.RecallId);

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
                Writer.WriteLine($"{player.Name} suddenly appears in a flash of bright light.", pc.ConnectionId);
            }

            player.Buffer.Enqueue("l");

            player.Attributes.Attribute[EffectLocation.Moves] = player.Attributes.Attribute[EffectLocation.Moves] / 2;

            if (player.Attributes.Attribute[EffectLocation.Moves] < 1)
            {
                player.Attributes.Attribute[EffectLocation.Moves] = 0;
            }

            UpdateClient.UpdateScore(player);
            UpdateClient.UpdateMoves(player);
            UpdateClient.GetMap(player, Cache.GetMap($"{recallRoom.AreaId}{recallRoom.Coords.Z}"));

        }

        public void Train(Player player, Room room, string stat)
        {
            if ((player.Status & CharacterStatus.Status.Sleeping) != 0)
            {
                Writer.WriteLine("In your dreams, or what?", player.ConnectionId);
                return;
            }

            if (room.Mobs.Find(x => x.Trainer) == null)
            {
                Writer.WriteLine("You can't do that here.", player.ConnectionId);
                return;
            }

            if (player.Trains <= 0)
            {
                Writer.WriteLine("You have no training sessions left.", player.ConnectionId);
                return;
            }

            if (string.IsNullOrEmpty(stat) || stat == "train")
            {

                Writer.WriteLine(
                    ($"<p>You have {player.Trains} training session{(player.Trains > 1 ? "s" : "")} remaining.<br />You can train: str dex con int wis cha hp mana move.</p>"
                    ), player.ConnectionId);
            }
            else
            {
                var statName = GetStatName(stat);
                if (string.IsNullOrEmpty(statName.Item1))
                {
                    Writer.WriteLine(
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
                    var hitDie = Cache.GetClass(player.ClassName);
                    var roll = Dice.Roll(1, hitDie.HitDice.DiceMinSize, hitDie.HitDice.DiceMaxSize);

                    player.MaxAttributes.Attribute[statName.Item2] += roll;
                    player.Attributes.Attribute[statName.Item2] += roll;

                    Writer.WriteLine(
                        ($"<p class='gain'>Your {statName.Item1} increases by {roll}.</p>"
                        ), player.ConnectionId);

                    UpdateClient.UpdateHP(player);
                    UpdateClient.UpdateMana(player);
                    UpdateClient.UpdateMoves(player);
                }
                else
                {
                    player.MaxAttributes.Attribute[statName.Item2] += 1;
                    player.Attributes.Attribute[statName.Item2] += 1;

                    Writer.WriteLine(
                        ($"<p class='gain'>Your {statName.Item1} increases by 1.</p>"
                        ), player.ConnectionId);
                }




                UpdateClient.UpdateScore(player);


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
            UpdateClient.UpdateHP(player);
            UpdateClient.UpdateMoves(player);
            UpdateClient.UpdateMana(player);

            Writer.WriteLine("You are restored.", player.ConnectionId);
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
            var success = Dice.Roll(1, 1, 100);

            if (success == 1 || success == 101)
            {
                return false;
            }

            return proficiency >= success;
        }

        public void GainSkillProficiency(SkillList foundSkill, Player player)
        {

            var getSkill = Cache.GetSkill(foundSkill.SkillId);

            if (getSkill == null)
            {
                var skill = Cache.GetAllSkills().FirstOrDefault(x => x.Name.Equals(foundSkill.SkillName, StringComparison.CurrentCultureIgnoreCase));
                foundSkill.SkillId = skill.Id;
            }


            if (foundSkill.Proficiency == 100)
            {
                return;
            }

            var increase = Dice.Roll(1, 1, 5);

            foundSkill.Proficiency += increase;

            Gain.GainExperiencePoints(player, 100 * foundSkill.Level / 4, false);

            UpdateClient.UpdateExp(player);

            Writer.WriteLine(
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

            Writer.WriteLine(sb.ToString(), player.ConnectionId);


        }

        public void Practice(Player player, Room room, string skillname)
        {
            if ((player.Status & CharacterStatus.Status.Sleeping) != 0)
            {
                Writer.WriteLine("In your dreams, or what?", player.ConnectionId);
                return;
            }

            if (room.Mobs.Find(x => x.Trainer) == null)
            {
                Writer.WriteLine("You can't do that here.", player.ConnectionId);
                return;
            }

            var trainerName = room.Mobs.Find(x => x.Trainer).Name;

            var skillName = skillname == "prac" || skillname == "practice" ? "" : skillname;

            if (string.IsNullOrEmpty(skillName))
            {

                Writer.WriteLine($"You have {player.Practices} practice{(player.Practices <= 1 ? "" : "s")} left.", player.ConnectionId);

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
                Writer.WriteLine(sb.ToString(), player.ConnectionId);
                return;
            }

            var foundSkill = player.Skills.Find(x => x.SkillName.StartsWith(skillName, StringComparison.OrdinalIgnoreCase));

            if (foundSkill == null)
            {
                Writer.WriteLineMobSay(trainerName, $"You don't have that skill to practice.", player.ConnectionId);
                return;
            }

            if (player.Practices == 0)
            {
                Writer.WriteLineMobSay(trainerName, $"You have no practices left.", player.ConnectionId);
                return;
            }

            if (foundSkill.Proficiency == 100)
            {
                Writer.WriteLineMobSay(trainerName, $"You have already mastered {foundSkill.SkillName}.", player.ConnectionId);
                return;
            }

            if (foundSkill.Proficiency >= 75)
            {
                Writer.WriteLineMobSay(trainerName, $"I've taught you everything I can about {foundSkill.SkillName}.", player.ConnectionId);
                return;
            }

            var maxGain = player.Attributes.Attribute[EffectLocation.Intelligence];
            var minGain = player.Attributes.Attribute[EffectLocation.Intelligence] / 2;
            var gain = Dice.Roll(1, minGain, maxGain);

            foundSkill.Proficiency += gain;
            player.Practices -= 1;

            if (foundSkill.Proficiency >= 75)
            {
                foundSkill.Proficiency = 75;
                Writer.WriteLine($"You practice for some time. Your proficiency with {foundSkill.SkillName} is now {foundSkill.Proficiency}%", player.ConnectionId);
                Writer.WriteLineMobSay(trainerName, $"You'll have to practice it on your own now...", player.ConnectionId);
                return;
            }

            Writer.WriteLine($"You practice for some time. Your proficiency with {foundSkill.SkillName} is now {foundSkill.Proficiency}%", player.ConnectionId);


        }

        public void SetEvent(Player player, string eventName, string value)
        {
            if (eventName.Equals("/setevent", StringComparison.CurrentCultureIgnoreCase) || string.IsNullOrEmpty(eventName))
            {
                foreach (var ev in player.EventState)
                {
                    Writer.WriteLine($"{ev.Key} - {ev.Value}", player.ConnectionId);
                }

                return;
            }

            if (player.EventState.ContainsKey(eventName))
            {
                player.EventState[eventName] = Int32.Parse(value);
                Writer.WriteLine($"{eventName} state changed to {player.EventState[eventName]}", player.ConnectionId);
                return;
            }

            Writer.WriteLine($"Invalid Event state", player.ConnectionId);
        }

        public void Read(Player player, string book, string pageNum, string fullCommand)
        {

            var splitCommand = fullCommand.Split(" ");
            pageNum = splitCommand.Length == 4 ? splitCommand[3] : pageNum;
            // Read Book Page 1
            if (book == "read")
            {
                Writer.WriteLine("Read what?", player.ConnectionId);
                return;
            }

            var nthTarget = Helpers.findNth(book);
            var item = Helpers.findObjectInInventory(nthTarget, player);

            if (item == null)
            {
                if (book.Contains("sign") || book.Contains("note") || book.Contains("letter") || book.Contains("board"))
                {
                    Writer.WriteLine("To read signs or notes just look at them instead.", player.ConnectionId);
                    return;
                }
                Writer.WriteLine("You can't find that.", player.ConnectionId);
                return;
            }

            if (item.ItemType != Item.Item.ItemTypes.Book)
            {
                Writer.WriteLine($"{item.Name} is not a book.", player.ConnectionId);
                return;
            }

            if (String.IsNullOrEmpty(pageNum))
            {
                Writer.WriteLine($"{item.Name} <br /> {item.Description.Look}<br /> To read the pages enter: 'Read {book} 1' to view page 1.", player.ConnectionId);
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
                Writer.WriteLine($"That exeeds the page count of {item.Book.Pages.Count}", player.ConnectionId);
                return;
            }

            if (n >= item.Book.PageCount)
            {

                Writer.WriteLine($"{item.Name} does not contain that many pages.", player.ConnectionId);

                return;
            }

            if (string.IsNullOrEmpty(item.Book.Pages[n]))
            {
                Writer.WriteLine($"This page is blank.", player.ConnectionId);
                return;
            }

            var result = Markdown.ToHtml(item.Book.Pages[n]);
            Writer.WriteLine($"{result}", player.ConnectionId);
        }

        public void Write(Player player, string book, string pageNum, string fullCommand)
        {
            var splitCommand = fullCommand.Split(" ");

            pageNum = splitCommand.Length == 4 ? splitCommand[3] : pageNum;

            var isTitle = splitCommand[2] == "title" ? true : false;

            if (book == "write")
            {
                Writer.WriteLine("Write in what?", player.ConnectionId);
                return;
            }

            var nthTarget = Helpers.findNth(book);
            var item = Helpers.findObjectInInventory(nthTarget, player);

            if (item == null)
            {
                Writer.WriteLine("You can't find that.", player.ConnectionId);
                return;
            }

            if (item.ItemType != Item.Item.ItemTypes.Book)
            {
                Writer.WriteLine($"{item.Name} is not a book.", player.ConnectionId);
                return;
            }

            if (isTitle)
            {
                // in this context pageNum would be the title
                // yes this is dumb, future Liam will curse at
                // this no doubt -_-

                var title = fullCommand.Remove(0, Helpers.GetNthIndex(fullCommand, ' ', 3));


                Writer.WriteLine($"{item.Name} has now been titled {title}.", player.ConnectionId);
                item.Name = title;

                UpdateClient.UpdateInventory(player);

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

                Writer.WriteLine($"{item.Name} does not contain that many pages.", player.ConnectionId);

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

            UpdateClient.UpdateContentPopUp(player, bookContent);

            Writer.WriteLine($"You begin to writing in your book.", player.ConnectionId);

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
               "Enter score to view your character information",
               "Don't forget to enter a description for your character, this makes the game more immersive for others",
               "The Academy is a playground for new players to explore, there are multiple quests, secrets and areas to explore",
               "If you like AQ let people know about it in r/mud or on the discord",
              "To get items from a container the syntax is get <item name> <container>. example: Get bread bag",
              "If there are several things the same that you want to look at you can target them using 2.<keyword> for example get 5.sword. will get the 5th sword in the room.",
               "ArchaicQuest is a role-play encouraged MUD so you must stay IC (in character) and have a name and description that matches the setting. The academy area is OOC(out of character) to help you learn the game.",
               "If you're enjoying your time, bring a friend next time and share on social media, lets get more folks playing",
               "ArchaicQuest is a PvE MUD with optional PvP clans for those that enjoy player Vs player combat.",
               "ArchaicQuest features, Cooking and crafting. The cooking is inspired by BOTW",
               "At the moment you can't turn hints off, that will be coming soon.",
               "Click the settings cog to change font type and size as well as other options"
           };

            return hints;
        }

        public void DBDumpToJSON(Player player)
        {
            DataBase.ExportDBToJSON();
        }
    }
}
