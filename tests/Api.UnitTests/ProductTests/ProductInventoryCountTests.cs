using Api.Domain;
using Api.Domain.Exceptions;
using Api.Features.Product.AddProduct;
using FluentAssertions;

namespace Api.UnitTests.ProductTests;

public class ProductInventoryCountTests
{
    [TestCase(10U, 5U, 15)]
    [TestCase(20U, 5U, 25)]
    [TestCase(1U, 2U, 3)]
    public void IncreaseInventory_WithNormalValues_MatchWithFinalValue(uint defaultCount, uint increaseValue,
        int finalValue)
    {
        var product = Product.Create(defaultCount, new AddProductDto("Apple", 55_000, 0));
        
        product.IncreaseInventory(increaseValue);
        
        product.InventoryCount.Should().Be((uint)finalValue);
    }


    [TestCase(10U, 5, 5)]
    [TestCase(20U, 5, 15)]
    public void DecreaseInventory_WithNormalValues_MatchWithFinalValue(uint defaultCount, int increaseValue,
        int finalValue)
    {
        var product = Product.Create(defaultCount, new AddProductDto("Apple", 55_000, 0));
        
        product.DecreaseInventory(increaseValue);
        
        product.InventoryCount.Should().Be((uint)finalValue);
    }

    [TestCase(1U, 2)]
    [TestCase(10U, 100)]
    public void DecreaseInventory_WithFalseValues_ThrowsProductOutOfInventoryException(uint defaultCount,
        int decreaseValue)
    {
        var product = Product.Create(defaultCount, new AddProductDto("Apple", 55_000, 0));
        
        Assert.Throws<ProductOutOfInventoryException>(() => { product.DecreaseInventory(decreaseValue); });
    }
}