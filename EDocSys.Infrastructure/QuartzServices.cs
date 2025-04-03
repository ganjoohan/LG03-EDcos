using System;
using EDocSys.Infrastructure.Jobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;


namespace EDocSys.Infrastructure
{
    public static class QuartzServices
    {
        public static IServiceCollection AddQuartzServices(this IServiceCollection services, IConfiguration configuration)
        {
            //quartz schedulerw
            services.AddQuartz(q =>
            {
                // Directly read job schedules from appsettings
                var reminderHour = configuration.GetValue<int>("EmailReminders:Hour");
                var reminderMinute = configuration.GetValue<int>("EmailReminders:Minute");


                var emailReminderJobKey = new JobKey(nameof(EmailReminderJob));
                q.AddJob<EmailReminderJob>(emailReminderJobKey)
                .AddTrigger(trigger =>
                {
                    trigger.ForJob(emailReminderJobKey)
                    .WithCronSchedule(CronScheduleBuilder.WeeklyOnDayAndHourAndMinute(DayOfWeek.Monday, reminderHour, reminderMinute));
                });


            });

            services.AddQuartzHostedService();

            return services;
        }
    }
}
