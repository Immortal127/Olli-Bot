using OlliBot.Domain.Enums;

namespace OlliBot.Domain.Entities;
internal class HumbleBundle
{
    public int Id { get; set; }

    public string Name { get; set; }

    public HumbleBundleType BundleType { get; set; }

    public DateTime ExpiryDate { get; set; }

    public DateTime DateSeen { get; set; }

    string Url { get; set; }
}
