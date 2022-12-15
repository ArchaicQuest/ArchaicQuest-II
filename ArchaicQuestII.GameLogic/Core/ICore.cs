using System.Collections.Generic;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Class;
using ArchaicQuestII.GameLogic.Character.Gain;
using ArchaicQuestII.GameLogic.Combat;
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
        public IMobScripts MobScripts { get; }
        
        /// <summary>
        /// Displays lists of players 
        /// </summary>
        /// <param name="player"></param>
        void DBDumpToJSON(Player player);
        List<string> Hints();
        void Read(Player player, string book, string pageNum, string fullCommand);
        void Write(Player player, string book, string pageNum, string fullCommand);
        void GainSkillProficiency(SkillList foundSkill, Player player);
        /// <summary>
        /// Display questlog
        /// </summary>
        /// <param name="player"></param>
        void QuestLog(Player player);
        void Recall(Player player, Room room);
        /// <summary>
        /// Train attributes
        /// </summary>
        /// <param name="player"></param>
        /// <param name="room"></param>
        /// <param name="stat"></param>
        void Train(Player player, Room room, string stat);
        public void RestorePlayer(Player player);

        void TrainSkill(Player player);

        /// <summary>
        /// Admin only used to test mob script events
        /// </summary>
        /// <param name="player"></param>
        /// <param name="eventName"></param>
        /// <param name="value"></param>
        void SetEvent(Player player, string eventName, string value);
    }
}
