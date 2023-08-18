using Api.Features.Common;
using Api.Features.Product.Services;
using Api.Infrastructure.Persistence;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Order.AddOrder;

public record AddOrderCommand(int ProductId, int UserId, int Count) : IRequest<OrderDto>;

public class AddOrderCommandHandler : IRequestHandler<AddOrderCommand, OrderDto>
{
    private readonly AppDbContext _appDbContext;
    private readonly IProductService _productService;

    public AddOrderCommandHandler(AppDbContext appDbContext, IProductService productService)
    {
        _appDbContext = appDbContext;
        _productService = productService;
    }

    public async Task<OrderDto> Handle(AddOrderCommand request, CancellationToken cancellationToken)
    {
        var product = await _appDbContext.Products.SingleOrDefaultAsync(x => x.Id == request.ProductId,
            cancellationToken: cancellationToken);
        if (product is null)
            throw new BadHttpRequestException("ProductId is invalid");

        product.DecreaseInventory(request.Count);
        var order = Domain.Order.Create(product, request.ProductId, (uint)request.Count);

        var executionStrategy = _appDbContext.Database.CreateExecutionStrategy();
        await executionStrategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _appDbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await _appDbContext.Orders.AddAsync(order, cancellationToken);
                _appDbContext.Products.Update(product);
                await _appDbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new BadHttpRequestException(e.Message);
            }
        });
        await _productService.InvalidateCacheAsync(product.Id, cancellationToken);
        return order.Adapt<OrderDto>();
    }
}