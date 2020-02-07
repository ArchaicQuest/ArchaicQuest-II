using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ArchaicQuestII.GameLogic.Commands;
using ArchaicQuestII.GameLogic.Hubs;
using Microsoft.AspNet.SignalR;

namespace ArchaicQuestII.GameLogic.Core
{

    public class GameLoop {


        private IWriteToClient _writeToClient;
        private ICache _cache;
        public GameLoop(IWriteToClient writeToClient, ICache cache)
        {
            _writeToClient = writeToClient;
            _cache = cache;
        }


        public async Task UpdateTime()
        {
            while (true)
            {
                await Task.Delay(60000);

                var players = _cache.GetPlayerCache();
                var validPlayers = players.Where(x => x.Value.Buffer.Count > 0);

                foreach (var player in validPlayers)
                {
                    _writeToClient.WriteLine("update", player.Value.ConnectionId);

                }   
            }
        }
    }

        //public void UpdatePlayers()
        //{
        //    var players = _cache.GetPlayerCache();
        //    var validPlayers = players.Where(x => x.Value.Buffer.Count > 0);

        //    foreach (var player in validPlayers)
        //    {
        //        var command = player.Value.Buffer.Pop();
        //        var room = _cache.GetRoom(player.Value.RoomId);

        //        _commands.ProcessCommand(command, player.Value, room);

        //    }

 
    }