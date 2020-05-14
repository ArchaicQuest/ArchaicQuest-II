using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Formatting.Compact;

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


            return WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .UseUrls("http://*:62640")
                .UseConfiguration(config).Build();

        }
    }
}
