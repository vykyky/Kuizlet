using Kuizlet.Application.Interfaces.Repositories;
using Kuizlet.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuizlet.Application.Services
{
    public class AccessRequestService
    {
        private readonly IAccessRequestRepository _accessRequestRepository;
        private readonly ICardSetRepository _cardSetRepository;
        private readonly IUserRepository _userRepository;
        private readonly UserService _userService;

        public AccessRequestService(
            IAccessRequestRepository accessRequestRepository,
            ICardSetRepository cardSetRepository,
            IUserRepository userRepository,
            UserService userService)
        {
            _accessRequestRepository = accessRequestRepository;
            _cardSetRepository = cardSetRepository;
            _userRepository = userRepository;
            _userService = userService;
        }

        public async Task<ResponseDto> RequestAccessAsync(Guid cardSetId)
        {
           throw new NotImplementedException();
        }

        public async Task<ResponseDto> RespondToRequestAsync(
            Guid cardSetId,
            Guid requestId,
            bool approve)
        {
          throw new NotImplementedException();
        }

        public async Task<List<AccessRequestDto>> GetPendingRequestsAsync(Guid cardSetId)
        {
            // Реализация...
           throw new NotImplementedException();
        }

       /* private AccessRequestDto ConvertToDto(AccessRequestDto request)
        {
            return new AccessRequestDto(

                request.Id,
                request.CardSet.Id,
                request.CardSet.Name,
                request.Requester.Username,
                request.Status
            );
        }*/
    }
}
