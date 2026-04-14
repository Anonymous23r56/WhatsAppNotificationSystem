//using Microsoft.EntityFrameworkCore;
//using NotificationApi.Services;
//using NotificationApi.Services.Interfaces;
//using Shared.Data;

//var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new() { Title = "WhatsApp Notification API", Version = "v1" });
//});

//// SQL Server
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer(builder.Configuration
//        .GetConnectionString("DefaultConnection")));

//// Services
////builder.Services.AddSingleton<KafkaProducerService>();
////builder.Services.AddScoped<AccountService>();
////builder.Services.AddScoped<BankTransactionService>();
//// Register services with their interfaces
//builder.Services.AddSingleton<IKafkaProducerService, KafkaProducerService>();
//builder.Services.AddScoped<IAccountService, AccountService>();
//builder.Services.AddScoped<IBankTransactionService, BankTransactionService>();

//var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI(c =>
//    {
//        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WhatsApp Notification API V1");
//        c.RoutePrefix = "swagger";
//    });
//}

//app.UseAuthorization();
//app.MapControllers();
//app.Run();

using Microsoft.EntityFrameworkCore;
using NotificationApi.Services;
using NotificationApi.Services.Interfaces;
using Shared.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "WhatsApp Notification API", Version = "v1" });
});

// SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration
        .GetConnectionString("DefaultConnection")));

// Register interfaces with implementations
builder.Services.AddSingleton<IKafkaProducerService, KafkaProducerService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IBankTransactionService, BankTransactionService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WhatsApp Notification API V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseAuthorization();
app.MapControllers();
app.Run();
