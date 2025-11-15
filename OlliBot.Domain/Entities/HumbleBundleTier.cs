namespace OlliBot.Domain.Entities;
internal class HumbleBundleTier
{
    int Tier { get; set; }
    Decimal Price { get; set; }
    List<HumbleBundleItem> HumbleBundleItems { get; set; }
}
