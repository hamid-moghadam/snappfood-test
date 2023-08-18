using Api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Infrastructure.Configurations.Models;

public class OrderConfigurations : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.Property(x => x.Id).UseHiLo();
        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("now()");

        builder.OwnsOne(x => x.Detail);
    }
}