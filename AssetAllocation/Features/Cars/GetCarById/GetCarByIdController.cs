using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AssetAllocation.Api;

[ApiController]
public class GetCarByIdController : ControllerBase
{
    private readonly IMediator _mediator;

    public GetCarByIdController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("cars/{id:guid}")]
    public async Task<ActionResult> GetCarById([FromRoute]Guid id, CancellationToken cancellationToken)
    {
        GetCarByIdQuery query = new(id);
        return await _mediator.Send(query, cancellationToken);
    }
}
