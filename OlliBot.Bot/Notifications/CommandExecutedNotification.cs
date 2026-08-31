using Discord;
using Discord.Interactions;
using MediatR;

namespace OlliBot.Bot.Notifications;

internal record CommandExecutedNotification(
    ICommandInfo CommandInfo,
    IInteractionContext InteractionContext,
    IResult Result) : INotification;