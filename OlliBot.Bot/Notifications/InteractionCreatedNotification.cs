using Discord.WebSocket;
using MediatR;

namespace OlliBot.Bot.Notifications;
internal record InteractionCreatedNotification(SocketInteraction Interaction) : INotification;