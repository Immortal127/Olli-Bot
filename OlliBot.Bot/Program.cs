using OlliBot.Application;
using OlliBot.Infrastructure;
using OlliBot.Infrastructure.Data;
using Serilog;

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

        builder.Services.AddApplication(builder.Configuration);
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddBotServices();
        builder.Services.AddDiscordServices(builder.Configuration);

        #endregion

        try
        {
            IHost host = builder.Build();

            string projectName = typeof(Program).Assembly.GetName().Name!;

            string middle = $"///// {projectName} {DateTime.Now:dd/MM/yyyy HH:mm:ss} /////";
            string border = new('/', middle.Length);

            Log.Information(border);
            Log.Information(middle);
            Log.Information(border);

            using (IServiceScope scope = host.Services.CreateScope())
            {
                DatabaseInitializer initializer =
                    scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();

                await initializer.InitializeAsync();
            }

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
}