using Discord;
using OlliBot.Infrastructure;
using Serilog;
using Serilog.Events;

namespace OlliBot.Bot;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
#if DEBUG
            .WriteTo.Debug()
#endif
            .CreateBootstrapLogger();

        Log.Information("Creating OlliBot.Bot...");

        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

        Log.Information("Configuring OlliBot.Bot...");

        #region DI Setup
        builder.Services.AddSerilog((services, loggerConfiguration) => loggerConfiguration
            .ReadFrom.Configuration(builder.Configuration)
            .ReadFrom.Services(services));

        builder.Services.AddInfrastructureServices(builder.Configuration);
        builder.Services.AddBotServices();
        builder.Services.AddDiscordServices(builder.Configuration);
        #endregion

        try
        {
            IHost host = builder.Build();

            string middle = $"///// OlliBot.Bot {DateTime.Now:dd/MM/yyyy HH:mm:ss} /////";
            string border = new('/', middle.Length);

            Log.Information(border);
            Log.Information(middle);
            Log.Information(border);

            await host.RunAsync();
        }
        catch (HostAbortedException)
        {
            // Expected during EF Core tooling operations
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Host failed to run");
        }
        finally
        {
            Log.Information("Flushing logs...");
            await Log.CloseAndFlushAsync();
        }
    }

    internal static async Task LogAsync(LogMessage message)
    {
        LogEventLevel severity = message.Severity switch
        {
            LogSeverity.Critical => LogEventLevel.Fatal,
            LogSeverity.Error => LogEventLevel.Error,
            LogSeverity.Warning => LogEventLevel.Warning,
            LogSeverity.Info => LogEventLevel.Information,
            LogSeverity.Verbose => LogEventLevel.Verbose,
            LogSeverity.Debug => LogEventLevel.Debug,
            _ => LogEventLevel.Information
        };
        Log.Write(severity, message.Exception, "[{Source}] {Message}", message.Source, message.Message);
        await Task.CompletedTask;
    }
}