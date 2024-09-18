using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AssetAllocation.Api;

[ApiController]
public class GetCarsByDynamicController : ControllerBase
{
    private readonly IMediator _mediator;

    public GetCarsByDynamicController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("cars/dynamic")]
    public async Task<ActionResult> GetCarsByDynamic(
        [FromQuery] PageRequest pageRequest,
        [FromBody] DynamicQuery dynamicQuery,
        CancellationToken cancellationToken)
    {
        GetCarsByDynamicQuery query = new(pageRequest, dynamicQuery);
        return await _mediator.Send(query, cancellationToken);
    }
}
