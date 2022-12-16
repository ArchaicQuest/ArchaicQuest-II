using System.Linq;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Model;

namespace ArchaicQuestII.GameLogic.Core
{
    public class QuestLog : IQuestLog
    {
        private readonly IWriteToClient _writeToClient;
        private readonly IUpdateClientUI _updateClientUi;

        public QuestLog(IWriteToClient writeToClient, IUpdateClientUI updateClientUi)
        {
            _writeToClient = writeToClient;
            _updateClientUi = updateClientUi;
        }
        public void IsQuestMob(Player player, string mobName)
        {
            foreach (var quest in player.QuestLog)
            {
                if (quest.Type != QuestTypes.Kill)
                {
                    continue;
                }

                var questCompleted = false;

                foreach (var mob in quest.MobsToKill.Where(mob => mob.Name.Equals(mobName)))
                {
                    mob.Current = mob.Current + 1;
                    questCompleted = mob.Count == mob.Current;
                }
                
                if (questCompleted)
                {
                    quest.Completed = true;

                    _writeToClient.WriteLine($"<h3 class='gain'>{quest.Title} Completed!</h3><p>Return to the quest giver for your reward.</p>", player.ConnectionId);
                }
            }
            _updateClientUi.UpdateQuest(player);
        }
    }
}
