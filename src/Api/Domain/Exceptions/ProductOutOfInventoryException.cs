using Api.Helpers;

namespace Api.Domain.Exceptions;

public class ProductOutOfInventoryException : BaseDomainException
{
    public ProductOutOfInventoryException(Product product) : base(
        $"Just {product.InventoryCount} {product.Title} is left in inventory", 325)
    {
    }
}