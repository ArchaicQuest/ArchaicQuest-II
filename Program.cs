using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;


namespace ArchaicQuestII
{
    public class Program
    {

        public static void Main(string[] args)
        {

            var configuration = new ConfigurationBuilder().Build();

            Serilog.Log.Logger = new LoggerConfiguration()
               .WriteTo.File("archaicquest.log")
               .CreateLogger();

            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
