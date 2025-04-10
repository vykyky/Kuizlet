using Kuizlet.Application.Models;
using Kuizlet.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Kuizlet.Web.Controllers
{
    [Route("request")]
    [ApiController]
    public class AccessRequestController : ControllerBase
    {
        private readonly IAccessRequestService _accessRequestService;

        public AccessRequestController(IAccessRequestService accessRequestService)
        {
            _accessRequestService = accessRequestService;
        }

        [HttpPost("{cardSetId}/request-access")]
        public async Task<ActionResult<ResponseDto>> RequestAccess(Guid cardSetId)
        {
            var result = await _accessRequestService.RequestAccessAsync(cardSetId);
            return Ok(result);
        }

        [HttpGet("{cardSetId}/requests")]
        public async Task<ActionResult<IEnumerable<AccessRequestDto>>> GetPendingRequests(Guid cardSetId)
        {
            var requests = await _accessRequestService.GetPendingRequestsAsync(cardSetId);
            return Ok(requests);
        }

        [HttpPut("{cardSetId}/requests/{requestId}")]
        public async Task<ActionResult<ResponseDto>> RespondToRequest(
            Guid cardSetId,
            Guid requestId,
            [FromQuery] bool approve)
        {
            var result = await _accessRequestService.RespondToRequestAsync(cardSetId, requestId, approve);
            return Ok(result);
        }
    }
}
