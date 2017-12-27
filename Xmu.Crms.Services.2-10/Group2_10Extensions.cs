using Xmu.Crms.Services.Group2_10;
using Xmu.Crms.Shared.Service;

namespace Microsoft.Extensions.DependencyInjection
{ 
    public static class Group1Extensions
    {
        // 为每一个你写的Service写一个这样的函数，把 UserService 替换为你实现的 Service
        public static IServiceCollection AddGroup2_10TopicService(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddScoped<ITopicService, TopicService>();
        }
        public static IServiceCollection AddGroup2_10SeminarGroupService(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddScoped<ISeminarGroupService, SeminarGroupService>();
        }
    }
}
