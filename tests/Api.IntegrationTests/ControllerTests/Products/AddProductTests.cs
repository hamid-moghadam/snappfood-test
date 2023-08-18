using System.Net;
using System.Net.Http.Json;
using Api.Features.Common;
using Api.Features.Product.AddProduct;
using Api.IntegrationTests.Helpers;
using FluentAssertions;

namespace Api.IntegrationTests.ControllerTests.Products;

public class AddProductTests : BaseTests
{
    [Test]
    public async Task AddProduct_WithNormalValues_ReturnsCreated()
    {
        var result =
            await SnappFoodService.Client.PostAsJsonAsync("api/v1/products", new AddProductDto("Pen", 10_000, 5));

        result.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Test]
    public async Task AddProduct_WithDefaultValues_ValuesAreCorrect()
    {
        var result =
            await SnappFoodService.Client.PostAsJsonAsync("api/v1/products", new AddProductDto("Pencil", 10_000, 5));

        var productDto = await result.Content.ReadFromJsonAsync<ProductDto>();

        productDto.Price.GrossValue.Should().Be(10_000);
        productDto.Price.DiscountPercent.Should().Be(5);
        productDto.Price.NetValue.Should().Be((uint)(productDto.Price.GrossValue -
                                                     productDto.Price.DiscountPercent * productDto.Price.GrossValue /
                                                     100));
        productDto.Title.Should().Be("Pencil");
    }


    [Test]
    public async Task AddProduct_AnotherProductWithSameTitleAlreadyAdded_ReturnsBadRequest()
    {
        var product = new AddProductDto("Book 1", 10000, 5);

        await SnappFoodService.Client.PostAsJsonAsync("api/v1/products", product);
        var badResult = await SnappFoodService.Client.PostAsJsonAsync("api/v1/products", product);

        badResult.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [TestCase(null)]
    [TestCase("")]
    public async Task AddProduct_WithWrongTitles_ReturnsBadRequest(string title)
    {
        var result =
            await SnappFoodService.Client.PostAsJsonAsync("api/v1/products", new AddProductDto(title, 10000, 0));

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [TestCase("Test1", -5)]
    [TestCase("Test2", 120)]
    public async Task AddProduct_WithWrongDiscount_ReturnsBadRequest(string title, sbyte discount)
    {
        var result =
            await SnappFoodService.Client.PostAsJsonAsync("api/v1/products",
                new AddProductDto(title, 10000, discount));

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}