using System.Net.Http.Json;
using Api.Features.Common;
using Api.Features.Product.AddProduct;
using Api.Features.Product.IncreaseInventory;
using Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Api.IntegrationTests.Services;

public class SnappFoodService
{
    public HttpClient Client { get; }

    public SnappFoodService(CustomWebApplicationFactory<Program, AppDbContext> factory)
    {
        Client = factory
            .CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    public async Task<ProductDto?> CreateProductAsync(AddProductDto addProductDto)
    {
        var result = await Client.PostAsJsonAsync("api/v1/products", addProductDto);
        result.EnsureSuccessStatusCode();
        return await result.Content.ReadFromJsonAsync<ProductDto>();
    }

    public async Task IncreaseProductInventoryAsync(int productId, uint count)
    {
        var result =
            await Client.PutAsJsonAsync($"api/v1/products/{productId}:increase",
                new IncreaseInventoryDto(count));
        result.EnsureSuccessStatusCode();
    }
}