using Microsoft.EntityFrameworkCore;
using NotificationConsumer.Services;
using NotificationConsumer.Workers;
using Shared.Data;

var builder = Host.CreateApplicationBuilder(args);

// SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration
        .GetConnectionString("DefaultConnection")));

// Services
builder.Services.AddSingleton<TwilioWhatsAppService>();

// Background Worker
builder.Services.AddHostedService<KafkaConsumerWorker>();

var host = builder.Build();
host.Run();