using Kuizlet.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuizlet.Application.Interfaces
{
    public interface IAccessRequestService
    {
        Task<ResponseDto> RequestAccessAsync(Guid cardSetId);
        Task<ResponseDto> RespondToRequestAsync(Guid cardSetId, Guid requestId, bool approve);
        Task<List<AccessRequestDto>> GetPendingRequestsAsync(Guid cardSetId);

    }
}
