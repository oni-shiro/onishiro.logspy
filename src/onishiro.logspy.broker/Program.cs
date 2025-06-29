using Onishiro.LogSpy.Broker.Configurations;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddSignalR();
builder.Services.ConfigureDependencies(config);
builder.Services.AddControllers();
builder.Services.AddCors(builder =>
{
    builder.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


var app = builder.Build();
app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();
// For Testing purposes, you can add a simple endpoint to verify the broker is running
app.MapGet("/", () => "LogSpy Broker is running!");

// Map SignalR hubs
app.MapHub<BrokerHub>("/logspy");
app.Run();

