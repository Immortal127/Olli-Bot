using MediatR;
using OlliBot.Domain.Enums;

namespace OlliBot.Application.HumbleBundle;
public record CheckForHumbleBundleUpdatesCommand(HumbleBundleType BundleType) : IRequest<CheckForHumbleBundleUpdatesResult>;