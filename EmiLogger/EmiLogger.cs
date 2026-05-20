namespace EmiLogger;

using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

public sealed class EmiLogger(string categoryName) : ILogger
{
    private readonly string _caption = categoryName.Contains('.')
        ? categoryName[(categoryName.LastIndexOf('.') + 1)..]
        : categoryName;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => logLevel >= Emi.MinLevel;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;
        Emi.LogInternal(logLevel, _caption, formatter(state, exception), exception);
    }
}

[ProviderAlias("Emi")]
public sealed class EmiLoggerProvider : ILoggerProvider
{
    private readonly ConcurrentDictionary<string, EmiLogger> _loggers = new();

    public ILogger CreateLogger(string categoryName)
        => _loggers.GetOrAdd(categoryName, name => new EmiLogger(name));

    public void Dispose() => _loggers.Clear();
}