using ArchaicQuestII.GameLogic.Character;
using System.Web;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Utilities;
using System.Collections.Generic;
using System.Linq;
using System;

namespace ArchaicQuestII.GameLogic.Loops
{
    public class HintLoop : ILoop
    {
        public int TickDelay => 120000;
        public bool ConfigureAwait => false;
        private List<Player> _players = new List<Player>();
        private List<string> _hints = new List<string>()
        {
            "If you get lost, enter recall to return to the starting room.",
            "If you need help use newbie to send a message. newbie help me",
            "ArchaicQuest is a new game so might be quite, join the discord to chat to others https://discord.gg/QVF6Uutt",
            "To communicate enter say then the message to speak. such as say hello there"
        };

        public void PreTick()
        {
            _players = Services.Instance.Cache
                .GetPlayerCache()
                .Values.Where(x => x.Config.Hints == true)
                .ToList();
        }

        public void Tick()
        {
            //Console.WriteLine("HintLoop");

            foreach (var player in _players)
            {
                Services.Instance.Writer.WriteLine(
                    $"<span style='color:lawngreen'>[Hint]</span> {HttpUtility.HtmlEncode(_hints[DiceBag.Roll(1, 0, _hints.Count)])}",
                    player
                );
            }
        }

        public void PostTick()
        {
            _players.Clear();
        }
    }
}
