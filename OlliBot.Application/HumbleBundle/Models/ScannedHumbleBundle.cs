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
            int lineLimit = 70; // Number of characters discord can fit per line in an embed
            int lineCount = 3; // Number of lines we want in each HB embed

            int TruncationLimit = lineCount * lineLimit;

            return Description.Length > TruncationLimit ? Description[..TruncationLimit].Trim() + "..." : Description;
        }
    }
    public string Url { get; set; }
    public string ImageUrl { get; set; }
    public string Note { get; set; }
}
