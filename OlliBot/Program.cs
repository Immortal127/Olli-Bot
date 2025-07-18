using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using OlliBot.Modules;
using OlliBot.Services;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace OlliBot
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

            try
            {
                builder.Configuration.AddJsonFile("config.json", optional: false, reloadOnChange: true);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to load config.json");
                throw;
            }

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console(outputTemplate: "{Timestamp:dd-MM-yyyy HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}", theme: AnsiConsoleTheme.Literate)
                //.Filter.ByExcluding(logEvent => logEvent.MessageTemplate.Text.Contains("Unknown event:"))
                .CreateLogger();

            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog();
            

            builder.Services.AddHostedService<Bot>();

            builder.Services.AddSingleton<DiscordSocketClient>((serviceProvider) =>
            {
                ConfigurationManager config = builder.Configuration;

                try
                {
                    var discordClient = new DiscordSocketClient(new DiscordSocketConfig
                    {
                        MessageCacheSize=5000,
                        AlwaysDownloadUsers=true,
                        GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.DirectMessages | GatewayIntents.GuildMembers
                    });

                    discordClient.Log += LogAsync;

                    return discordClient;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed to initialize Discord client");
                    throw;
                }
            });

            builder.Services.AddSingleton<InteractionService>((serviceProvider) =>
            {
                DiscordSocketClient discordClient = serviceProvider.GetRequiredService<DiscordSocketClient>();

                var interaction = new InteractionService(discordClient.Rest);

                return interaction;
            });

            builder.Services.AddTransient<BotInitialization>();
            builder.Services.AddSingleton<InteractionHandler>();
            builder.Services.AddSingleton<BotEventHandler>();


            builder.Services.AddTransient<IMessageService, MessageService>();
            builder.Services.AddTransient<IMessageFactory, MessageFactory>();
            builder.Services.AddTransient<IEmoteRankingService, EmoteRankingService>();
            builder.Services.AddSingleton<IDiscordClient>(sp => sp.GetRequiredService<DiscordSocketClient>());

            try
            {
                IHost host = builder.Build();
                host.Run();
            }
            catch (Exception ex) 
            {
                Log.Error($"Host failed to run: {ex.Message}");
            }
            finally
            {
                Log.Information("Flushing logs...");
                await Log.CloseAndFlushAsync();
            }
        }
        private static async Task LogAsync(LogMessage message)
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
}