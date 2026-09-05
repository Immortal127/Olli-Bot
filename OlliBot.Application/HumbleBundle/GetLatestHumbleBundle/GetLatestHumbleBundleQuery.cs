using MediatR;
using OlliBot.Domain.Enums;

namespace OlliBot.Application.HumbleBundle.GetLatestHumbleBundle;
public record GetLatestHumbleBundleQuery(HumbleBundleType BundleType) : IRequest<GetLatestHumbleBundleResult>;
