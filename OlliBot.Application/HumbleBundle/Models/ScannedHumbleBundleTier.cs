namespace OlliBot.Application.HumbleBundle.Models;
public class ScannedHumbleBundleTier
{
    public int Tier { get; set; }
    public decimal Price { get; set; }
    public List<ScannedHumbleBundleItem> HumbleBundleItems { get; set; }
}
