using ArchaicQuestII.API.Configuration.IoC.GameLogicExtensions;
using ArchaicQuestII.API.Entities;
using ArchaicQuestII.API.Helpers;
using ArchaicQuestII.API.Services;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Hubs;
using ArchaicQuestII.GameLogic.Hubs.Telnet;
using ArchaicQuestII.GameLogic.SeedData;
using LiteDB;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

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
                    builder => builder
                            .WithOrigins(
                                "http://localhost:4200",
                                "http://localhost:1337",
                                "https://admin.archaicquest.com",
                                "https://play.archaicquest.com")
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials());
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
            services.AddSingleton(
                new LiteDatabase(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AQ.db")));

            services.AddSingleton<IDataBase, DataBase>();
            services.AddSingleton<IPlayerDataBase>(new PlayerDataBase(new LiteDatabase(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AQ-PLAYERS.db"))));

            services.AddSingleton<IWriteToClient, WriteToClient>((factory) =>
                new WriteToClient(_hubContext, TelnetHub.Instance));

            services.AddGameLogic();
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

            SeedData.SeedAndCache(_db, _cache);

            if (!db.DoesCollectionExist(DataBase.Collections.Users))
            {
                var admin = new AdminUser()
                {
                    Username = "Admin",
                    Password = "admin",
                    Role = "Admin",
                    CanEdit = true,
                    CanDelete = true
                };

                db.Save(admin, DataBase.Collections.Users);
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            Console.WriteLine($"Start up completed in {elapsedMs}");
            GameLogic.Core.Helpers.PostToDiscord($"Start up completed in {Math.Ceiling((decimal)elapsedMs / 1000)} seconds", "event", _cache.GetConfig());
        }
    }
}
