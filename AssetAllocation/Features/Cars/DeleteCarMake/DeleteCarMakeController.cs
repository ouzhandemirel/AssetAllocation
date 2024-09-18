using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AssetAllocation.Api;

[ApiController]
public class DeleteCarMakeController : ControllerBase
{
    private readonly IMediator _mediator;

    public DeleteCarMakeController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpDelete("/car-makes/delete/{carMakeId:guid}")]
    public async Task<ActionResult> DeleteCarMake([FromRoute]Guid carMakeId, CancellationToken cancellationToken)
    {
        var command = new DeleteCarMakeCommand(carMakeId);
        return await _mediator.Send(command, cancellationToken);
    }
}
