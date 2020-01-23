using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character.Alignment;
using ArchaicQuestII.GameLogic.Character.AttackTypes;
using ArchaicQuestII.GameLogic.Character.Class;
using ArchaicQuestII.GameLogic.Character.Race;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.Hubs;
using LiteDB;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace ArchaicQuestII.API
{
    public class Startup
    {
        private IDataBase _db;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
           // services.AddSingleton<ILog>(new Log.Log());
            services.AddSingleton<LiteDatabase>(
                new LiteDatabase(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AQ.db")));
            services.AddScoped<IDataBase, DataBase>();
            services.AddMvc();
            services.AddSignalR(o =>
            {
                o.EnableDetailedErrors = true;
            });
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IDataBase db)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Play/Error");
            }
            _db = db;
            app.UseStaticFiles();

            app.UseCors(
                options => options.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader().AllowCredentials()
            );

            app.UseMvc(routes =>
            {

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Play}/{action=Index}/{id?}");

            });


            app.UseSignalR(routes =>
            {
                routes.MapHub<GameHub>("/Hubs/game");
            });
 
            foreach (var data in new Alignment().SeedData())
            {
                _db.Save(data, DataBase.Collections.Alignment);
            }

            foreach (var data in new AttackTypes().SeedData())
            {
                _db.Save(data, DataBase.Collections.AttackType);
            }

            foreach (var data in new Race().SeedData())
            {
                _db.Save(data, DataBase.Collections.Race);
            }

            foreach (var data in new CharacterStatus().SeedData())
            {
                _db.Save(data, DataBase.Collections.Status);
            }

            foreach (var data in new Class().SeedData())
            {
                _db.Save(data, DataBase.Collections.Status);
            }

        }
    }

}
