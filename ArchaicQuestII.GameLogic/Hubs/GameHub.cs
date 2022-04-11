
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Class;
using ArchaicQuestII.GameLogic.Character.Gain;
using ArchaicQuestII.GameLogic.Combat;
using ArchaicQuestII.GameLogic.Commands;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Spell;
using ArchaicQuestII.GameLogic.World.Room;
using Microsoft.Extensions.Logging;
using MoonSharp.Interpreter;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace ArchaicQuestII.GameLogic.Hubs
{
    public class GameHub : Hub
    {
        private readonly ILogger<GameHub> _logger;
        private IDataBase _db { get; }
        private ICache _cache { get; }
        private readonly IWriteToClient _writeToClient;
        private readonly ICommands _commands;
        private readonly IUpdateClientUI _updateClientUi;
        private readonly IMobScripts _mobScripts;
        private readonly ITime _time;
        private readonly IDice _dice;
        private readonly IGain _gain;
        private readonly IFormulas _formulas;

        public GameHub(IDataBase db, ICache cache, ILogger<GameHub> logger, IWriteToClient writeToClient, ICommands commands, IUpdateClientUI updateClientUi, IMobScripts mobScripts, ITime time, IDice dice, IGain gain, IFormulas formulas)
        {
            _logger = logger;
            _db = db;
            _cache = cache;
            _writeToClient = writeToClient;
            _commands = commands;
            _updateClientUi = updateClientUi;
            _mobScripts = mobScripts;
            _time = time;
            _dice = dice;
            _gain = gain;
            _formulas = formulas;
        }

 
        /// <summary>
        /// Do action when user connects 
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("SendMessage", "", "<p>Someone has entered the realm.</p>");
        }

        /// <summary>
        /// Do action when user disconnects 
        /// </summary>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception ex)
        {
            await Clients.All.SendAsync("SendMessage", "", "<p>Someone has left the realm.</p>");
        }

 

    /// <summary>
    /// get message from client
    /// </summary>
    /// <returns></returns>
    public void SendToServer(string message, string connectionId)
        {
           
            var player = _cache.GetPlayer(connectionId);

            if (player == null)
            {
                _writeToClient.WriteLine("<p>Refresh the page to reconnect!</p>");
                return;
            }
            player.Buffer.Enqueue(message);
          
        }

        /// <summary>
        /// get content from client
        /// </summary>
        /// <returns></returns>
        public void CharContent(string message, string connectionId)
        {

            var player = _cache.GetPlayer(connectionId);

            if (player == null)
            {
                _writeToClient.WriteLine("<p>Refresh the page to reconnect!</p>", player.ConnectionId);
                return;
            }

            JObject json = JsonConvert.DeserializeObject<dynamic>(message);

            var contentType = json.GetValue("type")?.ToString();

          if(contentType == "book") {

                var book = new WriteBook()
                {
                    Description = json.GetValue("desc")?.ToString(),
                    PageNumber = int.Parse(json.GetValue("pageNumber")?.ToString()),
                    Title = json.GetValue("name")?.ToString()
                };

                var getBook = player.Inventory.FirstOrDefault(x => x.Name.Equals(book.Title));

                if(getBook == null)
                {
                    _writeToClient.WriteLine("<p>There's a puff of smoke and all your work is undone. Seek an Immortal</p>", player.ConnectionId);
                    return;
                }

                getBook.Book.Pages[book.PageNumber] = book.Description;
                _writeToClient.WriteLine("You have successfully written in your book.", player.ConnectionId);

            }

            if (contentType == "description")
            {

                player.Description = json.GetValue("desc")?.ToString();
                _writeToClient.WriteLine("You have successfully updated your description.", player.ConnectionId);
                _updateClientUi.UpdateScore(player);

            }






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

        public async Task CloseConnection(string message, string hubId)
        {
            await Clients.Client(hubId).SendAsync("Close", message);
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
            UpdatePlayerSkills(player);

         
                AddCharacterToCache(hubId, player);

            var playerExist = _cache.PlayerAlreadyExists(player.Id);
            player.LastLoginTime = DateTime.Now;
            player.LastCommandTime = DateTime.Now;
            
            player.CommandLog.Add($"{string.Format("{0:f}", DateTime.Now)} - Logged in"));

            await SendToClient($"<p>Welcome {player.Name}. Your adventure awaits you.</p>", hubId);

            if (playerExist != null)
            {
                Helpers.PostToDiscord($"{player.Name} has entered the realms.", "event", _cache.GetConfig());
                GetRoom(hubId, playerExist, playerExist.RoomId);
            }
            else
            {
                GetRoom(hubId, player);
            }

            
        }


        public void UpdatePlayerSkills(Player player)
        {
            var classSkill = _db.GetCollection<Class>(DataBase.Collections.Class).FindOne(x =>
                x.Name.Equals(player.ClassName, StringComparison.CurrentCultureIgnoreCase));

            foreach (var skill in classSkill.Skills)
            {
                var theSkill = _cache.GetAllSkills().FirstOrDefault(x => x.Name.Equals(skill.SkillName));
                // skill doesn't exist and should be added
                if (player.Skills.FirstOrDefault(x =>
                    x.SkillName.Equals(skill.SkillName, StringComparison.CurrentCultureIgnoreCase)) == null)
                {


                    var addSkill = new SkillList()
                    {
                        Proficiency = 1,
                        Level = skill.Level,
                        SkillName = skill.SkillName,
                        SkillId = _cache.GetSkill(skill.SkillId) == null ? theSkill.Id : skill.SkillId,
                        IsSpell = false
                    };

                    if(theSkill.Cost.Table.ContainsKey(GameLogic.Skill.Enum.Cost.Mana)) {
                    skill.IsSpell = _cache.GetSkill(skill.SkillId) == null ? theSkill.Cost.Table[GameLogic.Skill.Enum.Cost.Mana] > 0 ? true : false : _cache.GetSkill(skill.SkillId).Cost.Table[GameLogic.Skill.Enum.Cost.Mana] > 0 ? true : false;
                    };

                    player.Skills.Add( addSkill );


                }
            }

            for (var i = player.Skills.Count - 1; i >= 0; i--)
            {
                if (classSkill.Skills.FirstOrDefault(x =>
                    x.SkillName.Equals(player.Skills[i].SkillName, StringComparison.CurrentCultureIgnoreCase)) == null)
                {
                    player.Skills.Remove(player.Skills[i]);
                }

                var skill = classSkill.Skills.FirstOrDefault(x => x.SkillName.Equals(player.Skills[i].SkillName));


                player.Skills[i].Level = skill.Level;
            }
 

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
            
            SetArmorRating(player);
          
            return player;
        }

        private void SetArmorRating(Player player)
        {
            foreach (var eq in player.Inventory.FindAll(x => x.Equipped.Equals(true)))
            {
                player.ArmorRating.Armour += eq.ArmourRating.Armour;
                player.ArmorRating.Magic += eq.ArmourRating.Magic;
            }
        }

        private async void AddCharacterToCache(string hubId, Player character)
        {
            var playerExist = _cache.PlayerAlreadyExists(character.Id);


            if (playerExist != null)
            {
                // log char off
                await SendToClient($"<p style='color:red'>*** You have logged in elsewhere. ***</p>", playerExist.ConnectionId);
                await this.CloseConnection(string.Empty, playerExist.ConnectionId);
             
                // remove from _cache
                _cache.RemovePlayer(playerExist.ConnectionId);

                playerExist.ConnectionId = hubId;
                _cache.AddPlayer(hubId, playerExist);

                await SendToClient($"<p style='color:red'>*** You have been reconnected ***</p>", hubId);

              
            }
            else
            {
                _cache.AddPlayer(hubId, character);
            }
            

          

        }

        private void GetRoom(string hubId, Player character, string startingRoom = "")
        {
            //add to DB, configure from admin
              var roomid = !string.IsNullOrEmpty(startingRoom) ? startingRoom : _cache.GetConfig().StartingRoom;
            var room = _cache.GetRoom(roomid);
           character.RoomId = $"{room.AreaId}{room.Coords.X}{room.Coords.Y}{room.Coords.Z}";

           if (string.IsNullOrEmpty(character.RecallId))
           {
               var defaultRoom = _cache.GetConfig().StartingRoom;
               character.RecallId = defaultRoom;
            }

           var playerAlreadyInRoom = room.Players.ToList().FirstOrDefault(x => x.Id.Equals(character.Id)) != null;
           if (!playerAlreadyInRoom)
           {
               room.Players.Add(character);
               if(character.Pets.Any())
                {
                    foreach (var pet in character.Pets)
                    {
                        var petAlreadyInRoom = room.Mobs.FirstOrDefault(x => x.Id.Equals(pet.Id)) != null;

                        if (!petAlreadyInRoom)
                        {
                            room.Mobs.Add(pet);
                        }
                    }
                   
                }
           }


           var rooms = _cache.GetAllRoomsInArea(1);

           _updateClientUi.UpdateHP(character);
            _updateClientUi.UpdateMana(character);
            _updateClientUi.UpdateMoves(character);
            _updateClientUi.UpdateExp(character);
            _updateClientUi.UpdateEquipment(character);
            _updateClientUi.UpdateInventory(character);
            _updateClientUi.UpdateScore(character);
            _updateClientUi.UpdateQuest(character);
            _updateClientUi.UpdateAffects(character);
            _updateClientUi.UpdateTime(character);

            _updateClientUi.GetMap(character,_cache.GetMap($"{room.AreaId}{room.Coords.Z}"));

            new RoomActions(_writeToClient, _time, _cache, _dice, _gain, _formulas).Look("", room, character);

            foreach (var mob in room.Mobs)
            {
                if (!string.IsNullOrEmpty(mob.Events.Enter))
                {
                    UserData.RegisterType<MobScripts>();

                    Script script = new Script();

                    DynValue obj = UserData.Create(_mobScripts);
                    script.Globals.Set("obj", obj);
                    UserData.RegisterProxyType<MyProxy, Room>(r => new MyProxy(room));
                    UserData.RegisterProxyType<ProxyPlayer, Player>(r => new ProxyPlayer(character));


                    script.Globals["room"] = room;

                    script.Globals["player"] = character;
                    script.Globals["mob"] = mob;


                    DynValue res = script.DoString(mob.Events.Enter);
                }
            }



            //  return room;
        }

        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }


    }
}
