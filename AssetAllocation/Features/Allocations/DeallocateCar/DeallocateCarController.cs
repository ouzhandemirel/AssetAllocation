using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AssetAllocation.Api;

[ApiController]
public class DeallocateCarController : ControllerBase
{
    private readonly IMediator _mediator;

    public DeallocateCarController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("car-allocations/deallocate")]
    public async Task<ActionResult> DeallocateCar([FromBody]DeallocateCarCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }
}
