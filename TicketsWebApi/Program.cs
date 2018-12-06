using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TicketsWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(LoadConfig(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")))
                .UseStartup<Startup>()
                .UseUrls("http://localhost:4000");


        private static IConfiguration LoadConfig(string environment) =>
            new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.{environment}.json", true, true)
                .AddJsonFile("SmtpSettings.json")
                .Build();
    }
}