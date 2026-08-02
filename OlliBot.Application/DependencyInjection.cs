using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OlliBot.Application.Messages.Commands;
using OlliBot.Application.Messages.Queries;

namespace OlliBot.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration cfg)
    {

        services.AddTransient<AddMessageHandler>();
        services.AddTransient<CallMessageHandler>();
        services.AddTransient<DeleteMessageHandler>();
        services.AddTransient<ListMessageHandler>();
        services.AddTransient<UpdateMessageHandler>();
        return services;
    }
}
