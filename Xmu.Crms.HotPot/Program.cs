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
                .UseStartup<Xmu.Crms.Shared.Startup>()
                .ConfigureServices(services => 
                {
                    services
                    //HotPot小组
                    .AddHotPotClassService()
                    .AddHotPotGradeService()
                    .AddHotPotLoginService()
                    .AddHotPotUploadService()
                    .AddCrmsView("Mobile.HotPot")

                    //Group1_7
                    .AddGroup1_7SeminarService()
                    .AddGroup1_7SchoolService()
                    .AddGroup1_7CourseService()

                    //Group2_10
                    .AddGroup2_10SeminarGroupService()
                    .AddGroup2_10TopicService()

                    //SmartFive
                    .AddSmartFiveFixGroupService()
                    .AddSmartFiveUserService()

                    //Insomnia
                    .AddCrmsView("Web.Insomnia")
                    .AddCrmsView("API.Insomnia")
                    .AddInsomniaUserService()
                    .AddInsomniaTopicService()
                    .AddInsomniaSeminarGroupService()
                    .AddInsomniaPbkdf2LoginService()
                    .AddInsomniaLoginService()
                    .AddInsomniaFixedGroupService();
                   

                })              
                .Build(); 
    }
}
