using Api.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Order.AddOrder;

public class AddOrderController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public AddOrderController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("orders:buy")]
    public async Task<ActionResult> Buy([FromBody] AddOrderCommand addOrderCommand, CancellationToken cancellationToken)
    {
        var order = await _mediator.Send(addOrderCommand, cancellationToken);
        return Created("", order);
    }
}