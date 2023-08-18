using Api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Infrastructure.Configurations.Models;

public class ProductConfigurations : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.OwnsOne(x => x.Price);
        builder.Property(x => x.Title)
            .HasMaxLength(Product.TitleMaxLength)
            .IsRequired();
        builder.HasIndex(x => x.Title).IsUnique();

        builder
            .Property(b => b.Version)
            .IsRowVersion();
    }
}