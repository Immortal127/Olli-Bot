using OlliBot.Domain.Enums;

namespace OlliBot.Domain.Entities;
public class HumbleBundle
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Url { get; set; }

    public HumbleBundleType BundleType { get; set; }

    public DateTime ExpiryDate { get; set; }

    public DateTime DateSeen { get; set; }
}
