using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace EmiLogger;

public static class Emi
{

    public static LogLevel MinLevel { get; set; } = LogLevel.Debug;

    private static readonly Dictionary<LogLevel, (string Label, string Color)> Levels = new()
    {
        { LogLevel.Trace, ("trace", "grey50") },
        { LogLevel.Debug, ("debug", "grey85") },
        { LogLevel.Information, ("info", "dodgerblue2") },
        { LogLevel.Warning, ("warn", "yellow3") },
        { LogLevel.Error, ("error", "red") },
        { LogLevel.Critical, ("fatal", "red on white") },
    };

    // should be used if you want to force ANSI to work
    public static void ForceSetCapabilities()
    {
        AnsiConsole.Profile.Capabilities.ColorSystem = ColorSystem.TrueColor;
        AnsiConsole.Profile.Capabilities.Ansi = true;
        AnsiConsole.Profile.Width = 512;
    }

    internal static void LogInternal(LogLevel level, string caption, string message, Exception? exception = null)
    {
        if (level < MinLevel) return;
        if (!Levels.TryGetValue(level, out var meta)) return;

        string time = DateTimeOffset.Now.ToString("HH:mm:ss");

        AnsiConsole.MarkupLine(
            $"[[[darkorange]{time}[/]]] " +
            $"[[[{meta.Color}]{meta.Label}[/]]] " +
            $"[steelblue1_1]{Markup.Escape(caption)}:[/] " +
            $"{Markup.Escape(message)}");

        if (exception != null)
            AnsiConsole.WriteException(exception, ExceptionFormats.ShortenPaths);
    }

    private static string CallerCaption(int skip = 2)
    {
        var frame = new StackTrace(true).GetFrames()
            .Skip(skip)
            .FirstOrDefault(f => f.GetMethod()?.DeclaringType != typeof(Emi));
        var method = frame?.GetMethod();
        return method != null ? $"{method.DeclaringType?.Name}.{method.Name}" : "Unknown";
    }

    public static void Log(string caption, string message, LogLevel level = LogLevel.Information)
        => LogInternal(level, caption, message);

    public static void Log(string caption, string message, Exception ex, LogLevel level = LogLevel.Error)
    {
        LogInternal(level, caption, message, ex);
    }

    public static void Log(string caption, LogLevel level, string message, params object[] args)
        => LogInternal(level, caption, string.Format(message, args));

    public static void Log(string message, LogLevel level = LogLevel.Information)
        => LogInternal(level, CallerCaption(), message);

    public static void Debug(string message)
        => LogInternal(LogLevel.Debug, CallerCaption(), message);

    public static void Info(string message)
        => LogInternal(LogLevel.Information, CallerCaption(), message);

    public static void Warn(string message)
        => LogInternal(LogLevel.Warning, CallerCaption(), message);

    public static void Error(string message)
        => LogInternal(LogLevel.Error, CallerCaption(), message);

    public static void Critical(string message)
        => LogInternal(LogLevel.Critical, CallerCaption(), message);

    public static void Debug(string message, params object[] args)
        => LogInternal(LogLevel.Debug, CallerCaption(), string.Format(message, args));

    public static void Info(string message, params object[] args)
        => LogInternal(LogLevel.Information, CallerCaption(), string.Format(message, args));

    public static void Warn(string message, params object[] args)
        => LogInternal(LogLevel.Warning, CallerCaption(), string.Format(message, args));

    public static void Error(string message, params object[] args)
        => LogInternal(LogLevel.Error, CallerCaption(), string.Format(message, args));

    public static void Error(string message, Exception ex)
        => LogInternal(LogLevel.Error, CallerCaption(), message, ex);

    public static void Critical(string message, Exception ex)
        => LogInternal(LogLevel.Critical, CallerCaption(), message, ex);
}