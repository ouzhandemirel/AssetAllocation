using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AssetAllocation.Api;

[ApiController]
public class DeleteCarController : ControllerBase
{
    private readonly IMediator _mediator;

    public DeleteCarController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpDelete("/cars/delete/{carId:guid}")]
    public async Task<ActionResult> DeleteCar([FromRoute]Guid carId, CancellationToken cancellationToken)
    {
        var command = new DeleteCarCommand(carId);
        return await _mediator.Send(command, cancellationToken);
    }
}
