using ArchaicQuestII.API.Helpers;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character.Alignment;
using ArchaicQuestII.GameLogic.Character.AttackTypes;
using ArchaicQuestII.GameLogic.Character.Class;
using ArchaicQuestII.GameLogic.Character.Race;
using ArchaicQuestII.GameLogic.Character.Status;
using LiteDB;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using ArchaicQuestII.GameLogic.Commands;
using ArchaicQuestII.GameLogic.Commands.Movement;
using ArchaicQuestII.GameLogic.Commands.Debug;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Hubs;
using ArchaicQuestII.GameLogic.World.Room;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using ArchaicQuestII.API.Entities;
using ArchaicQuestII.API.Services;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Emote;
using ArchaicQuestII.GameLogic.Character.Equipment;
using ArchaicQuestII.GameLogic.Character.Gain;
using ArchaicQuestII.GameLogic.Character.Help;
using ArchaicQuestII.GameLogic.Character.MobFunctions;
using ArchaicQuestII.GameLogic.Character.MobFunctions.Shop;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Combat;
using ArchaicQuestII.GameLogic.Commands.Communication;
using ArchaicQuestII.GameLogic.Commands.Inventory;
using ArchaicQuestII.GameLogic.Commands.Objects;
using ArchaicQuestII.GameLogic.Commands.Score;
using ArchaicQuestII.GameLogic.Commands.Skills;
using ArchaicQuestII.GameLogic.Crafting;
using ArchaicQuestII.GameLogic.Hubs.Telnet;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.Item.RandomItemTypes;
using ArchaicQuestII.GameLogic.Skill;
using ArchaicQuestII.GameLogic.Skill.Core;
using ArchaicQuestII.GameLogic.Skill.Model;
using ArchaicQuestII.GameLogic.Skill.Skills;
using ArchaicQuestII.GameLogic.Socials;
using ArchaicQuestII.GameLogic.Spell;
using ArchaicQuestII.GameLogic.Spell.Interface;
using ArchaicQuestII.GameLogic.Spell.Spells.DamageSpells;
using ArchaicQuestII.GameLogic.World.Area;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Newtonsoft.Json;
using Config = ArchaicQuestII.GameLogic.Core.Config;
using Damage = ArchaicQuestII.GameLogic.Core.Damage;
 
using Object = ArchaicQuestII.GameLogic.Commands.Objects.Object;
using SkillList = ArchaicQuestII.GameLogic.Character.Class.SkillList;
using ArchaicQuestII.GameLogic.Character.MobFunctions.Healer;

namespace ArchaicQuestII.API
{
    public class Startup
    {
        private IDataBase _db;
        private ICache _cache;
      
        private IHubContext<GameHub> _hubContext;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

    

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddCors(options =>
            {
                options.AddPolicy("client",
                    builder => builder.WithOrigins("http://localhost:4200", "http://localhost:1337", "https://admin.archaicquest.com", "https://play.archaicquest.com")
                        .AllowAnyMethod().AllowAnyHeader().AllowCredentials());
            });

            services.AddControllers().AddNewtonsoftJson();
            services.AddSignalR(o =>
            {
                o.EnableDetailedErrors = true;
            });


            // configure jwt authentication
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            // configure DI for application services
            services.AddScoped<IUserService, UserService>();
            services.AddSingleton<LiteDatabase>(
                new LiteDatabase(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AQ.db")));
            services.AddSingleton<LiteDatabase>(
                new LiteDatabase(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AQ-PLAYERS.db")));
            services.AddSingleton<IDataBase, DataBase>();
            services.AddSingleton<IPlayerDataBase, PlayerDataBase>();
            services.AddSingleton<ICache>(new Cache());
            services.AddSingleton<IDamage, Damage>();
            services.AddSingleton<IDice, Dice>();
            services.AddTransient<IMovement, Movement>();
            services.AddTransient<ISkills, Skills>();
            services.AddTransient<ISpells, CastSpell>();
            services.AddTransient<IDebug, Debug>();
            services.AddTransient<IInventory, Inventory>();
            services.AddSingleton<Icommunication, Communication>();
            services.AddTransient<IObject, Object>();
            services.AddTransient<IEquip, Equip>();
            services.AddSingleton<ICommands, Commands>();
            services.AddSingleton<IScore, Score>();
            services.AddTransient<ISpellTargetCharacter, SpellTargetCharacter>();
            services.AddSingleton<IGameLoop, GameLoop>();
            services.AddTransient<IRoomActions, RoomActions>();
            services.AddTransient<IAddRoom, AddRoom>();
            services.AddSingleton<ICombat, Combat>();
            services.AddSingleton<IGain, Gain>();
            services.AddSingleton<ISocials, Social>();
            services.AddSingleton<ICommandHandler, CommandHandler>();
            services.AddSingleton<IFormulas, Formulas>();
            services.AddSingleton<IUpdateClientUI, UpdateClientUI>();
            services.AddSingleton<IMobScripts, MobScripts>();
            services.AddSingleton<ITime, Time>();
            services.AddSingleton<ICore, GameLogic.Core.Core>();
            services.AddSingleton<IQuestLog, QuestLog>();
            services.AddSingleton<IMobFunctions, Shop>();
            services.AddSingleton<IHealer, Healer>();
            services.AddSingleton<IHelp, HelpFile>();
            services.AddSingleton<ICrafting, Crafting>();
            services.AddSingleton<ICooking, Cooking>();
            services.AddSingleton<ISkillManager, SkillManager>();
            services.AddSingleton<IDamageSpells, DamageSpells>();
            services.AddSingleton<IDamageSkills, DamageSkills>();
            services.AddSingleton<IPassiveSkills, PassiveSkills>();
            services.AddSingleton<IUtilSkills, UtilSkills>();
            services.AddSingleton<ISpellList, SpellList>();
            services.AddSingleton<ISkillList, GameLogic.Skill.SkillList>();
            services.AddSingleton<ISKill, DoSkill>();
            services.AddSingleton<IWeather, Weather>();
            services.AddSingleton<IRandomWeapon, RandomWeapons>();
            services.AddSingleton<IRandomItem, RandomItem>();
            services.AddSingleton<IRandomClothItems, RandomClothItems>();
            services.AddSingleton<IRandomLeatherItems, RandomLeatherItems>();
            services.AddSingleton<IRandomStuddedLeatherArmour, RandomStuddedLeatherItems>();
            services.AddSingleton<IRandomChainMailArmour, RandomChainMailItems>();
            services.AddSingleton<IRandomPlateMailArmour, RandomPlateMailItems>();
            services.AddSingleton<IWriteToClient, WriteToClient>((factory) => new WriteToClient(_hubContext, TelnetHub.Instance));
           
        }

        /// <summary>
        /// MOB SKILLS
        /// ----------
        /// I don't want to manually update mobs in each room
        /// if there is a change so this will run at startup to make changes
        /// to mobs. not a big deal if this takes while as it's a one time cost at startup
        ///
        /// This will mean we need special classes for mobs, can't have a shop keeper using skills
        /// that they should not know. Ok for an old mage in a magic shop casting spells at a player
        /// but can't have a mob selling socks drop kicking the player and going HAM with 2nd, 3rd attack
        /// and finishing off with a bash and a cleave to the skull. A tad unrealistic.
        ///
        /// Basic Mob class should be added with the essentials
        /// dodge, blunt, staves, and short blade skills
        /// probably others to add. maybe parry
        /// </summary>
        /// <param name="room"></param>
        public void AddSkillsToMobs(Room room)
        {
            foreach (var mob in room.Mobs)
            {

                mob.Skills = new List<SkillList>();
             
                var classSkill = _db.GetCollection<Class>(DataBase.Collections.Class).FindOne(x =>
                    x.Name.Equals(mob.ClassName, StringComparison.CurrentCultureIgnoreCase));

                foreach (var skill in classSkill.Skills)
                {
                    // skill doesn't exist and should be added
                    if (mob.Skills.FirstOrDefault(x =>
                        x.SkillName.Equals(skill.SkillName, StringComparison.CurrentCultureIgnoreCase)) == null)
                    {
                        mob.Skills.Add(
                            new SkillList()
                            {
                                Proficiency = 100,
                                Level = skill.Level,
                                SkillName = skill.SkillName,
                                SkillId = skill.SkillId
                            }
                        );
                    }

                    mob.Skills.FirstOrDefault(x => x.SkillName.Equals(skill.SkillName, StringComparison.CurrentCultureIgnoreCase)).SkillId = skill.SkillId;
                }

                //set mob armor
                mob.ArmorRating = new ArmourRating()
                {
                    Armour = mob.Level > 5 ? mob.Level * 3 : 1,
                    Magic = mob.Level * 3 / 4,
                };

                //give mob unique IDs
                mob.UniqueId = Guid.NewGuid();
            }
        }

        public void MapMobRoomId(Room room)
        {
            foreach (var mob in room.Mobs)
            {
                mob.UniqueId = Guid.NewGuid();
                mob.RoomId = $"{room.AreaId}{room.Coords.X}{room.Coords.Y}{room.Coords.Z}";
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDataBase db, ICache cache)
        {
            if (env.EnvironmentName == "dev")
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Play/Error");
            }
            _db = db;
            _cache = cache;

   
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseCors("client");
            app.UseMiddleware<JwtMiddleware>();

            // Forward headers for Ngnix

            app.UseForwardedHeaders(new ForwardedHeadersOptions
           {
               ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
           });

           app.UseRouting();
           app.UseAuthentication();
           app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<GameHub>("/Hubs/game");

            });

          
            _hubContext = app.ApplicationServices.GetService<IHubContext<GameHub>>();
            app.StartLoops();

            var watch = System.Diagnostics.Stopwatch.StartNew();

            var rooms = _db.GetList<Room>(DataBase.Collections.Room);

            foreach (var room in rooms)
            {
                AddSkillsToMobs(room);
                MapMobRoomId(room);
                _cache.AddRoom($"{room.AreaId}{room.Coords.X}{room.Coords.Y}{room.Coords.Z}", room);
                _cache.AddOriginalRoom($"{room.AreaId}{room.Coords.X}{room.Coords.Y}{room.Coords.Z}", JsonConvert.DeserializeObject<Room>(JsonConvert.SerializeObject(room)));
            }


            if (!_db.DoesCollectionExist(DataBase.Collections.Alignment))
            {
                foreach (var data in new Alignment().SeedData())
                {
                    _db.Save(data, DataBase.Collections.Alignment);
                }
            }

            if (!_db.DoesCollectionExist(DataBase.Collections.Skill))
            {
                foreach (var data in new SeedCoreSkills().SeedData())
                {
                    _db.Save(data, DataBase.Collections.Skill);
                }

            }
            else
            {
                var currentSkills = _db.GetList<Skill>(DataBase.Collections.Skill);
                foreach (var data in new SeedCoreSkills().SeedData())
                {
                    if (currentSkills.FirstOrDefault(x => x.Name == data.Name) == null)
                    {
                        _db.Save(data, DataBase.Collections.Skill);
                    }
                }
            }

            if (!_db.DoesCollectionExist(DataBase.Collections.AttackType))
            {
                foreach (var data in new AttackTypes().SeedData())
                {
                    _db.Save(data, DataBase.Collections.AttackType);
                }
            }

            if (!_db.DoesCollectionExist(DataBase.Collections.Race))
            {
                foreach (var data in new Race().SeedData())
                {
                    _db.Save(data, DataBase.Collections.Race);
                }
            }

            if (!_db.DoesCollectionExist(DataBase.Collections.Status))
            {
                foreach (var data in new CharacterStatus().SeedData())
                {
                    _db.Save(data, DataBase.Collections.Status);
                }
            }

            if (!_db.DoesCollectionExist(DataBase.Collections.Class))
            {
                foreach (var data in new Class().SeedData())
                {
                    _db.Save(data, DataBase.Collections.Class);

                }
            }

            var classes = _db.GetList<Class>(DataBase.Collections.Class);

            foreach (var pcClass in classes)
            {
                _cache.AddClass(pcClass.Name, pcClass);
            }



            if (!_db.DoesCollectionExist(DataBase.Collections.Config))
            {
                _db.Save(new Config(), DataBase.Collections.Config);
            }

            var config = _db.GetById<Config>(1, DataBase.Collections.Config);
            _cache.SetConfig(config);

            //add skills
            var skills = new SeedCoreSkills().SeedData();

                foreach (var skill in skills)
            {
                 skill.Id = skills.Count > 0 ? skills.Max(x => x.Id) + 1 : 1;
                _cache.AddSkill(skill.Id, skill);
            }

                // update player skills id

            var quests = _db.GetList<Quest>(DataBase.Collections.Quests);

            foreach (var quest in quests)
            {
                _cache.AddQuest(quest.Id, quest);
            }

            var areas = _db.GetList<Area>(DataBase.Collections.Area);
           
            //foreach (var area in areas)
            //{
            //    var roomList = rooms.FindAll(x => x.AreaId == area.Id);
            //    _cache.AddMap(area.Id, Map.DrawMap(roomList));
            //}

            foreach (var area in areas)
            {
                var roomList = rooms.FindAll(x => x.AreaId == area.Id);
                var areaByZIndex = roomList.FindAll(x => x.Coords.Z != 0).Distinct();
                foreach (var zarea in areaByZIndex)
                {
                    var roomsByZ = new List<Room>();
                    foreach (var room in roomList.FindAll(x => x.Coords.Z == zarea.Coords.Z))
                    {
                        roomsByZ.Add(room);
                    }

                    _cache.AddMap($"{area.Id}{zarea.Coords.Z}", Map.DrawMap(roomsByZ));
                }

                var rooms0index = roomList.FindAll(x => x.Coords.Z == 0);
                _cache.AddMap($"{area.Id}0", Map.DrawMap(rooms0index));
            }

            var socials = new SocialSeedData().SeedData();

            foreach (var social in socials)
            {
                _cache.AddSocial(social.Key, social.Value);
            }

            
            if (!_db.DoesCollectionExist(DataBase.Collections.Socials))
            {
                foreach (var social in socials)
                {
                    _db.Save(social, DataBase.Collections.Socials);
                }
            }


            if (!_db.DoesCollectionExist(DataBase.Collections.Items)) {
                foreach (var itemSeed in new ItemSeed().SeedData())
                {
                    _db.Save(itemSeed, DataBase.Collections.Items);
                }
            }
            else
            {
                var hasMoney = _db.GetList<Item>(DataBase.Collections.Items)
                    .FirstOrDefault(x => x.ItemType == Item.ItemTypes.Money);


                if (hasMoney == null)
                {
                    foreach (var itemSeed in new ItemSeed().SeedData())
                    {
                        _db.Save(itemSeed, DataBase.Collections.Items);
                    }
                }
            }

            if (!_db.DoesCollectionExist(DataBase.Collections.Help))
            {
                foreach (var seed in new HelpSeed().SeedData())
                {
                    _db.Save(seed, DataBase.Collections.Help);
                }
            }
            else
            {
                var helpList = _db.GetList<Help>(DataBase.Collections.Help);


                foreach (var seed in new HelpSeed().SeedData())
                {
                    if (helpList.FirstOrDefault(x => x.Title.Equals(seed.Title)) != null)
                    {
                        continue;
                    }

                    _db.Save(seed, DataBase.Collections.Help);
                }
            }
            var helpFiles = _db.GetList<Help>(DataBase.Collections.Help);
            foreach (var helpFile in helpFiles)
            {
                _cache.AddHelp(helpFile.Id, helpFile);
            }

            var craftingRecipes = _db.GetList<CraftingRecipes>(DataBase.Collections.CraftingRecipes);
            foreach (var craftingRecipe in craftingRecipes)
            {
                _cache.AddCraftingRecipes(craftingRecipe.Id, craftingRecipe);
            }

            if (!_db.DoesCollectionExist(DataBase.Collections.Users))
            {

                var admin = new AdminUser() {Username = "Admin", Password = "admin", Role = "Admin", CanEdit = true, CanDelete = true};
               
                    _db.Save(admin, DataBase.Collections.Users);
                
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            Console.WriteLine($"Start up completed in {elapsedMs}");
           GameLogic.Core.Helpers.PostToDiscord($"Start up completed in {Math.Ceiling((decimal)elapsedMs / 1000)} seconds", "event", _cache.GetConfig());
        }
    }

    public static class Loops
    {
        public static void StartLoops(this IApplicationBuilder app)
        {
            var loop = app.ApplicationServices.GetRequiredService<IGameLoop>();
           
            Task.Run(TelnetHub.Instance.ProcessConnections);
            Task.Run(loop.UpdateTime);
            Task.Run(loop.UpdateCombat);
            Task.Run(loop.UpdatePlayers);
            Task.Run(loop.UpdatePlayerLag);
            Task.Run(loop.UpdateRoomEmote).ConfigureAwait(false);
            Task.Run(loop.UpdateMobEmote).ConfigureAwait(false);
            Task.Run(loop.UpdateWorldTime).ConfigureAwait(false);
            Task.Run(loop.Tick);
        }
    }

}
