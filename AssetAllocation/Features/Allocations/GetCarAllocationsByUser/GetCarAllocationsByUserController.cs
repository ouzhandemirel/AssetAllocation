using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AssetAllocation.Api;

[ApiController]
public class GetCarAllocationsByUserController : ControllerBase
{
    private readonly IMediator _mediator;
    public GetCarAllocationsByUserController(IMediator mediator)
    {
        _mediator = mediator;   
    }

    [HttpGet("car-allocations/by-user/{personId:guid}")]
    public async Task<ActionResult> GetCarAllocationsByUser([FromRoute]Guid personId, CancellationToken cancellationToken)
    {
        var query = new GetCarAllocationsByUserQuery(personId);
        return await _mediator.Send(query, cancellationToken);
    }
}
