using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OlliBot.Infrastructure.Data.Configurations;
internal class HumbleBundleConfig : IEntityTypeConfiguration<Domain.Entities.HumbleBundleSubscriber>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.HumbleBundleSubscriber> b)
    {
        b.HasKey(x => new { x.Id });
    }
}
