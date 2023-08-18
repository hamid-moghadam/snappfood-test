using System.Net;
using System.Net.Http.Json;
using Api.Features.Common;
using Api.Features.Order.AddOrder;
using Api.Features.Product.AddProduct;
using Api.IntegrationTests.Helpers;
using FluentAssertions;

namespace Api.IntegrationTests.ControllerTests.Orders;

public class AddOrderTests : BaseTests
{
    [Test]
    public async Task AddOrder_WithNormalValues_ReturnsCreated()
    {
        uint increaseCount = 10;
        var buyCount = 5;

        var productDto = await SnappFoodService.CreateProductAsync(new AddProductDto("Apple", 20_000, 1));
        await SnappFoodService.IncreaseProductInventoryAsync(productDto!.Id, increaseCount);

        var result =
            await SnappFoodService.Client.PostAsJsonAsync("api/v1/orders:buy",
                new AddOrderCommand(productDto.Id, 1, buyCount));

        result.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Test]
    public async Task AddOrder_WithDefaultValues_ValuesAreCorrect()
    {
        uint increaseCount = 10;
        var buyCount = 5;

        var productDto = await SnappFoodService.CreateProductAsync(new AddProductDto("Car", 20_000, 1));
        await SnappFoodService.IncreaseProductInventoryAsync(productDto!.Id, increaseCount);

        var result =
            await SnappFoodService.Client.PostAsJsonAsync("api/v1/orders:buy",
                new AddOrderCommand(productDto.Id, 1, buyCount));
        var order = await result.Content.ReadFromJsonAsync<OrderDto>();

        order.Should().NotBeNull();
        order.Detail.DiscountPercent.Should().Be(1);
        order.Detail.Count.Should().Be(5);
        order.Product.InventoryCount.Should().Be((uint)(productDto.InventoryCount + increaseCount - buyCount));
    }

    [Test]
    public async Task AddOrder_WithHigherInventoryCount_ReturnsBadRequest()
    {
        uint productIncreaseCount = 10;
        var productDto = await SnappFoodService.CreateProductAsync(new AddProductDto("Banana", 20_000, 1));
        await SnappFoodService.IncreaseProductInventoryAsync(productDto!.Id, productIncreaseCount);

        var result =
            await SnappFoodService.Client.PostAsJsonAsync("api/v1/orders:buy",
                new AddOrderCommand(productDto.Id, 1, (int)(productIncreaseCount * 200)));

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}