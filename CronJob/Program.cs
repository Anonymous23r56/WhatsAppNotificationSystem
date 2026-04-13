using CronJob.Jobs;
using CronJob.Services;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Shared.Data;

var builder = Host.CreateApplicationBuilder(args);

// SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration
        .GetConnectionString("DefaultConnection")));

// Twilio Service
builder.Services.AddSingleton<TwilioWhatsAppService>();

// Quartz Scheduler
builder.Services.AddQuartz(q =>
{
    var jobKey = new JobKey("RetryFailedNotificationsJob");

    q.AddJob<RetryFailedNotificationsJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("RetryFailedNotificationsJob-trigger")
        .WithCronSchedule(
            builder.Configuration["Quartz:CronSchedule"]!
        )
    );
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

// Register job for DI
builder.Services.AddScoped<RetryFailedNotificationsJob>();

var host = builder.Build();
host.Run();