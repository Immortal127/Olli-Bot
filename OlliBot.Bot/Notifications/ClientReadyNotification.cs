using MediatR;

namespace OlliBot.Bot.Notifications;

public record ClientReadyNotification() : INotification;