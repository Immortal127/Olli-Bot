using Discord;

namespace OlliBot.Bot.Services;
public class DiscordLogService(ILogger<DiscordLogService> logger)
{
    public Task Log(LogMessage message)
    {
        logger.Log(
            ConvertLogSeverity(message.Severity),
            message.Exception,
            "[{Source}] {Message}",
            message.Source,
            message.Message);

        return Task.CompletedTask;
    }

    private static LogLevel ConvertLogSeverity(LogSeverity severity)
    {
        return severity switch
        {
            LogSeverity.Critical => LogLevel.Critical,
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Warning => LogLevel.Warning,
            LogSeverity.Info => LogLevel.Information,
            LogSeverity.Verbose => LogLevel.Trace,
            LogSeverity.Debug => LogLevel.Debug,
            _ => LogLevel.Information
        };
    }
}
