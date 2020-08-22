using System;
using ArchaicQuestII.GameLogic.Commands;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Combat;

namespace ArchaicQuestII.GameLogic.Core
{

    public class GameLoop : IGameLoop
    {


        private IWriteToClient _writeToClient;
        private ICache _cache;
        private ICommands _commands;
        private ICombat _combat;

        public GameLoop(IWriteToClient writeToClient, ICache cache, ICommands commands, ICombat combat)
        {
            _writeToClient = writeToClient;
            _cache = cache;
            _commands = commands;
            _combat = combat;
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
            Console.WriteLine("started loop");
            while (true)
            {
                await Task.Delay(1000);
                Console.WriteLine(" loop");
                var players = _cache.GetPlayerCache();
                 

                foreach (var player in players)
                {
                   // _writeToClient.WriteLine("update", player.Value.ConnectionId);

                }
            }
        }

        public async Task UpdateCombat()
        {
 // create a combat cache to add mobs too so they can fight back
 // block movement while fighting
 // end fight if target is not there / dead
 // create flee commant
            Console.WriteLine("started combat loop");
            while (true)
            {
                await Task.Delay(3000);
                Console.WriteLine("combat loop");
                var players = _cache.GetPlayerCache();
                var validPlayers = players.Where(x => x.Value.Status == CharacterStatus.Status.Fighting);

                foreach (var player in validPlayers)
                {
                    _combat.Fight(player.Value, player.Value.Target, _cache.GetRoom(player.Value.RoomId), false);

                }
            }
        }



        public async Task UpdatePlayers()
        {
            while (true)
            {

                try
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
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

    }


}