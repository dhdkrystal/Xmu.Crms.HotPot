using Xmu.Crms.Services.Group1_7;
using Xmu.Crms.Shared.Service;

namespace Microsoft.Extensions.DependencyInjection
{ 
    public static class Group1_7Extensions
    {
        // 为每一个你写的Service写一个这样的函数，把 UserService 替换为你实现的 Service
        public static IServiceCollection AddGroup1_7SeminarService(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddScoped<ISeminarService, SeminarService>();
        }

        public static IServiceCollection AddGroup1_7SchoolService(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddScoped<ISchoolService, SchoolService>();
        }

        public static IServiceCollection AddGroup1_7CourseService(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddScoped<ICourseService, CourseService>();
        }
    }
}
