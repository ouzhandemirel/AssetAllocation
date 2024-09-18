using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AssetAllocation.Api;

[ApiController]
public class CreateCarModelController : ControllerBase
{
    private readonly IMediator _mediator;
    public CreateCarModelController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("car-models/create")]
    public async Task<ActionResult> CreateCarModel([FromBody]CreateCarModelCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }
}
