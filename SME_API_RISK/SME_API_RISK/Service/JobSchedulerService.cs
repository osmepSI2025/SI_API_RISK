using Quartz;
using Quartz.Impl.Matchers;
using SME_API_RISK.Entities;
using Microsoft.EntityFrameworkCore;
public class JobSchedulerService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<JobSchedulerService> _logger;
    private readonly ISchedulerFactory _schedulerFactory;

    public JobSchedulerService(IServiceProvider serviceProvider, ILogger<JobSchedulerService> logger, ISchedulerFactory schedulerFactory)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _schedulerFactory = schedulerFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("JobSchedulerService is starting.");
        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<RISKDBContext>();
            var jobs = await dbContext.MScheduledJobs.Where(j => j.IsActive == true).ToListAsync(cancellationToken);

            // Clear all triggers in the "dynamic" group before scheduling
            var allScheduledJobKeys = await scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals("dynamic"));
            foreach (var key in allScheduledJobKeys)
            {
                var triggers = await scheduler.GetTriggersOfJob(key, cancellationToken);
                foreach (var trigger in triggers)
                {
                    await scheduler.UnscheduleJob(trigger.Key, cancellationToken);
                    _logger.LogInformation($"Trigger '{trigger.Key.Name}' for job '{key.Name}' deleted.");
                }
            }

            foreach (var job in jobs)
            {
                // แก้ไข: เพิ่มการตรวจสอบค่าว่างเปล่า (whitespace)
                if (!int.TryParse(job.RunMinute.ToString(), out _) || !int.TryParse(job.RunHour.ToString(), out _))
                {
                    _logger.LogError($"Job '{job.JobName}' has invalid RunMinute or RunHour. Skipping.");
                    continue;
                }
                string cron = $"0 {job.RunMinute} {job.RunHour} * * ?";
                var jobKey = new JobKey(job.JobName, "dynamic");

                // ตรวจสอบว่า Job มีอยู่แล้วหรือไม่
                if (await scheduler.CheckExists(jobKey, cancellationToken))
                {
                    _logger.LogInformation($"Job '{job.JobName}' already exists. Rescheduling with new trigger.");

                    var trigger = TriggerBuilder.Create()
                        .WithIdentity($"{job.JobName}-trigger", "dynamic")
                        .WithCronSchedule(cron)
                        .Build();

                    await scheduler.RescheduleJob(trigger.Key, trigger, cancellationToken);
                }
                else
                {
                    _logger.LogInformation($"Job '{job.JobName}' does not exist. Creating a new one.");

                    var jobDetail = JobBuilder.Create<ScheduledJobPuller>()
                        .WithIdentity(jobKey)
                        .UsingJobData("JobName", job.JobName)
                        .Build();

                    var trigger = TriggerBuilder.Create()
                        .WithIdentity($"{job.JobName}-trigger", "dynamic")
                        .WithCronSchedule(cron)
                        .Build();

                    await scheduler.ScheduleJob(jobDetail, trigger, cancellationToken);
                }
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("JobSchedulerService is stopping.");
        return Task.CompletedTask;
    }
}
