using System.Text;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Info
{
    public class QuestLogCmd : ICommand
    {
        public QuestLogCmd()
        {
            Aliases = new[] { "questlog", "qlog" };
            Description = "Displays your current quests.";
            Usages = new[] { "Type: questlog" };
            Title = "";
            DeniedStatus = null;
            UserRole = UserRole.Player;
        }

        public string[] Aliases { get; }
        public string Description { get; }
        public string[] Usages { get; }
        public string Title { get; }
        public CharacterStatus.Status[] DeniedStatus { get; }
        public UserRole UserRole { get; }

        public void Execute(Player player, Room room, string[] input)
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

                sb.Append(
                    $"</ol><p>{q.Description}</p><p>Reward:</p><ul><li>{q.ExpGain} Experience points</li><li>{q.GoldGain} Gold</li>"
                );

                foreach (var i in q.ItemGain)
                {
                    sb.Append($"<li>{i.Name}</li>");
                }

                sb.Append("</ul></div>");
            }

            Services.Instance.Writer.WriteLine(sb.ToString(), player);
        }
    }
}
