using Api.Features.Common;
using Api.Features.Product.Services;
using Api.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Product.GetProduct;

public class GetProductController : ApiControllerBase
{
    private readonly IProductService _productService;

    public GetProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet("products/{id}")]
    public async Task<ProductDto?> Get(int id, CancellationToken cancellationToken)
    {
        return await _productService.GetAsync(id, cancellationToken);
    }
}