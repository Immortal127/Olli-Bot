using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OlliBot.Domain.Entities;

namespace OlliBot.Infrastructure.Data.Configurations;
internal class HumbleBundleSubscriberConfig : IEntityTypeConfiguration<HumbleBundleSubscriber>
{
    public void Configure(EntityTypeBuilder<HumbleBundleSubscriber> b)
    {
        b.HasKey(x => new { x.Id });
    }
}
