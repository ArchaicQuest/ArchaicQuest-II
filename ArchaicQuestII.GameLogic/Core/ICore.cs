using System.Collections.Generic;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Class;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Core
{
    public interface ICore
    {
        /// <summary>
        /// Displays lists of players 
        /// </summary>
        /// <param name="player"></param>
        void DBDumpToJSON(Player player);
        List<string> Hints();
        void Scan(Player player, Room room, string direction);
        void ScanDirection(Player player, Room room, string direction);
        void Affects(Player player);
        void CheckPose(Player player);
        void ImmTeleport(Player player, Room room, string location);
        void Practice(Player player, Room room, string skillName);
        void Read(Player player, string book, string pageNum, string fullCommand);
        void Write(Player player, string book, string pageNum, string fullCommand);
        bool SkillCheckSuccesful(SkillList skill);
        void GainSkillProficiency(SkillList foundSkill, Player player);
        void Save(Player player);
        void Quit(Player player, Room room);
        /// <summary>
        /// Where are players within the area
        /// </summary>
        /// <param name="player"></param>
        /// <param name="room"></param>
        void Where(Player player, Room room);
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
        /// <summary>
        /// Eat food
        /// </summary>
        /// <param name="player"></param>
        /// <param name="room"></param>
        /// <param name="obj">name of food to eat</param>
        void Eat(Player player, Room room, string obj);
        void Drink(Player player, Room room, string obj);

        void TrainSkill(Player player);

        void Dismount(Player player, Room room);

        /// <summary>
        /// Admin only used to test mob script events
        /// </summary>
        /// <param name="player"></param>
        /// <param name="eventName"></param>
        /// <param name="value"></param>
        void SetEvent(Player player, string eventName, string value);

        void Harvest(Player player, string item, Room room);

        public bool CheckSkillSuccessAndImprove(Player player, string skillName);

        void InitFightStatus(Player player, Player target);
    }
}
