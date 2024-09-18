using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AssetAllocation.Api.Features.Account;

[ApiController]
public class LoginController : ControllerBase
{
    private readonly IMediator _mediator;

    public LoginController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody]LoginQuery loginQuery, CancellationToken cancellationToken)
    {
        return await _mediator.Send(loginQuery, cancellationToken);
    }
}
