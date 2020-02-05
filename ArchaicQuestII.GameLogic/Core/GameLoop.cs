using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ArchaicQuestII.GameLogic.Commands;

namespace ArchaicQuestII.GameLogic.Core
{
    public class GameLoop
    {
        private readonly ICache _cache;
        private readonly ICommands _commands;
        public GameLoop(ICache cache, ICommands commands)
        {
            _cache = cache;
            _commands = commands;
        }
        public Task MainLoop()
        {
            while (true)
            {
               
                var players = _cache.GetPlayerCache();

                foreach (var player in players)
                {
                   var command = player.Value.Buffer.Pop();
                   var room = _cache.GetRoom(player.Value.RoomId);

                    _commands.ProcessCommand(command, player.Value, room);
                   
                }
 
 
            }
        }
    }
}
