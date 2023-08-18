using Api.Features.Product.Services;
using Api.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Product.AddProduct;

public class AddProductController : ApiControllerBase
{
    private readonly IProductService _productService;

    public AddProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpPost("products")]
    public async Task<IActionResult> Add([FromBody] AddProductDto addProductDto, CancellationToken cancellationToken)
    {
        var productDto = await _productService.AddAsync(addProductDto, cancellationToken);
        return Created("", productDto);
    }
}