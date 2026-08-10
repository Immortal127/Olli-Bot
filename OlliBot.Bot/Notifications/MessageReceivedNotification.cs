using Discord.WebSocket;
using MediatR;

namespace OlliBot.Bot.Notifications;

public record MessageReceivedNotification(SocketMessage Message) : INotification;