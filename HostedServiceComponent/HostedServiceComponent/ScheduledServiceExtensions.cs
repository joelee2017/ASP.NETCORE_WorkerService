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

        //public static void AddJobAndTrigger<T>(this IServiceCollectionQuartzConfigurator quartz, IConfiguration config) where T : IJob
        //{
        //    // Use the name of the IJob as the appsettings.json key
        //    string jobName = typeof(T).Name;

        //    // Try and load the schedule from configuration
        //    var configKey = $"Quartz:{jobName}";
        //    var cronSchedule = config[configKey];

        //    // Some minor validation
        //    if (string.IsNullOrEmpty(cronSchedule))
        //    {
        //        throw new Exception($"No Quartz.NET Cron schedule found for job in configuration at {configKey}");
        //    }

        //    // register the job as before
        //    var jobKey = new JobKey(jobName);
        //    quartz.AddJob<T>(opts => opts.WithIdentity(jobKey));

        //    quartz.AddTrigger(opts => opts.ForJob(jobKey).WithIdentity(jobName + "-trigger").WithCronSchedule(cronSchedule)); 
        //    // use the schedule from configuration
        //}


        public static void AddJobAndTrigger<T>(this IServiceCollection services, IConfiguration config) where T : CronJobService
        {
            string jobName = typeof(T).Name;
            bool enbale = config.GetValue<bool>($"cronJob:{jobName}:Enable");
            string expression = config.GetValue<string>($"cronJob:{jobName}:CronExpression");
            string expression2 = config.GetSection($"cronJob:{jobName}:CronExpression").Value.ToString();
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
