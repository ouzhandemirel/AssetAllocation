using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AssetAllocation.Api.Features.Account.GetAccessToken
{
    [ApiController]
    public class GetAccessTokenController : ControllerBase
    {
        private readonly IMediator _mediator;
        public GetAccessTokenController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("token")]
        public async Task<ActionResult> GetAccessToken([FromBody]GetAccessTokenQuery request, CancellationToken cancellationToken)
        {
            return await _mediator.Send(request, cancellationToken);
        }
    }
}
