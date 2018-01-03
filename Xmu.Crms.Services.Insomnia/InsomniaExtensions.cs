using Xmu.Crms.Services.Insomnia;
using Xmu.Crms.Shared.Service;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class InsomniaExtensions
    {
        public static IServiceCollection AddInsomniaSeminarGroupService(this IServiceCollection serviceCollection) =>
            serviceCollection.AddScoped<ISeminarGroupService, GroupService>();

        public static IServiceCollection AddInsomniaFixedGroupService(this IServiceCollection serviceCollection) =>
            serviceCollection.AddScoped<IFixGroupService, FixedGroupService>();

        public static IServiceCollection AddInsomniaPbkdf2LoginService(this IServiceCollection serviceCollection) =>
            serviceCollection.AddScoped<ILoginService, Pbkdf2LoginService>();

        public static IServiceCollection AddInsomniaLoginService(this IServiceCollection serviceCollection) =>
            serviceCollection.AddScoped<ILoginService, Md5LoginService>();

        public static IServiceCollection AddInsomniaUserService(this IServiceCollection serviceCollection) =>
            serviceCollection.AddScoped<IUserService, UserService>();

        public static IServiceCollection AddInsomniaTopicService(this IServiceCollection serviceCollection) =>
    serviceCollection.AddScoped<ITopicService, TopicService>();
    }
}