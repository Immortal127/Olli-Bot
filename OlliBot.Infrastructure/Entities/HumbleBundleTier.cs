namespace OlliBot.Domain.Entities;
public class HumbleBundleTier
{
    public int Tier { get; set; }
    public Decimal Price { get; set; }
    public List<HumbleBundleItem> HumbleBundleItems { get; set; }
}
