using System.Linq;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Model;

namespace ArchaicQuestII.GameLogic.Core
{
    public class QuestLog : IQuestLog
    {
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

                    Services.Instance.Writer.WriteLine(
                        $"<h3 class='gain'>{quest.Title} Completed!</h3><p>Return to the quest giver for your reward.</p>",
                        player
                    );
                }
            }
            Services.Instance.UpdateClient.UpdateQuest(player);
        }
    }
}
