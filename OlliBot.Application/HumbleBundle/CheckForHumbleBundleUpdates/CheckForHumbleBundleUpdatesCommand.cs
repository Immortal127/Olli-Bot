using MediatR;
using OlliBot.Domain.Enums;

namespace OlliBot.Application.HumbleBundle.CheckForHumbleBundleUpdates;
public record CheckForHumbleBundleUpdatesCommand(HumbleBundleType BundleType) : IRequest<CheckForHumbleBundleUpdatesResult>;