using HostedServiceComponent;
using HostedServiceComponent.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestCronJobService
{
    public class Test1CronJobService : CronJobService
    {
        private readonly ILogger<Test1CronJobService> _logger;

        public Test1CronJobService(IScheduleConfig<Test1CronJobService> config, ILogger<Test1CronJobService> logger)
        : base(config.CronExpression, config.TimeZoneInfo)
        {
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("CronJob  starts.");
            return base.StartAsync(cancellationToken);
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} CronJob  is working.");
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("CronJob  is stopping.");
            return base.StopAsync(cancellationToken);
        }
    }
}
