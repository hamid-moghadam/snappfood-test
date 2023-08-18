using Api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Infrastructure.Configurations.Seeders;

public class UserSeeder : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasData(new List<User>(new[]
        {
            new User
            {
                Id = 1,
                Name = "Hamid Moghadam"
            },
            new User
            {
                Id = 2,
                Name = "Hossein Ahmadi"
            },
            new User
            {
                Id = 3,
                Name = "Alireza Imani"
            }
        }));
    }
}