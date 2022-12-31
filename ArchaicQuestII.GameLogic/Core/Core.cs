using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Class;
using ArchaicQuestII.GameLogic.Character.Gain;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.World.Room;
using System;
using System.Collections.Generic;
using System.Linq;
using ArchaicQuestII.GameLogic.Client;
using ArchaicQuestII.GameLogic.Combat;
using ArchaicQuestII.GameLogic.Skill.Skills;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Area;

namespace ArchaicQuestII.GameLogic.Core
{
    public class Core : ICore
    {
        public ICache Cache { get; }
        public IWriteToClient Writer { get; }
        public IDataBase DataBase { get; }
        public IPlayerDataBase PlayerDataBase { get; }
        public IUpdateClientUI UpdateClient { get; }
        public IGain Gain { get; }
        public ICombat Combat { get; }
        public IRoomActions RoomActions { get; }
        public IAreaActions AreaActions { get; }
        public IMobScripts MobScripts { get; }
        
        public IPassiveSkills PassiveSkills { get; }
        public IFormulas Formulas { get; }

        public IErrorLog ErrorLog { get; }
        
        public ITime Time { get; }

        public Core(ICache cache, 
            IWriteToClient writeToClient, 
            IDataBase dataBase, 
            IUpdateClientUI updateClient,
            IGain gain, 
            ICombat combat, 
            IPlayerDataBase playerDataBase, 
            IRoomActions roomActions,
            IMobScripts mobScripts,
            IErrorLog errorLog,
            IPassiveSkills passiveSkills,
            IFormulas formulas,
            ITime time)
        {
            Cache = cache;
            Writer = writeToClient;
            DataBase = dataBase;
            UpdateClient = updateClient;
            Gain = gain;
            Combat = combat;
            PlayerDataBase = playerDataBase;
            RoomActions = roomActions;
            MobScripts = mobScripts;
            ErrorLog = errorLog;
            PassiveSkills = passiveSkills;
            Formulas = formulas;
            Time = time;
        }
        
        public bool CommandTargetCheck(string target, Player player, string errorMessage = "What?")
        {
            if (!string.IsNullOrEmpty(target)) return true;
            Writer.WriteLine(errorMessage, player.ConnectionId);
            return false;
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

            var increase = DiceBag.Roll(1, 1, 5);

            foundSkill.Proficiency += increase;

            Gain.GainExperiencePoints(player, 100 * foundSkill.Level / 4, false);

            UpdateClient.UpdateExp(player);

            Writer.WriteLine(
                $"<p class='improve'>You learn from your mistakes and gain {100 * foundSkill.Level / 4} experience points.</p>" +
                $"<p class='improve'>Your knowledge of {foundSkill.SkillName} increases by {increase}%.</p>",
                player.ConnectionId, 0);
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
