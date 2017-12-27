using Xmu.Crms.Shared.Service;
using Xmu.Crms.Services.SmartFive;

namespace Microsoft.Extensions.DependencyInjection

{

    public static class HotPotExtensions

    {

        public static IServiceCollection AddSmartFiveFixGroupService(this IServiceCollection serviceCollection) =>
            serviceCollection.AddScoped<IFixGroupService, FixGroupService>();

        public static IServiceCollection AddSmartFiveUserService(this IServiceCollection serviceCollection) =>
           serviceCollection.AddScoped<IUserService, UserService>();

    }

}