using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OlliBot.Application.Messages.Commands;

namespace OlliBot.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration cfg)
    {

        services.AddTransient<AddMessageHandler>();

        return services;
    }
}
