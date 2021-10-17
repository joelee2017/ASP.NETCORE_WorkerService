using HostedServiceComponent.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace HostedServiceComponent
{
    public static class ScheduledServiceExtensions
    {
        // 使用方式  
        // services.AddCronJob<XXXCronJob>(c =>
        //   {
        //     c.TimeZoneInfo = TimeZoneInfo.Local;
        //     c.CronExpression = @"00 05 * * *";
        //   });
        public static IServiceCollection AddCronJob<T>(this IServiceCollection services, Action<IScheduleConfig<T>> options) where T : CronJobService
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options), @"Please provide Schedule Configurations.");
            }
            var config = new ScheduleConfig<T>();            

            options.Invoke(config);
            if (string.IsNullOrWhiteSpace(config.CronExpression))
            {
                throw new ArgumentNullException(nameof(ScheduleConfig<T>.CronExpression), @"Empty Cron Expression is not allowed.");
            }

            services.AddSingleton<IScheduleConfig<T>>(config);
            services.AddHostedService<T>();
            return services;
        }

        public static void AddJobAndTrigger<T>(this IServiceCollection services, IConfiguration config) where T : CronJobService
        {
            string jobName = typeof(T).Name;
            bool enbale = config.GetValue<bool>($"{jobName}:Enable");
            string expression = config.GetValue<string>($"{jobName}:CronExpression");
            string expression2 = config.GetSection($"{jobName}:CronExpression").Value.ToString();
            if (enbale)
            {
                services.AddCronJob<T>(s => {
                    s.TimeZoneInfo = TimeZoneInfo.Local;
                    s.CronExpression = expression;
                });

            }
        }
    }
}
