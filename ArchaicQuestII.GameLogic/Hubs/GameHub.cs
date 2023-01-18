using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Class;
using ArchaicQuestII.GameLogic.Character.Config;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Utilities;
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

        private readonly ICoreHandler _coreHandler;

        public GameHub(
            ILogger<GameHub> logger, 
            ICoreHandler coreHandler
            )
        {
            _logger = logger;
            _coreHandler = coreHandler;
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
            var player = _coreHandler.Character.GetPlayer(connectionId);

            if (player == null)
            {
                _coreHandler.Client.WriteLine("<p>Refresh the page to reconnect!</p>");
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
            var player = _coreHandler.Character.GetPlayer(connectionId);

            if (player == null)
            {
                _coreHandler.Client.WriteLine("<p>Refresh the page to reconnect!</p>", player.ConnectionId);
                return;
            }

            JObject json = JsonConvert.DeserializeObject<dynamic>(message);

            var contentType = json.GetValue("type")?.ToString();

            if (contentType == "book")
            {
                var book = new WriteBook()
                {
                    Description = json.GetValue("desc")?.ToString(),
                    PageNumber = int.Parse(json.GetValue("pageNumber")?.ToString()),
                    Title = json.GetValue("name")?.ToString()
                };

                var getBook = player.Inventory.FirstOrDefault(x => x.Name.Equals(book.Title));

                if (getBook == null)
                {
                    _coreHandler.Client.WriteLine("<p>There's a puff of smoke and all your work is undone. Seek an Immortal</p>", player.ConnectionId);
                    return;
                }

                getBook.Book.Pages[book.PageNumber] = book.Description;

                player.OpenedBook = getBook;
                _coreHandler.Client.WriteLine("You have successfully written in your book.", player.ConnectionId);
            }

            if (contentType == "description")
            {
                player.Description = json.GetValue("desc")?.ToString();
                _coreHandler.Client.WriteLine("You have successfully updated your description.", player.ConnectionId);
                _coreHandler.Client.UpdateScore(player);
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
            // no longer used for ArchaicQuest but if you want to load an ascii MOTD uncomment 
            //var location = System.Reflection.Assembly.GetEntryAssembly().Location;
            //var directory = System.IO.Path.GetDirectoryName(location);
            //var motd = File.ReadAllText(directory + "/motd");

            var motd = "<img src='/assets/images/aqdragon.png' alt='ArchaicQuest Logo' class='motd' />";
            await SendToClient(motd, id);
        }
        
        public void CreateCharacter(string name = "Liam")
        {
            var newPlayer = new Player
            {
                Name = name
            };
            
            _coreHandler.Pdb.Save(newPlayer, PlayerDataBase.Collections.Players);
        }

        public async void AddCharacter(string hubId, Guid characterId)
        {
            var player = GetCharacter(hubId, characterId);
            UpdatePlayerSkills(player);
            player.Config ??= new PlayerConfig();

            foreach (var quest in player.QuestLog)
            {
                var updatedQuest = _coreHandler.Character.GetQuest(quest.Id);
                quest.Type = updatedQuest.Type;
                quest.ItemsToGet = updatedQuest.ItemsToGet;
                quest.MobsToKill = updatedQuest.MobsToKill;
                quest.Title = updatedQuest.Title;
                quest.Description = updatedQuest.Description;
                quest.ExpGain = updatedQuest.ExpGain;
                quest.GoldGain = updatedQuest.GoldGain;
                quest.ItemGain = updatedQuest.ItemGain;
                quest.Area = updatedQuest.Area;
            }

            AddCharacterToCache(hubId, player);

            var playerExist = _coreHandler.Character.PlayerAlreadyExists(player.Id);
            player.LastLoginTime = DateTime.Now;
            player.LastCommandTime = DateTime.Now;

            player.CommandLog.Add($"{DateTime.Now:f} - Logged in");
            var pcAccount = _coreHandler.Pdb.GetById<Account.Account>(player.AccountId, PlayerDataBase.Collections.Account);
            if (pcAccount != null)
            {
                pcAccount.DateLastPlayed = DateTime.Now;
                _coreHandler.Pdb.Save(pcAccount, PlayerDataBase.Collections.Account);
                var loginStats = new Account.AccountLoginStats
                {
                    AccountId = player.AccountId,
                    loginDate = DateTime.Now
                };

                _coreHandler.Pdb.Save(loginStats, PlayerDataBase.Collections.LoginStats);
            }

            await SendToClient($"<p>Welcome {player.Name}. Your adventure awaits you.</p>", hubId);

            if (playerExist != null)
            {
                if(_coreHandler.Config.PostToDiscord)
                    Helpers.PostToDiscord($"{player.Name} has entered the realms.",
                        _coreHandler.Config.EventsDiscordWebHookURL);
                
                GetRoom(hubId, playerExist, playerExist.RoomId);
            }
            else
            {
                GetRoom(hubId, player);
            }
        }

        public void UpdatePlayerSkills(Player player)
        {
            var classSkill = _coreHandler.Db.GetCollection<Class>(DataBase.Collections.Class).FindOne(x =>
                x.Name.Equals(player.ClassName, StringComparison.CurrentCultureIgnoreCase));

             foreach (var skill in classSkill.Skills)
            {
                var theSkill = _coreHandler.Command.GetAllSkills().FirstOrDefault(x => x.Name.Equals(skill.SkillName, StringComparison.CurrentCultureIgnoreCase));
                
                // skill doesn't exist and should be added
                if (player.Skills.FirstOrDefault(x =>
                    x.SkillName.Equals(skill.SkillName, StringComparison.CurrentCultureIgnoreCase)) == null)
                {
                    
                    var addSkill = new SkillList
                    {
                        Proficiency = 1,
                        Level = skill.Level,
                        SkillName = skill.SkillName,
                        SkillId = _coreHandler.Command.GetSkill(skill.SkillId) == null ? theSkill.Id : skill.SkillId,
                        IsSpell = false
                    };

                    if (theSkill.Cost.Table.ContainsKey(Skill.Enum.Cost.Mana))
                    {
                        skill.IsSpell = _coreHandler.Command.GetSkill(skill.SkillId) == null ? theSkill.Cost.Table[Skill.Enum.Cost.Mana] > 0 : _coreHandler.Command.GetSkill(skill.SkillId).Cost.Table[Skill.Enum.Cost.Mana] > 0 ? true : false;
                    }

                    player.Skills.Add(addSkill);
                }
            }

            if (player.Skills.Count > 0)
            {
                for (var i = player.Skills.Count - 1; i >= 0; i--)
                {
                    if (classSkill.Skills.FirstOrDefault(x =>
                            x.SkillName.Equals(player.Skills[i].SkillName,
                                StringComparison.CurrentCultureIgnoreCase)) == null)
                    {
                        player.Skills.Remove(player.Skills[i]);
                    }

                    var skill = classSkill.Skills.FirstOrDefault(x => x.SkillName.Equals(player.Skills[i].SkillName));
                    
                    player.Skills[i].Level = skill.Level;
                }
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
            var player = _coreHandler.Pdb.GetById<Player>(characterId, PlayerDataBase.Collections.Players);
            player.ConnectionId = hubId;
            player.LastCommandTime = DateTime.Now;
            player.LastLoginTime = DateTime.Now;
            player.Following = string.Empty;
            player.Followers = new List<Player>();
            player.Grouped = false;
            
            SetArmorRating(player);

            if (player.Attributes.Attribute[EffectLocation.Hitpoints] <= 0)
            {
                player.Attributes.Attribute[EffectLocation.Hitpoints] =
                    player.MaxAttributes.Attribute[EffectLocation.Hitpoints] / 4;
            }

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
            var playerExist = _coreHandler.Character.PlayerAlreadyExists(character.Id);
            
            if (playerExist != null)
            {
                // log char off
                await SendToClient($"<p style='color:red'>*** You have logged in elsewhere. ***</p>", playerExist.ConnectionId);
                await this.CloseConnection(string.Empty, playerExist.ConnectionId);

                // remove from _cache
                _coreHandler.Character.RemovePlayer(playerExist.ConnectionId);

                playerExist.ConnectionId = hubId;
                _coreHandler.Character.AddPlayer(hubId, playerExist);

                await SendToClient($"<p style='color:red'>*** You have been reconnected ***</p>", hubId);
            }
            else
            {
                _coreHandler.Character.AddPlayer(hubId, character);
            }
        }

        private void GetRoom(string hubId, Player character, string startingRoom = "")
        {
            //add to DB, configure from admin
            var roomid = !string.IsNullOrEmpty(startingRoom) ? startingRoom : _coreHandler.Config.StartingRoom;
            var room = _coreHandler.World.GetRoom(roomid) ?? _coreHandler.World.GetRoom(_coreHandler.Config.StartingRoom);
            character.RoomId = $"{room.AreaId}{room.Coords.X}{room.Coords.Y}{room.Coords.Z}";

            if (string.IsNullOrEmpty(character.RecallId))
            {
                var defaultRoom = _coreHandler.Config.StartingRoom;
                character.RecallId = defaultRoom;
            }

            var playerAlreadyInRoom = room.Players.ToList().FirstOrDefault(x => x.Id.Equals(character.Id)) != null;
            if (!playerAlreadyInRoom)
            {
                room.Players.Add(character);
                if (character.Pets.Any())
                {
                    foreach (var pet in from pet in character.Pets let petAlreadyInRoom = room.Mobs.FirstOrDefault(x => x.Id.Equals(pet.Id)) != null where !petAlreadyInRoom select pet)
                    {
                        room.Mobs.Add(pet);
                        _coreHandler.Client.WriteToOthersInRoom($"{pet.Name} suddenly appears.", room, character);
                    }
                }
                
                _coreHandler.Client.WriteToOthersInRoom($"{character.Name} suddenly appears.", room, character);
            }
            
            var rooms = _coreHandler.World.GetAllRoomsInArea(1);

            _coreHandler.Client.UpdateHP(character);
            _coreHandler.Client.UpdateMana(character);
            _coreHandler.Client.UpdateMoves(character);
            _coreHandler.Client.UpdateExp(character);
            _coreHandler.Client.UpdateEquipment(character);
            _coreHandler.Client.UpdateInventory(character);
            _coreHandler.Client.UpdateScore(character);
            _coreHandler.Client.UpdateQuest(character);
            _coreHandler.Client.UpdateAffects(character);
            _coreHandler.Client.UpdateTime(character, _coreHandler.World.Time);
            _coreHandler.Client.GetMap(character, _coreHandler.World.GetMap($"{room.AreaId}{room.Coords.Z}"));
            
            character.Buffer.Enqueue("look");

            foreach (var mob in room.Mobs.ToList().Where(mob => !string.IsNullOrEmpty(mob.Events.Enter)))
            {
                UserData.RegisterType<MobScripts>();

                var script = new Script();

                var obj = UserData.Create(_coreHandler.Character.MobScripts);
                script.Globals.Set("obj", obj);
                UserData.RegisterProxyType<MyProxy, Room>(r => new MyProxy(room));
                UserData.RegisterProxyType<ProxyPlayer, Player>(r => new ProxyPlayer(character));
                
                script.Globals["room"] = room;
                script.Globals["player"] = character;
                script.Globals["mob"] = mob;
                script.DoString(mob.Events.Enter);
            }
            
            //  return room;
        }

        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }
    }
}
