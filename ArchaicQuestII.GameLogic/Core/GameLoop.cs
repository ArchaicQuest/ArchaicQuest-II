using ArchaicQuestII.GameLogic.Commands;
using System.Linq;
using System.Threading.Tasks;

namespace ArchaicQuestII.GameLogic.Core
{

    public class GameLoop : IGameLoop
    {


        private IWriteToClient _writeToClient;
        private ICache _cache;
        private ICommands _commands;

        public GameLoop(IWriteToClient writeToClient, ICache cache, ICommands commands)
        {
            _writeToClient = writeToClient;
            _cache = cache;
            _commands = commands;
        }


        public async Task UpdateTime()
        {
            _writeToClient.WriteLine("start looper ");
            //var players = _cache.GetPlayerCache();
            //var validPlayers = players.Where(x => x.Value.Buffer.Count > 0);

            //foreach (var player in validPlayers)
            //{
            //    _writeToClient.WriteLine("update", player.Value.ConnectionId);

            //}
            while (true)
            {
                await Task.Delay(1000);

                var players = _cache.GetPlayerCache();
                var validPlayers = players.Where(x => x.Value.Buffer.Count > 0);

                foreach (var player in players)
                {
                   // _writeToClient.WriteLine("update", player.Value.ConnectionId);

                }
            }
        }


        public async Task UpdatePlayers()
        {
            while (true)
            {
                await Task.Delay(125);
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


}