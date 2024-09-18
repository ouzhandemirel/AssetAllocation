using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AssetAllocation.Api;

[ApiController]
public class DeleteCarModelController : ControllerBase
{
    private readonly IMediator _mediator;

    public DeleteCarModelController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpDelete("/car-models/delete/{modelId:guid}")]
    public async Task<ActionResult> DeleteCarModel([FromRoute]Guid modelId, CancellationToken cancellationToken)
    {
        var command = new DeleteCarModelCommand(modelId);
        return await _mediator.Send(command, cancellationToken);
    }
}
