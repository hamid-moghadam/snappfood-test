using Api.Infrastructure.Persistence;
using Api.IntegrationTests.Services;

namespace Api.IntegrationTests.Helpers;

public class BaseTests :IDisposable, IAsyncDisposable
{
    private CustomWebApplicationFactory<Program, AppDbContext> _factory;
    protected SnappFoodService SnappFoodService { get; private set; }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _factory = new CustomWebApplicationFactory<Program, AppDbContext>();
        SnappFoodService = new SnappFoodService(_factory);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
    }

    public void Dispose()
    {
        _factory.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _factory.DisposeAsync();
    }
}