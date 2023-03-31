using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Commands;
using ArchaicQuestII.GameLogic.Core;

namespace ArchaicQuestII.GameLogic.Loops
{
    public class LagLoop : ILoop
    {
        public int TickDelay => 125; //4000 for lag

        public bool ConfigureAwait => true;

        private int LagTick = 32;
        private ICore _core;
        private ICommandHandler _commandHandler;
        private List<Player> _laggedPlayers;
        private List<Player> _bufferedPlayers;

        public void Init(ICore core, ICommandHandler commandHandler)
        {
            _core = core;
            _commandHandler = commandHandler;
        }

        public void PreTick()
        {
            var players = _core.Cache.GetPlayerCache().Values;
            _laggedPlayers = players.Where(x => x.Lag > 0).ToList();
            _bufferedPlayers = players.Where(x => x.Buffer.Count > 0).ToList();
        }

        public void Tick()
        {

            foreach (var player in _bufferedPlayers)
            {
                // don't action commands if player is lagged
                if (player.Lag > 0)
                {
                    continue;
                }

                var command = player.Buffer.Dequeue();
                var room = _core.Cache.GetRoom(player.RoomId);
                player.LastCommandTime = DateTime.Now;

                if (player.CommandLog.Count >= 2500)
                {
                    player.CommandLog = new List<string>();
                }

                player.CommandLog.Add($"{string.Format("{0:f}", DateTime.Now)} - {command}");
                _commandHandler.HandleCommand(player, room, command);
            }

            LagTick--;

            if(LagTick <= 0)
            {
                foreach (var player in _laggedPlayers)
                {
                    player.Lag -= 1;
                }

                LagTick = 32;
            }
        }

        public void PostTick()
        {
            _laggedPlayers.Clear();
        }
    }
}

