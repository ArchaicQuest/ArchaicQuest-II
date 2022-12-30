using System.Collections.Generic;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Class;
using ArchaicQuestII.GameLogic.Character.Gain;
using ArchaicQuestII.GameLogic.Client;
using ArchaicQuestII.GameLogic.Combat;
using ArchaicQuestII.GameLogic.Skill.Skills;
using ArchaicQuestII.GameLogic.World.Area;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Core
{
    public interface ICore
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
        public IAreaActions AreaActions { get; }
        public IMobScripts MobScripts { get; }
        public IErrorLog ErrorLog { get; }
        public IPassiveSkills PassiveSkills { get; }
        public IFormulas Formulas { get; }
        public ITime Time { get; }
        
        /// <summary>
        /// Displays lists of players 
        /// </summary>
        /// <param name="player"></param>
        void DBDumpToJSON(Player player);
        List<string> Hints();
        void GainSkillProficiency(SkillList foundSkill, Player player);
        public void RestorePlayer(Player player);
        bool CommandTargetCheck(string target, Player player, string errorMessage = "What?");
    }
}
