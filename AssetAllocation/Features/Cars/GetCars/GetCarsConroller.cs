using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AssetAllocation.Api;

[Authorize]
[ApiController]
public class GetCarsConroller : ControllerBase
{
    private readonly IMediator _mediator;

    public GetCarsConroller(IMediator mediator)
    {
        _mediator = mediator;
    }

    [EnableRateLimiting("fixed")]
    [HttpGet("cars")]
    public async Task<ActionResult> GetCars([FromQuery] PageRequest pageRequest, CancellationToken cancellationToken)
    {
        GetCarsQuery query = new(pageRequest);
        return await _mediator.Send(query, cancellationToken);
    }
}