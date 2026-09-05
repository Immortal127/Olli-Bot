using MediatR;
using OlliBot.Domain.Enums;

namespace OlliBot.Application.HumbleBundle.ScanHumbleBundle;
public record ScanHumbleBundleCommand(HumbleBundleType BundleType) : IRequest<ScanHumbleBundleResult>;