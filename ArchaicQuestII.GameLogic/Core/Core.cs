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
                var statName = Helpers.GetStatName(stat);
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
