using Api.Features.Product.Services;
using Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Product.IncreaseInventory;

public record IncreaseInventoryCommand(int ProductId, IncreaseInventoryDto Dto) : IRequest;

public class IncreaseInventoryCommandHandler : IRequestHandler<IncreaseInventoryCommand>
{
    private readonly AppDbContext _appDbContext;
    private readonly IProductService _productService;

    public IncreaseInventoryCommandHandler(AppDbContext appDbContext, IProductService productService)
    {
        _appDbContext = appDbContext;
        _productService = productService;
    }

    public async Task Handle(IncreaseInventoryCommand request, CancellationToken cancellationToken)
    {
        var product =
            await _appDbContext.Products.SingleOrDefaultAsync(x => x.Id == request.ProductId,
                cancellationToken: cancellationToken);

        if (product == null)
            throw new BadHttpRequestException("ProductId not found");

        product.IncreaseInventory(request.Dto.Count);

        var executionStrategy = _appDbContext.Database.CreateExecutionStrategy();
        await executionStrategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _appDbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                _appDbContext.Update(product);
                await _appDbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new BadHttpRequestException("Concurrency error. Try again later");
            }
        });
        
        await _productService.InvalidateCacheAsync(request.ProductId, cancellationToken);
    }
}