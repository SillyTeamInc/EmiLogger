using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace EmiLogger;

public static class EmiLoggingExtensions
{
    public static ILoggingBuilder AddEmiLogging(
        this ILoggingBuilder builder,
        LogLevel appMinimum = LogLevel.Debug,
        LogLevel aspNetMinimum = LogLevel.Warning)
    {
        Emi.MinLevel = appMinimum;
 
        builder.ClearProviders();
        builder.AddProvider(new EmiLoggerProvider());
 
        builder.AddFilter<EmiLoggerProvider>("Microsoft", aspNetMinimum);
        builder.AddFilter<EmiLoggerProvider>("System", aspNetMinimum);
 
        if (Debugger.IsAttached)
        {
            builder.AddDebug();
            builder.AddFilter<Microsoft.Extensions.Logging.Debug.DebugLoggerProvider>("Microsoft", LogLevel.Trace);
            builder.AddFilter<Microsoft.Extensions.Logging.Debug.DebugLoggerProvider>("System", LogLevel.Trace);
        }
 
        return builder;
    }
}

