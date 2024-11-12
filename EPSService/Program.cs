using ESPService;
using ESPService.Data;
using ESPService.Hubs;
using ESPService.Repository;
using ESPService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddSingleton<ICodeGenerator, CodeGeneratorService>();
builder.Services.AddScoped<IDiscountCodeRepository, DiscountCodeRepository>();
builder.Services.AddTransient<IWebsocketNotificationService, SignalRNotificationService>();
builder.Services.AddSignalR();
builder.Services.AddDbContext<AppDbContext>(config =>
{
    config.UseSqlite(builder.Configuration.GetConnectionString("Default"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapHub<CodesHub>("/CodesHub");

app.MapGrpcService<DiscountCodeGeneratorService>();
app.InitDatabase();
app.Run();
