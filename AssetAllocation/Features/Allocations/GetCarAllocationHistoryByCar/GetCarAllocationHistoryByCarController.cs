using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AssetAllocation.Api;

[ApiController]
public class GetCarAllocationHistoryByCarController : ControllerBase
{
    private readonly IMediator _mediator;

    public GetCarAllocationHistoryByCarController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("car-allocations/history-by-car/{carId:guid}")]
    public async Task<ActionResult> GetCarAllocationHistoryByCar([FromRoute]Guid carId, CancellationToken cancellationToken)
    {
        var query = new GetCarAllocationHistoryByCarQuery(carId);
        return await _mediator.Send(query, cancellationToken);
    }
}
