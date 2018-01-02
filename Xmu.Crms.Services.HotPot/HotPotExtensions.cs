using Xmu.Crms.Services.HotPot;
using Xmu.Crms.Shared.Service;



namespace Microsoft.Extensions.DependencyInjection

{

    public static class HotPotExtensions

    {

        public static IServiceCollection AddHotPotClassService(this IServiceCollection serviceCollection) =>
            serviceCollection.AddScoped<IClassService, ClassService>();

        public static IServiceCollection AddHotPotGradeService(this IServiceCollection serviceCollection) =>
           serviceCollection.AddScoped<IGradeService, GradeService>();

        public static IServiceCollection AddHotPotLoginService(this IServiceCollection serviceCollection) =>
           serviceCollection.AddScoped<ILoginService, LoginService>();

        public static IServiceCollection AddHotPotUploadService(this IServiceCollection serviceCollection) =>
          serviceCollection.AddScoped<IUploadService, UploadService>();

    }

}