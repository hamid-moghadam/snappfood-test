using Api.Features.Common;
using Api.Features.Product.AddProduct;
using Api.Infrastructure.Persistence;
using Api.Options;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Api.Features.Product.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _appDbContext;
    private readonly IOptionsMonitor<DefaultValuesOptions> _defaultValuesOptionsMonitor;

    public ProductService(AppDbContext appDbContext, IOptionsMonitor<DefaultValuesOptions> defaultValuesOptionsMonitor)
    {
        _appDbContext = appDbContext;
        _defaultValuesOptionsMonitor = defaultValuesOptionsMonitor;
    }

    public async Task<ProductDto?> GetAsync(int id, CancellationToken cancellationToken)
    {
        return await _appDbContext.Products.AsNoTracking().Where(x => x.Id == id).ProjectToType<ProductDto>()
            .SingleOrDefaultAsync(cancellationToken: cancellationToken);
    }

    //todo: must have to find a way to remove it from ProductService
    public Task InvalidateCacheAsync(int id, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public async Task<ProductDto> AddAsync(AddProductDto addProductDto, CancellationToken cancellationToken)
    {
        if (await _appDbContext.Products.AnyAsync(x => x.Title == addProductDto.Title,
                cancellationToken: cancellationToken))
            throw new BadHttpRequestException("Product title already exists");

        var product = Domain.Product.Create(_defaultValuesOptionsMonitor.CurrentValue.InventoryCount, addProductDto);
        await _appDbContext.Products.AddAsync(product, cancellationToken);
        await _appDbContext.SaveChangesAsync(cancellationToken);

        return product.Adapt<ProductDto>();
    }
}