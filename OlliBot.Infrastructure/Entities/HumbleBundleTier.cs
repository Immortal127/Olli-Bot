namespace OlliBot.Infrastructure.Entities;
public class HumbleBundleTier
{
    public int Tier { get; set; }
    public decimal Price { get; set; }
    public List<HumbleBundleItem> HumbleBundleItems { get; set; }
}
