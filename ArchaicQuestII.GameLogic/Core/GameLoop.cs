using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ArchaicQuestII.GameLogic.Commands;

namespace ArchaicQuestII.GameLogic.Core
{
    public class GameLoop: IGameLoop
    {
        private readonly ICache _cache;
        private readonly ICommands _commands;
        const int TICKS_PER_SECOND = 8;
        const int SKIP_TICKS = 1000 / TICKS_PER_SECOND;
 
        public GameLoop(ICache cache, ICommands commands)
        {
            _cache = cache;
            _commands = commands;
        }
        public void MainLoop()
        {

            var nextGameTick = Environment.TickCount;
            var sleepTime = 0;
            var gameIsRunning = true;

            while (gameIsRunning)
            { 
                UpdatePlayers();

                nextGameTick += SKIP_TICKS;
                sleepTime = nextGameTick - Environment.TickCount;

                if (sleepTime >= 0)
                {
                    Thread.Sleep(sleepTime);
                }
                else
                {
                    // Shit, we are running behind!
                    
                }

            }
        }

        public void UpdatePlayers()
        {
            var players = _cache.GetPlayerCache();
            var validPlayers = players.Where(x => x.Value.Buffer.Count > 0);

            foreach (var player in validPlayers)
            {
                var command = player.Value.Buffer.Pop();
                var room = _cache.GetRoom(player.Value.RoomId);

                _commands.ProcessCommand(command, player.Value, room);

            }

        }
    }
}
