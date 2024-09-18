using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AssetAllocation.Api;

[ApiController]
public class UpdateCarMileageController : ControllerBase
{
    private readonly IMediator _mediator;

    public UpdateCarMileageController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPut("car-mileages/update")]
    public async Task<ActionResult> UpdateCarMileage([FromBody]UpdateCarMileageCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }
}
