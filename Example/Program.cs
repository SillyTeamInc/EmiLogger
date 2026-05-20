using System.Threading.RateLimiting;
using EmiLogger;
using Microsoft.AspNetCore.RateLimiting;
using Spectre.Console;

namespace Example;

class Program
{
    static void Main(string[] args)
    {
        Emi.ForceSetCapabilities();
        Emi.Debug("This is a debug message.");
        Emi.Info("This is an info message.");
        Emi.Warn("This is a warning message.");
        Emi.Error("This is an error message.");
        Emi.Critical("This is a critical message.");
        
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Logging.AddEmiLogging(LogLevel.Debug, LogLevel.Information);

        
        builder.Services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("default", o =>
            {
                o.PermitLimit = 60;
                o.Window = TimeSpan.FromSeconds(10);
                o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                o.QueueLimit = 0;
            });
        });

        var app = builder.Build();

        
        app.UseRateLimiter();
        app.UseWebSockets();
        app.MapControllers().RequireRateLimiting("default"); 
        app.Run("http://localhost:8081");
    }
}