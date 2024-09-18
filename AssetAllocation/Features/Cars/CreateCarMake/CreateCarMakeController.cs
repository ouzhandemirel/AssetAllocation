using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AssetAllocation.Api;

[ApiController]
public class CreateCarMakeController : ControllerBase
{
    private readonly IMediator _mediator;
    public CreateCarMakeController(IMediator mediator)
    {
        _mediator = mediator;   
    }

    [HttpPost("/car-makes/create")]
    public async Task<ActionResult> CreateCarMake([FromBody]CreateCarMakeCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }
}
