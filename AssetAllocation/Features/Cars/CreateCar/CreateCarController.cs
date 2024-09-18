using System.Runtime.CompilerServices;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static AssetAllocation.Api.CreateCarCommand;

namespace AssetAllocation.Api;

[ApiController]
public class CreateCarController : ControllerBase
{
    private readonly IMediator _mediator;

    public CreateCarController(IMediator mediator, CreateCarCommandValidator validator)
    {
        _mediator = mediator;
    }

    [HttpPost("cars/create")]
    public async Task<ActionResult> CreateCar([FromBody]CreateCarCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }
}
