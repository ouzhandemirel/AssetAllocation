using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AssetAllocation.Api;

[ApiController]
public class AllocateCarConroller : ControllerBase
{
    private readonly IMediator _mediator;

    public AllocateCarConroller(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("car-allocations/allocate")]
    public async Task<ActionResult> AllocateCar([FromBody] AllocateCarCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }
}
