using Discord;
using Discord.Interactions;
using MediatR;

namespace OlliBot.Bot.Notifications;

internal record SlashCommandExecutedNotification(
    SlashCommandInfo SlashCommandInfo,
    IInteractionContext InteractionContext,
    IResult Result) : INotification;