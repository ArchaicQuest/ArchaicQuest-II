
using Microsoft.AspNetCore.SignalR;
using System;
using System.IO;
using System.Threading.Tasks;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Commands;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;
using Microsoft.Extensions.Logging;

namespace ArchaicQuestII.GameLogic.Hubs
{
    public class GameHub : Hub
    {
        private readonly ILogger<GameHub> _logger;
        private IDataBase _db { get; }
        private ICache _cache { get; }
        private readonly IWriteToClient _writeToClient;
        private readonly ICommands _commands;
        private int start = 0;
        public GameHub(IDataBase db, ICache cache, ILogger<GameHub> logger, IWriteToClient writeToClient, ICommands commands)
        {
            _logger = logger;
            _db = db;
            _cache = cache;
            _writeToClient = writeToClient;
            _commands = commands;
        }

 
        /// <summary>
        /// Do action when user connects 
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {

            if (start == 0)
            {
                start = 1;
                await Clients.All.SendAsync("SendMessage", "", "Starting loop");
            
            }
    
            
            await Clients.All.SendAsync("SendMessage", "", "Someone has entered the realm");
        }

        /// <summary>
        /// Do action when user disconnects 
        /// </summary>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception ex)
        {
            await Clients.All.SendAsync("SendMessage", "Someone", " hase left the realm");
        }

        /// <summary>
        /// get message from client
        /// </summary>
        /// <returns></returns>
        public void SendToServer(string message, string connectionId)
        {
           
            var player = _cache.GetPlayer(connectionId);
            player.Buffer.Push(message);
          
        }

        /// <summary>
        /// Send message to all clients
        /// </summary>
        /// <returns></returns>
        public async Task Send(string message)
        {
            _logger.LogInformation($"Player sent {message}");
            await Clients.All.SendAsync("SendMessage", "user x", message);
        }

        /// <summary>
        /// Send message to specific client
        /// </summary>
        /// <returns></returns>
        public async Task SendToClient(string message, string hubId)
        {
            _logger.LogInformation($"Player sent {message}");
            await Clients.Client(hubId).SendAsync("SendMessage", message);
        }

        public async void Welcome(string id)
        {
            var location = System.Reflection.Assembly.GetEntryAssembly().Location;
            var directory = System.IO.Path.GetDirectoryName(location);

            var motd = File.ReadAllText(directory + "/motd");
 
           await SendToClient(motd, id);
        }
 

        public void CreateCharacter(string name = "Liam")
        {
            var newPlayer = new Player()
            {
                Name = name
            };

            _db.Save(newPlayer, DataBase.Collections.Players);

        }

        public async void AddCharacter(string hubId, Guid characterId)
        {

            var player = GetCharacter(hubId, characterId);
            AddCharacterToCache(hubId, player);

          

            await SendToClient($"Welcome {player.Name}. Your adventure awaits you.", hubId);

            GetRoom(hubId, player);
        }

        /// <summary>
        /// Find Character in DB and add to cache
        /// Check if player is already in cache 
        /// if so kick em off
        /// </summary>
        /// <param name="hubId">string</param>
        /// <param name="characterId">guid</param>
        /// <returns>Player Character</returns>
        private Player GetCharacter(string hubId, Guid characterId)
        {
            var player = _db.GetById<Player>(characterId, DataBase.Collections.Players);
            player.ConnectionId = hubId;
            player.LastCommandTime = DateTime.Now;
            player.LastLoginTime = DateTime.Now;

            return player;
        }

        private void AddCharacterToCache(string hubId, Player character)
        {
             
            if (_cache.PlayerAlreadyExists(character.Id))
            {
                // log char off
                // remove from _cache
                // return
            }

            _cache.AddPlayer(hubId, character);

        }

        private void GetRoom(string hubId, Player character)
        {
           var room = _cache.GetRoom(1);

            new RoomActions(_writeToClient).Look(room, character);

          //  return room;
        }

        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }


    }
}
