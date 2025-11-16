using OlliBot.Domain.Enums;

namespace OlliBot.Domain.Entities;
public class HumbleBundle
{
    public string Name { get; set; }
    public List<HumbleBundleTier> BundleTiers { get; set; }
    public DateTime ExpiryDate { get; set; }
    public HumbleBundleType BundleType { get; set; }

}
