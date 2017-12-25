using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Xmu.Crms.Services.HotPot;
using Xmu.Crms.Shared;
using Xmu.Crms.Shared.Models;


namespace Xmu.Crms.HotPot
{
    
        public class Program

        {

            public static void Main(string[] args)
        {
            var host = BuildWebHost(args);

            host.Run();

        }


        public static IWebHost BuildWebHost(string[] args) =>

        WebHost.CreateDefaultBuilder(args)

                .UseIISIntegration()

                .UseStartup<Startup>()

                .ConfigureServices(services => services.AddHotPotClassService())

                .Build();



       
    }
}
