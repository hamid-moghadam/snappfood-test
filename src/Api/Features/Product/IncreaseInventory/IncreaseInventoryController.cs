using Api.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Product.IncreaseInventory;

public class IncreaseInventoryController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public IncreaseInventoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPut("products/{id}:increase")]
    public async Task<IActionResult> Increase(int id, [FromBody] IncreaseInventoryDto increaseInventoryDto,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(new IncreaseInventoryCommand(id, increaseInventoryDto), cancellationToken);
        return NoContent();
    }
}