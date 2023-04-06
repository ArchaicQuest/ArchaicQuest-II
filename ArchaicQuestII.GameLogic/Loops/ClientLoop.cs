using System;
using System.Collections.Generic;
using System.Linq;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Commands;
using ArchaicQuestII.GameLogic.Core;

namespace ArchaicQuestII.GameLogic.Loops
{
    public class ClientLoop : ILoop
    {
        public int TickDelay => 125;

        public bool ConfigureAwait => true;

        private List<Player> _players;

        private ICore _core;

        public void Init(ICore core, ICommandHandler commandHandler)
        {
            _core = core;
            _players = new List<Player>();
        }

        public void PreTick()
        {
            _players = _core.Cache.GetPlayerCache().Values.ToList();
        }

        public void Tick()
        {
            foreach (var player in _players)
            {
                _core.UpdateClient.UpdateScore(player);
            }
        }

        public void PostTick()
        {
            _players.Clear();
        }
    }
}

