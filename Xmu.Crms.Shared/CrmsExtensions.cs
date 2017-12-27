using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xmu.Crms.Shared;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CrmsExtensions
    {
        public static IMvcBuilder AddApplicationParts(this IMvcBuilder builder, IEnumerable<Assembly> assemblies)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (assemblies == null)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }

            foreach (var assembly in assemblies)
            {
                builder.ConfigureApplicationPartManager(manager => manager.ApplicationParts.Add(new AssemblyPart(assembly)));
            }

            return builder;
        }


        public static IServiceCollection AddCrmsView(this IServiceCollection serviceCollection, string viewName)
        {
            var startConfig = serviceCollection.SingleOrDefault(s => s.ServiceType == typeof(CrmsStartupConfig))?.ImplementationInstance as CrmsStartupConfig ?? new CrmsStartupConfig();
            startConfig.ControllerAssemblies.Add(Assembly.Load("Xmu.Crms." + viewName));
            serviceCollection.TryAdd(new ServiceDescriptor(typeof(CrmsStartupConfig), startConfig));
            return serviceCollection;
        }

        public static IServiceCollection UseCrmsSqlite(this IServiceCollection serviceCollection, SqliteConnection sqlite)
        {
            var startConfig = serviceCollection.SingleOrDefault(s => s.ServiceType == typeof(CrmsStartupConfig))?.ImplementationInstance as CrmsStartupConfig ?? new CrmsStartupConfig();
            startConfig.SqliteConnection = sqlite;
            serviceCollection.TryAdd(new ServiceDescriptor(typeof(CrmsStartupConfig), startConfig));
            return serviceCollection;
        }
    }
}
