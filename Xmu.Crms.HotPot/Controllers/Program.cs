using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Xmu.Crms.HotPot.Controllers
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Startup.ControllerAssembly.Add(Assembly.GetEntryAssembly());
            Startup.ControllerAssembly.Add(Assembly.GetAssembly(typeof(Xmu.Crms.Web.Group1.Program)));

            Startup.ConfigureCrmsServices += collection => collection.AddGroup1UserService();

            var host = BuildWebHost(args);

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var conf = services.GetRequiredService<IConfiguration>();
                var db = services.GetService<CrmsContext>();

                if (Convert.ToBoolean(conf["Database:EnsureCreated"]))
                {
                    await db.Database.EnsureCreatedAsync();
                }

                if (Convert.ToBoolean(conf["Database:Migrate"]))
                {
                    await db.Database.MigrateAsync();
                }

                if (Convert.ToBoolean(conf["Database:InsertStub"]))
                {
                    var school = await db.Schools.AddAsync(new School
                    {
                        City = "厦门",
                        Name = "厦门市人民公园",
                        Province = "福建"
                    });

                    await db.SaveChangesAsync();

                    await db.UserInfos.AddAsync(new UserInfo
                    {
                        Avatar = "/upload/avatar/Logo_Li.png",
                        Email = "t@t.test",
                        Gender = 0,
                        Name = "张三",
                        Number = "123456",
                        Password = PasswordUtils.HashString("123"),
                        Phone = "1234",
                        School = await db.Schools.FindAsync(school.Entity.Id),
                        Title = 1
                    });

                    await db.UserInfos.AddAsync(new UserInfo
                    {
                        Avatar = "/upload/avatar/Logo_Li.png",
                        Email = "t2@t.test",
                        Gender = 1,
                        Name = "李四",
                        Number = "134254",
                        Password = PasswordUtils.HashString("456"),
                        Phone = "123",
                        School = await db.Schools.FindAsync(school.Entity.Id),
                        Title = 1
                    });

                    await db.SaveChangesAsync();
                }
            }

            host.Run();
        }


        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();
    }
}