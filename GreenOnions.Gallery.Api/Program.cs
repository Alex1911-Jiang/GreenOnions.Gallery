using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace GreenOnions.Gallery.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddCommandLine(args)
                   .Build();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
