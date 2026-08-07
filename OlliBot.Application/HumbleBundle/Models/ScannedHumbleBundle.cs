using OlliBot.Domain.Enums;

namespace OlliBot.Application.HumbleBundle.Models;
public class ScannedHumbleBundle
{
    public required string Name { get; set; }
    public List<ScannedHumbleBundleTier> BundleTiers { get; set; } = new List<ScannedHumbleBundleTier>();
    public DateTime ExpiryDate { get; set; }
    public required HumbleBundleType BundleType { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ShortDescription
    {
        get
        {
            int TruncationLimit = 150;

            return Description.Length > TruncationLimit ? Description[..TruncationLimit] + "..." : Description;
        }
    }
    public string Url { get; set; }
    public string ImageUrl { get; set; }
    public string Note { get; set; }
}
