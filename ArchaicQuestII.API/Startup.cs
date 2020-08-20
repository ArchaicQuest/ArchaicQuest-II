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
using static ArchaicQuestII.API.Services.services;
using System.Threading.Tasks;
using ArchaicQuestII.GameLogic.Character.Equipment;
using ArchaicQuestII.GameLogic.Combat;
using ArchaicQuestII.GameLogic.Commands.Communication;
using ArchaicQuestII.GameLogic.Commands.Inventory;
using ArchaicQuestII.GameLogic.Commands.Objects;
using ArchaicQuestII.GameLogic.Commands.Score;
using ArchaicQuestII.GameLogic.Commands.Skills;
using ArchaicQuestII.GameLogic.Skill.Model;
using ArchaicQuestII.GameLogic.Spell;
using ArchaicQuestII.GameLogic.Spell.Interface;
using ArchaicQuestII.GameLogic.World.Area;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Object = ArchaicQuestII.GameLogic.Commands.Objects.Object;

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
           
       //     services.AddMvc();
            services.AddControllers().AddNewtonsoftJson();
            services.AddSignalR(o =>
            {
                o.EnableDetailedErrors = true;
            });
            services.AddCors(options =>
            {
                options.AddPolicy("client",
                    builder => builder.WithOrigins("http://localhost:4200", "http://localhost:1337", "https://admin.archaicquest.com", "https://play.archaicquest.com")
                        .AllowAnyMethod().AllowAnyHeader().AllowCredentials());
           
            });

          

            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            // configure DI for application services
            services.AddScoped<IAdminUserService, AdminUserService>();
            services.AddSingleton<LiteDatabase>(
                new LiteDatabase(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AQ.db")));
            services.AddScoped<IDataBase, DataBase>();
            services.AddSingleton<ICache>(new Cache());
            services.AddSingleton<IDamage, Damage>();

            services.AddTransient<IMovement, Movement>();
            services.AddTransient<ISkills, Skills>();
            services.AddTransient<ISpells, Spells>();
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
            services.AddSingleton<IFormulas, Formulas>();
            services.AddSingleton<IUpdateClientUI, UpdateClientUI>();
            services.AddSingleton<IWriteToClient, WriteToClient>((factory) => new WriteToClient(_hubContext));

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
          
        
            app.UseAuthentication();

           // Forward headers for Ngnix

           app.UseForwardedHeaders(new ForwardedHeadersOptions
           {
               ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
           });

           app.UseRouting();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<GameHub>("/Hubs/game");
              
            });

          
            _hubContext = app.ApplicationServices.GetService<IHubContext<GameHub>>();
            app.StartLoops();

            var rooms = _db.GetList<Room>(DataBase.Collections.Room);

            foreach (var room in rooms)
            {
                _cache.AddRoom(room.Id, room);
            }


            if (!_db.DoesCollectionExist(DataBase.Collections.Alignment))
            {
                foreach (var data in new Alignment().SeedData())
                {
                    _db.Save(data, DataBase.Collections.Alignment);
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

            if (!_db.DoesCollectionExist(DataBase.Collections.Config))
            {
                _db.Save(new Config(), DataBase.Collections.Config);
            }

            var config = _db.GetById<Config>(1, DataBase.Collections.Config);
            _cache.SetConfig(config);

            //add skills
            var skills = _db.GetList<Skill>(DataBase.Collections.Skill);

            foreach (var skill in skills)
            {
                _cache.AddSkill(skill.Id, skill);
            }

            var areas = _db.GetList<Area>(DataBase.Collections.Area);
           
            foreach (var area in areas)
            {
                var roomList = rooms.FindAll(x => x.AreaId == area.Id);
                _cache.AddMap(area.Id, Map.DrawMap(roomList));
            }


        }
    }

    public static class Loops
    {
        public static void StartLoops(this IApplicationBuilder app)
        {
            var loop = app.ApplicationServices.GetRequiredService<IGameLoop>();
            Task.Run(loop.UpdateTime);
            Task.Run(loop.UpdatePlayers);
        }
    }

}
