using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AssetAllocation.Api;

[ApiController]
public class CreatePersonController : ControllerBase
{
    private readonly IMediator _mediator;

    public CreatePersonController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("/persons/create")]
    public async Task<ActionResult> CreatePerson([FromBody]CreatePersonCommand command, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(command, cancellationToken));
    }
}