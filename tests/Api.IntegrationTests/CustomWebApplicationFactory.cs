using System.Data.Common;
using Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Api.IntegrationTests;

public class CustomWebApplicationFactory<TProgram, TDbContext>
    : WebApplicationFactory<TProgram> where TProgram : class where TDbContext : DbContext
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder
            .ConfigureAppConfiguration((_, builder) =>
            {
                // builder
                    // .SetBasePath(AppContext.BaseDirectory).AddJsonFile("appsettings.Development.json");
            })
            .ConfigureServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                         typeof(DbContextOptions<TDbContext>));

                services.Remove(dbContextDescriptor);

                var dbConnectionDescriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                         typeof(DbConnection));

                services.Remove(dbConnectionDescriptor);
                services.AddDbContext<TDbContext>((container, options) =>
                {
                    var configuration = container.GetRequiredService<IConfiguration>();

                    options.UseNpgsql(configuration["ConnectionStrings:AppDbContextTest"]);
                });


                using var scope = services.BuildServiceProvider().CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<AppDbContext>();
                db.Database.EnsureDeleted();
                db.Database.Migrate();
            });

        builder.UseEnvironment("Development");
    }
}