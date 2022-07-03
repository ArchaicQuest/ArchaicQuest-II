
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Class;
using ArchaicQuestII.GameLogic.Character.Equipment;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.SeedData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ArchaicQuestII.Controllers.character
{

    [ApiController]
    public class PlayerController : ControllerBase
    {
        private IDataBase _db { get; }
        private IPlayerDataBase _pdb { get; }
        private ICache _cache { get; }
        public PlayerController(IDataBase db, ICache cache, IPlayerDataBase pdb)
        {
            _db = db;
            _pdb = pdb;
            _cache = cache;
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("api/character/Player")]
        public ObjectResult Post([FromBody] Player player)
        {
            if (!ModelState.IsValid)
            {
                var exception = new Exception("Invalid player");
                throw exception;
            }

            var playerClass = _db.GetList<Class>(DataBase.Collections.Class).FirstOrDefault(x => x.Name.Equals(player.ClassName));

            var newPlayer = new Player()
            {
                AccountId = player.AccountId,
                Id = Guid.NewGuid(),
                Name = player.Name,
                Status = CharacterStatus.Status.Standing,
                Level = 1,
                ArmorRating = new ArmourRating()
                {
                    Armour = 1,
                    Magic = 1
                },
                Affects = new Affects(),
                AlignmentScore = 0,
                Attributes = player.Attributes,
                MaxAttributes = player.Attributes,
                Inventory = new ItemList(),
                Equipped = new Equipment(),
                ClassName = player.ClassName,
                Config = null,
                Description = player.Description,
                Gender = player.Gender,
                Stats = new Stats()
                {
                    HitPoints = player.Attributes.Attribute[GameLogic.Effect.EffectLocation.Constitution] * 2, //create formula to handle these stats
                    MovePoints = player.Attributes.Attribute[GameLogic.Effect.EffectLocation.Dexterity] * 2,  // only for testing
                    ManaPoints = player.Attributes.Attribute[GameLogic.Effect.EffectLocation.Intelligence] * 2,
                },
                MaxStats = player.Stats,
                Money = new GameLogic.Character.Model.Money()
                {
                    Gold = 10,
                    Silver = 0,
                },
                Bank = new GameLogic.Character.Model.Money()
                {
                    Gold = 10,
                    Silver = 0,
                },
                Race = player.Race,
                JoinedDate = DateTime.Now,
                LastLoginTime = DateTime.Now,
                Skills = new List<SkillList>(),
                Roam = player.Roam,
                Build = player.Build,
                Face = player.Face,
                Skin = player.Skin,
                Eyes = player.Eyes,
                FacialHair = player.FacialHair,
                HairColour = player.HairColour,
                HairLength = player.HairLength,
                HairTexture = player.HairTexture,
                RoomId = _cache.GetConfig().StartingRoom,
            };

            var ItemSeed = Items.seedData;
            var Light = ItemSeed.FirstOrDefault(x => x.Name.Equals("The torch of illuminatio"));
            Light.Equipped = true;
            newPlayer.Inventory.Add(Light);
            newPlayer.Equipped.Light = Light;

            var shirt = ItemSeed.FirstOrDefault(x => x.Name.Equals("A ragged shirt"));
            var robe = ItemSeed.FirstOrDefault(x => x.Name.Equals("A simple cloth robe"));
            var sleeves = ItemSeed.FirstOrDefault(x => x.Name.Equals("A pair of baggy sleeves"));
            var trousers = ItemSeed.FirstOrDefault(x => x.Name.Equals("A pair of baggy trousers"));
            var boots = ItemSeed.FirstOrDefault(x => x.Name.Equals("A pair of worn leather boots"));
            var dagger = ItemSeed.FirstOrDefault(x => x.Name.Equals("A simple iron dagger"));
            var sword = ItemSeed.FirstOrDefault(x => x.Name.Equals("A simple iron sword"));
            var mace = ItemSeed.FirstOrDefault(x => x.Name.Equals("A simple iron mace"));

            shirt.Equipped = true;
            newPlayer.Inventory.Add(shirt);
            newPlayer.Equipped.Torso = shirt;

            sleeves.Equipped = true;
            newPlayer.Inventory.Add(sleeves);
            newPlayer.Equipped.Arms = sleeves;

            trousers.Equipped = true;
            newPlayer.Inventory.Add(trousers);
            newPlayer.Equipped.Legs = trousers;

            boots.Equipped = true;
            newPlayer.Inventory.Add(boots);
            newPlayer.Equipped.Feet = boots;


            if (newPlayer.ClassName.Equals("Mage"))
            {
                newPlayer.Inventory.Remove(shirt);

                robe.Equipped = true;
                newPlayer.Inventory.Add(robe);
                newPlayer.Equipped.Torso = robe;

                dagger.Equipped = true;
                newPlayer.Inventory.Add(dagger);
                newPlayer.Equipped.Wielded = dagger;

            }

            if (newPlayer.ClassName.Equals("Thief"))
            {
                dagger.Equipped = true;
                newPlayer.Inventory.Add(dagger);
                newPlayer.Equipped.Wielded = dagger;
            }

            if (newPlayer.ClassName.Equals("Fighter"))
            {
                sword.Equipped = true;
                newPlayer.Inventory.Add(sword);
                newPlayer.Equipped.Wielded = sword;
            }

            if (newPlayer.ClassName.Equals("Cleric"))
            {
                mace.Equipped = true;
                newPlayer.Inventory.Add(mace);
                newPlayer.Equipped.Wielded = mace;
            }


            newPlayer.Skills = playerClass?.Skills ?? new List<SkillList>();

            ArchaicQuestII.GameLogic.SeedData.Classes.SetGenericTitle(newPlayer);


            if (!string.IsNullOrEmpty(player.Id.ToString()) && player.Id != Guid.Empty)
            {

                var foundItem = _pdb.GetById<Character>(player.Id, PlayerDataBase.Collections.Players);

                if (foundItem == null)
                {
                    throw new Exception("player Id does not exist");
                }

                newPlayer.Id = player.Id;
            }

            var account = _pdb.GetById<Account>(player.AccountId, PlayerDataBase.Collections.Account);
            account.Characters.Add(newPlayer.Id);
            Helpers.PostToDiscord($"{player.Name} has joined the realms for the first time.", "event", _cache.GetConfig());


            var dupeCheck = _pdb.GetCollection<Player>(PlayerDataBase.Collections.Players).FindOne(x => x.Name.Equals(newPlayer.Name));

            if (dupeCheck != null)
            {

                if (dupeCheck.Id != newPlayer.Id)
                {
                    return Ok(newPlayer.Id);
                }

            }



            _pdb.Save(account, PlayerDataBase.Collections.Account);
            _pdb.Save(newPlayer, PlayerDataBase.Collections.Players);

            return Ok(newPlayer.Id);

        }



        //[HttpGet]
        //[Route("api/mob/FindMobById")]
        //public Player FindMobById([FromQuery] Guid id)
        //{


        [HttpGet]
        [AllowAnonymous]
        [Route("api/player/NameAllowed")]
        public bool NameAllowed([FromQuery] string name)
        {

            var nameExists = _pdb.GetCollection<Player>(PlayerDataBase.Collections.Players).FindOne(x => x.Name == name);

            if (nameExists == null)
            {
                return true;
            }

            return false;

        }


        [HttpGet]
        [API.Helpers.Authorize]
        [Route("api/character/Player/{id:guid}")]
        public List<Player> Get(Guid? id)
        {

            var pc = _pdb.GetCollection<Account>(PlayerDataBase.Collections.Account).FindById(id);

            if (id == null)
            {
                return _pdb.GetCollection<Player>(PlayerDataBase.Collections.Players).FindAll().ToList();
            }

            var players = new List<Player>();

            foreach (var character in pc.Characters)
            {
                var foundPC = _pdb.GetCollection<Player>(PlayerDataBase.Collections.Players).FindById(character);

                if (foundPC != null)
                {
                    players.Add(foundPC);
                }
            }

            return players.OrderByDescending(x => x.LastLoginTime).ToList();

        }

        [HttpGet]
        [API.Helpers.Authorize]
        [Route("api/character/viewPlayer/{id:guid}")]
        public Player GetPlayer(Guid? id)
        {

            var pc = _pdb.GetCollection<Player>(PlayerDataBase.Collections.Players).FindById(id);

            return pc;

        }

        [HttpGet]
        [API.Helpers.Authorize]
        [Route("api/character/accounts")]
        public List<Account> getAccounts([FromQuery] string query)
        {

            var account = _pdb.GetCollection<Account>(PlayerDataBase.Collections.Account).FindAll().Where(x => x.Id != null).OrderByDescending(x => x.DateLastPlayed);

            if (string.IsNullOrEmpty(query))
            {
                return account.ToList();
            }

            return account.ToList();

        }





    }
}
