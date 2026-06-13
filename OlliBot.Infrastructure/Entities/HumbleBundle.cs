using OlliBot.Domain.Enums;

namespace OlliBot.Domain.Entities;
public class HumbleBundle
{
    public required string Name { get; set; }
    public List<HumbleBundleTier> BundleTiers { get; set; }
    public DateTime ExpiryDate { get; set; }
    public required HumbleBundleType BundleType { get; set; }

}
