using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;

namespace ArchaicQuestII.API
{
    public class Program
    {
        public static void Main(string[] args)
        {

            BuildWebHost(args).Run();

            Serilog.Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.File("log.txt")
                .CreateLogger();


            try
            {
                Serilog.Log.Information("Starting up ArchaicQuest");
                BuildWebHost(args).Run();
            }
            catch (Exception ex)
            {
                Serilog.Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Serilog.Log.CloseAndFlush();
            }
        }

        public static IWebHost BuildWebHost(string[] args)
        {

            var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("hosting.json")
            .AddCommandLine(args)
            .Build();

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var isDevelopment = environment == Environments.Development;

            if (isDevelopment)
            {
                return WebHost.CreateDefaultBuilder(args)
               .UseUrls("http://*:62640")
               .UseConfiguration(config)
               .UseContentRoot(Directory.GetCurrentDirectory())
               .UseStartup<Startup>()
               .Build();
            }



            return WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .UseUrls("http://*:62640")
                .UseConfiguration(config)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();

        }
    }
}
