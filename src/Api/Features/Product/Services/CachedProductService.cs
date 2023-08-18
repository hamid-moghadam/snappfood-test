using System.Text;
using System.Text.Json;
using Api.Features.Common;
using Api.Features.Product.AddProduct;
using Api.Options;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace Api.Features.Product.Services;

public class CachedProductService : IProductService
{
    private readonly IProductService _productService;
    private readonly IDistributedCache _distributedCache;
    private readonly IOptionsMonitor<DefaultValuesOptions> _defaultOptionsMonitor;

    public CachedProductService(IDistributedCache distributedCache, IProductService productService,
        IOptionsMonitor<DefaultValuesOptions> defaultOptionsMonitor)
    {
        _distributedCache = distributedCache;
        _productService = productService;
        _defaultOptionsMonitor = defaultOptionsMonitor;
    }


    public async Task<ProductDto?> GetAsync(int id, CancellationToken cancellationToken)
    {
        var key = GetProductKey(id);
        byte[]? productByteArray = null;
        // try
        // {
            productByteArray = await _distributedCache.GetAsync(key, token: cancellationToken);
        // }
        // catch (Exception e)
        // {
            // return await _productService.GetAsync(id, cancellationToken);
        // }

        string productString;
        if (productByteArray != null)
        {
            productString = Encoding.UTF8.GetString(productByteArray);
            return JsonSerializer.Deserialize<ProductDto>(productString);
        }

        var product = await _productService.GetAsync(id, cancellationToken);
        if (product is null)
            throw new BadHttpRequestException("Product not found");

        await AddToCacheAsync(product, cancellationToken);
        return product;
    }

    public async Task InvalidateCacheAsync(int id, CancellationToken cancellationToken)
    {
        var key = GetProductKey(id);
        try
        {
            await _distributedCache.RemoveAsync(key, cancellationToken);
        }
        catch
        {
            // ignored
            Console.WriteLine("yes");
        }
    }

    public async Task<ProductDto> AddAsync(AddProductDto addProductDto, CancellationToken cancellationToken)
    {
        var result = await _productService.AddAsync(addProductDto, cancellationToken);
        await AddToCacheAsync(result, cancellationToken);
        return result;
    }

    private string GetProductKey(int id) => string.Format(Domain.Product.RedisKeyTemplate, id);


    private async Task AddToCacheAsync(ProductDto product, CancellationToken cancellationToken)
    {
        string key = GetProductKey(product.Id);
        var productString = JsonSerializer.Serialize(product);
        var productByteArray = Encoding.UTF8.GetBytes(productString);

        var options = new DistributedCacheEntryOptions()
            .SetAbsoluteExpiration(
                DateTime.Now.AddMinutes(_defaultOptionsMonitor.CurrentValue.ProductAbsoluteExpirationInMinutes))
            .SetSlidingExpiration(
                TimeSpan.FromMinutes(_defaultOptionsMonitor.CurrentValue.ProductSlidingExpirationInMinutes));

        try
        {
            await _distributedCache.SetAsync(key, productByteArray, options, cancellationToken);
        }
        catch (Exception e)
        {
            // ignored
        }
    }
}