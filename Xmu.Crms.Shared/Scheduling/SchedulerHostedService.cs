using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Service;

namespace Xmu.Crms.Shared.Scheduling
{
    //Stolen from https://blog.maartenballiauw.be/post/2017/08/01/building-a-scheduled-cache-updater-in-aspnet-core-2.html
    public class SchedulerHostedService : HostedService
    {
        private static readonly TimeSpan _Interval = TimeSpan.FromMinutes(10);
        private readonly IServiceProvider _provider;
        public IList<CrmsEventAttribute> Events { get; }

        public SchedulerHostedService(IServiceProvider provider)
        {
            _provider = provider;
            Events = AppDomain.CurrentDomain.GetAssemblies()
                .Where(asm => !asm.IsDynamic)
                .SelectMany(asm => asm.ExportedTypes)
                .SelectMany(typ => typ.GetMethods()).SelectMany(m =>
                    m.GetCustomAttributes(typeof(CrmsEventAttribute), true).OfType<CrmsEventAttribute>()
                        .Select(e => e.SetCallback(m))).ToList();
        }

        public event EventHandler<UnobservedTaskExceptionEventArgs> UnobservedTaskException;

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await ExecuteOnceAsync(cancellationToken);
                await Task.Delay(_Interval, cancellationToken);
            }
        }

        private async Task ExecuteOnceAsync(CancellationToken cancellationToken)
        {
            using (var scope = _provider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<CrmsContext>();
                var timer = scope.ServiceProvider.GetService<ITimerService>();
                
                var taskFactory = new TaskFactory(TaskScheduler.Current);
                var task = taskFactory.StartNew(() => timer?.Scheduled(), cancellationToken);
                foreach (var eventAttribute in Events)
                {
                    var type = db.Model.FindEntityType(eventAttribute.Table.FullName);
                    var dbset =
                        (IQueryable<object>) 
                        db
                            .GetType()
                            .GetRuntimeProperties()
                            .FirstOrDefault(o => o.PropertyType.IsGenericType &&
                                                 o.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>) &&
                                                 o.PropertyType.GenericTypeArguments.Contains(type.ClrType))
                            ?.GetValue(db) ?? throw new Exception();
                    var realParams = 
                        dbset
                            .Where(e =>
                                TimeInInterval((DateTime) type.GetProperties()
                                    .Single(p => p.Name == eventAttribute.TimeColumn && p.ClrType == typeof(DateTime))
                                    .PropertyInfo.GetValue(e)))
                            .Where(e => eventAttribute.WhereColumns
                                .Select(col => col.StartsWith("!") ? Tuple.Create(col.Substring(1), true) : Tuple.Create(col, false))
                                .Select(ci =>  ci.Item2 ^ Convert.ToBoolean(type.GetProperties().Single(ppt => ppt.Name == ci.Item1).PropertyInfo.GetValue(e))).All(b => b))
                            .Select(e =>
                                eventAttribute.ParamColumns.Select(col => e.GetType().GetProperty(col).GetValue(e))
                                    .ToArray()).ToList();

                    foreach (var parameters in realParams)
                    {
                        var instance = scope.ServiceProvider.GetRequiredService(eventAttribute.Callback.DeclaringType);
                        await taskFactory.StartNew(
                            () =>
                            {
                                try
                                {
                                    eventAttribute.Callback.Invoke(instance, parameters);
                                }
                                catch (Exception ex)
                                {
                                    var args = new UnobservedTaskExceptionEventArgs(
                                        ex as AggregateException ?? new AggregateException(ex));

                                    UnobservedTaskException?.Invoke(this, args);

                                    if (!args.Observed)
                                    {
                                        throw;
                                    }
                                }
                            },
                            cancellationToken);
                    }
                }
                await task;
            }
        }

        private static bool TimeInInterval(DateTime time) => time > DateTime.Now && time < DateTime.Now + _Interval;
    }
}