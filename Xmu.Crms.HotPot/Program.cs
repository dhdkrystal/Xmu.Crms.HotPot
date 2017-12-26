using System.Reflection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

using Xmu.Crms.Shared;


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
                .ConfigureServices(services => services.AddHotPotClassService())
                .UseStartup<Startup>()
                .Build(); 
    }
}
