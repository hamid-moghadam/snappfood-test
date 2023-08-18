using Api.Domain.Exceptions;
using Api.Features.Product.AddProduct;
using Mapster;

namespace Api.Domain;

public class Product
{
    #region Consts

    public const int TitleMaxLength = 150;
    public const string RedisKeyTemplate = "Product:{0}";

    #endregion


    public int Id { get; private set; }
    public string Title { get; private set; }
    public uint InventoryCount { get; private set; }
    public Price Price { get; private set; }

    public uint Version { get; set; }

    public ICollection<Order> Orders { get; set; }


    public void IncreaseInventory(uint value)
    {
        InventoryCount += value;
    }

    public void DecreaseInventory(int value)
    {
        if (InventoryCount - value < 0)
            throw new ProductOutOfInventoryException(this);
        InventoryCount -= (uint)value;
    }

    public static Product Create(uint inventoryCount, AddProductDto addProductDto)
    {
        var product = addProductDto.Adapt<Product>();
        product.InventoryCount = inventoryCount;
        product.Price = new Price { DiscountPercent = addProductDto.DiscountPercent, GrossValue = addProductDto.Price };
        return product;
    }
}

public record Price
{
    public uint GrossValue { get; init; }
    public uint NetValue => (uint)(GrossValue - DiscountPercent * GrossValue / 100);
    public sbyte DiscountPercent { get; init; }
}