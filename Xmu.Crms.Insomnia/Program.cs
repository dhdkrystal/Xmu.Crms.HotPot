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
using Xmu.Crms.Services.Insomnia;
using Xmu.Crms.Shared;
using Xmu.Crms.Shared1;
using Xmu.Crms.Shared.Models;

namespace Xmu.Crms.Insomnia
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = BuildWebHost(args);
            //using (var scope = host.Services.CreateScope())
            //{
            //    var services = scope.ServiceProvider;
            //    var conf = services.GetRequiredService<IConfiguration>();
            //    var db = services.GetService<CrmsContext>();

            //    if (Convert.ToBoolean(conf["Database:EnsureCreated"]))
            //    {
            //        await db.Database.EnsureCreatedAsync();
            //    }

            //    if (Convert.ToBoolean(conf["Database:Migrate"]))
            //    {
            //        await db.Database.MigrateAsync();
            //    }

            //    if (Convert.ToBoolean(conf["Database:InsertStub"]))
            //    {
            //        var school = await db.School.AddAsync(new School
            //        {
            //            City = "厦门",
            //            Name = "厦门市人民公园",
            //            Province = "福建"
            //        });

            //        await db.SaveChangesAsync();

            //        await db.UserInfo.AddAsync(new UserInfo
            //        {
            //            Avatar = "/upload/avatar/Logo_Li.png",
            //            Email = "t@t.test",
            //            Gender = Gender.Male,
            //            Name = "张三",
            //            Number = "123456",
            //            Password = PasswordUtils.HashString("123"),
            //            Phone = "1234",
            //            School = await db.School.FindAsync(school.Entity.Id),
            //            Title = Title.Other
            //        });

            //        await db.UserInfo.AddAsync(new UserInfo
            //        {
            //            Avatar = "/upload/avatar/Logo_Li.png",
            //            Email = "t2@t.test",
            //            Gender = Gender.Female,
            //            Name = "李四",
            //            Number = "134254",
            //            Password = PasswordUtils.HashString("456"),
            //            Phone = "123",
            //            School = await db.School.FindAsync(school.Entity.Id),
            //            Title = Title.Professer
            //        });

            //        await db.SaveChangesAsync();
            //    }

            //    if (Convert.ToBoolean(conf["Database:Check"]))
            //    {
            //        var checkSetting = new JsonSerializerSettings();
            //        checkSetting.Converters.Add(new StringEnumConverter());
            //        checkSetting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            //        Debug.WriteLine(JsonConvert.SerializeObject(await db.Attendences.SingleOrDefaultAsync(a => a.Id == 1), checkSetting));
            //        Debug.WriteLine(JsonConvert.SerializeObject(await db.ClassInfo.SingleOrDefaultAsync(a => a.Id == 1), checkSetting));
            //        Debug.WriteLine(JsonConvert.SerializeObject(await db.Course.SingleOrDefaultAsync(a => a.Id == 1), checkSetting));
            //        Debug.WriteLine(JsonConvert.SerializeObject(await db.CourseSelection.SingleOrDefaultAsync(a => a.Id == 1), checkSetting));
            //        Debug.WriteLine(JsonConvert.SerializeObject(await db.FixGroup.SingleOrDefaultAsync(a => a.Id == 1), checkSetting));
            //        Debug.WriteLine(JsonConvert.SerializeObject(await db.FixGroupMember.SingleOrDefaultAsync(a => a.Id == 1), checkSetting));
            //        Debug.WriteLine(JsonConvert.SerializeObject(await db.Location.SingleOrDefaultAsync(a => a.Id == 1), checkSetting));
            //        Debug.WriteLine(JsonConvert.SerializeObject(await db.Seminar.SingleOrDefaultAsync(a => a.Id == 1), checkSetting));
            //        Debug.WriteLine(JsonConvert.SerializeObject(await db.SeminarGroup.SingleOrDefaultAsync(a => a.Id == 1), checkSetting));
            //        Debug.WriteLine(JsonConvert.SerializeObject(await db.SeminarGroupMember.SingleOrDefaultAsync(a => a.Id == 1), checkSetting));
            //        Debug.WriteLine(JsonConvert.SerializeObject(await db.SeminarGroupTopic.SingleOrDefaultAsync(a => a.Id == 1), checkSetting));
            //        Debug.WriteLine(JsonConvert.SerializeObject(await db.StudentScoreGroup.SingleOrDefaultAsync(a => a.Id == 1), checkSetting));
            //        Debug.WriteLine(JsonConvert.SerializeObject(await db.Topic.SingleOrDefaultAsync(a => a.Id == 1), checkSetting));
            //        Debug.WriteLine(JsonConvert.SerializeObject(await db.UserInfo.SingleOrDefaultAsync(a => a.Id == 1), checkSetting));
            //        Debug.WriteLine(JsonConvert.SerializeObject(await db.UserInfo.SingleOrDefaultAsync(a => a.Id == 3), checkSetting));
            //    }
            //}

            host.Run();
        }


        public static IWebHost BuildWebHost(string[] args) =>
            CreateWebHostBuilder(args)
                .Build();

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseIISIntegration()
                .ConfigureServices(collection =>
                {
                    collection
                        .AddInsomniaSeminarGroupService()
                        .AddInsomniaFixedGroupService()
                        .AddInsomniaLoginService()
                        .AddInsomniaTopicService()
                        .AddInsomniaUserService()
                        .AddCrmsView("API.Insomnia")
                        .AddCrmsView("Mobile.HighGrade")
                        .AddCrmsView("Web.Insomnia")

                        .AddViceVersaClassDao()
                        .AddViceVersaClassService()
                        .AddViceVersaCourseDao()
                        .AddViceVersaCourseService()
                        .AddViceVersaGradeDao()
                        .AddViceVersaGradeService()

                        .AddHighGradeSchoolService()
                        .AddHighGradeSeminarService()

                        .aAddSmartFiveClassService()
                        .aAddSmartFiveCourseService()
                        .aAddSmartFiveFixGroupService()
                        .aAddSmartFiveGradeService()
                        .aAddSmartFiveLoginService()
                        .aAddSmartFiveSchoolService()
                        .aAddSmartFiveSeminarGroupService()
                        .aAddSmartFiveSeminarService()
                        .aAddSmartFiveTimerService()
                        .aAddSmartFiveTopicService()
                        .aAddSmartFiveUserService()


                        ;
                })
                .UseStartup<Shared.Startup>();
                //.UseStartup<Shared1.Startup>();
        }
    }
}