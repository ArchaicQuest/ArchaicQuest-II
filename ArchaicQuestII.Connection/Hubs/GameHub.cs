
using Microsoft.AspNetCore.SignalR;
using System;
using System.IO;
using System.Threading.Tasks;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;
using Microsoft.Extensions.Logging;

namespace ArchaicQuestII.Connection
{
    public class GameHub : Hub
    {
        private readonly ILogger<GameHub> _logger;
        private IDataBase _db { get; }
        private ICache _cache { get; }
        public GameHub(IDataBase db, ICache cache, ILogger<GameHub> logger)
        {
            _logger = logger;
            _db = db;
            _cache = cache;
        }
        /// <summary>
        /// Do action when user connects 
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            // await Clients.All.SendAsync("SendAction", "user", "joined");
        }

        /// <summary>
        /// Do action when user disconnects 
        /// </summary>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception ex)
        {
           // await Clients.All.SendAsync("SendAction", "user", "left");
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

            var room = GetRoom(hubId, player);

            await SendToClient($"Welcome {player.Name}. Your adventure awaits you.", hubId);


            await SendToClient($"<h4>{room.Title}</h4><p>{room.Description}</p>", hubId);

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

        private Room GetRoom(string hubId, Player character)
        {
           var room = _cache.GetRoom(1);

            return room;
        }


    }
}
