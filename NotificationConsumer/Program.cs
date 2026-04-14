using Microsoft.EntityFrameworkCore;
using NotificationConsumer.Services;
using NotificationConsumer.Workers;
using Shared.Data;
using NotificationConsumer.Services.Interfaces;

var builder = Host.CreateApplicationBuilder(args);

// SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration
        .GetConnectionString("DefaultConnection")));

// Services
// Register interfaces with implementations
builder.Services.AddSingleton<ITwilioWhatsAppService, TwilioWhatsAppService>();
builder.Services.AddScoped<INotificationSetupService, NotificationSetupService>();
builder.Services.AddScoped<INotificationLogService, NotificationLogService>();


// Background Worker
builder.Services.AddHostedService<KafkaConsumerWorker>();

var host = builder.Build();
host.Run();