using Discord;
using Discord.Interactions;

namespace OlliBot.Bot;

[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Method,
    AllowMultiple = false,
    Inherited = true)]
public sealed class RequireDmOrGuildPermissionAttribute(
    GuildPermission permission) : PreconditionAttribute
{
    public override Task<PreconditionResult> CheckRequirementsAsync(
        IInteractionContext context,
        ICommandInfo commandInfo,
        IServiceProvider services)
    {
        if (context.Guild is null)
        {
            return Task.FromResult(PreconditionResult.FromSuccess());
        }

        if (context.User is not IGuildUser guildUser)
        {
            return Task.FromResult(
                PreconditionResult.FromError(
                    "Could not determine your guild permissions."));
        }

        bool hasPermission = guildUser.GuildPermissions.Has(permission);

        return Task.FromResult(
            hasPermission
                ? PreconditionResult.FromSuccess()
                : PreconditionResult.FromError(
                    $"You require the {permission} permission to use this command."));
    }
}