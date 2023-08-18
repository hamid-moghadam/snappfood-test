using Api.Features.Common;
using Api.Features.Product.AddProduct;

namespace Api.Features.Product.Services;

public interface IProductService
{
    Task<ProductDto?> GetAsync(int id, CancellationToken cancellationToken);
    Task InvalidateCacheAsync(int id, CancellationToken cancellationToken);
    Task<ProductDto> AddAsync(AddProductDto addProductDto, CancellationToken cancellationToken);
}