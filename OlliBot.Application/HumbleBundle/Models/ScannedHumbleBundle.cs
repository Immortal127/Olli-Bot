using OlliBot.Domain.Enums;

namespace OlliBot.Application.HumbleBundle.Models;
public class ScannedHumbleBundle
{
    public required string Name { get; set; }
    public List<ScannedHumbleBundleTier> BundleTiers { get; set; }
    public DateTime ExpiryDate { get; set; }
    public required HumbleBundleType BundleType { get; set; }

}
