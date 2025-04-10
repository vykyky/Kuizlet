using Kuizlet.Application.Interfaces.Repositories;
using Kuizlet.Application.Models;
using Kuizlet.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kuizlet.Domain.Entities;
using Kuizlet.Domain.Exceptions;

namespace Kuizlet.Application.Services
{
    public class AccessRequestService : IAccessRequestService
    {
        private readonly IAccessRequestRepository _accessRequestRepository;
        private readonly ICardSetRepository _cardSetRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserInfo _currentUserInfo;

        public AccessRequestService(
            IAccessRequestRepository accessRequestRepo,
            ICardSetRepository cardSetRepo,
            IUserRepository userRepo,
            ICurrentUserInfo currentUserInfo)
        {
            _accessRequestRepository = accessRequestRepo;
            _cardSetRepository = cardSetRepo;
            _userRepository = userRepo;
            _currentUserInfo = currentUserInfo;
        }

        public async Task<ResponseDto> RequestAccessAsync(Guid cardSetId)
        {
            var currentLogin = _currentUserInfo.GetCurrentLogin();
            var currentUserId = await _userRepository.GetIdByLoginAsync(currentLogin);

            if (!currentUserId.HasValue)
                return new ResponseDto("error", "User not found");

            var cardSet = await _cardSetRepository.GetByIdAsync(cardSetId);
            if (cardSet == null)
                return new ResponseDto("error", "Card set not found");

            // Проверка владельца
            if (cardSet.CreatorId == currentUserId.Value)
                return new ResponseDto("message", "You are the creator of this card set");

            // Проверка уже имеющегося доступа
            if (cardSet.ApprovedUserIds.Contains(currentUserId.Value))
                return new ResponseDto("message", "You already have access to this card set");

            // Проверка существующих запросов
            var existingRequest = await _accessRequestRepository
                .FindByCardSetAndUserAsync(cardSetId, currentUserId.Value);

            if (existingRequest != null)
            {
                if (existingRequest.Status == "PENDING")
                    return new ResponseDto("message", "You already have a pending request for this card set");

                if (existingRequest.Status == "REJECTED")
                {
                    existingRequest.Status = "PENDING";
                    await _accessRequestRepository.UpdateAsync(existingRequest);
                    return new ResponseDto("success", "Request resent successfully");
                }
            }

            // Создание нового запроса
            var request = new AccessRequest
            {
                Id = Guid.NewGuid(),
                RequesterId = currentUserId.Value,
                CardSetId = cardSetId,
                Status = "PENDING",
            };

            await _accessRequestRepository.AddAsync(request);
            return new ResponseDto("success", "Request sent successfully");
        }

        public async Task<ResponseDto> RespondToRequestAsync(Guid cardSetId, Guid requestId, bool approve)
        {
            var request = await _accessRequestRepository.GetByIdAsync(requestId);
            if (request == null)
                throw new InvalidOperationException("Access request not found");

            // Проверка соответствия cardSetId
            if (request.CardSetId != cardSetId)
                throw new InvalidOperationException("Invalid cardSetId for the given request");

            var currentLogin = _currentUserInfo.GetCurrentLogin();
            var currentUserId = await _userRepository.GetIdByLoginAsync(currentLogin);

            var cardSet = await _cardSetRepository.GetByIdAsync(cardSetId);

            if (cardSet?.CreatorId != currentUserId)
                throw new UnauthorizedException("You are not authorized to respond to this request");

            if (approve)
            {
                // Одобрение запроса
                request.Status = "APPROVED";
                await _cardSetRepository.AddApprovedUserAsync(cardSetId, request.RequesterId);
                await _accessRequestRepository.DeleteAsync(requestId);
                return new ResponseDto("success", "Request has been approved");
            }
            else
            {
                // Отклонение запроса
                request.Status = "REJECTED";
                await _accessRequestRepository.UpdateAsync(request);
                return new ResponseDto("success", "Request has been rejected");
            }
        }

        public async Task<List<AccessRequestDto>> GetPendingRequestsAsync(Guid cardSetId)
        {
            var currentLogin = _currentUserInfo.GetCurrentLogin();
            var currentUserId = await _userRepository.GetIdByLoginAsync(currentLogin);
            var cardSet = await _cardSetRepository.GetByIdAsync(cardSetId);

            if (cardSet?.CreatorId != currentUserId)
                throw new UnauthorizedAccessException("Not authorized to view requests");

            var requests = await _accessRequestRepository.GetPendingByCardSetAsync(cardSetId);

            var result = new List<AccessRequestDto>();

            foreach (var request in requests)
            {
                var user = await _userRepository.GetByIdAsync(request.RequesterId);
                result.Add(new AccessRequestDto
                (
                    request.Id,
                    request.CardSetId,
                    cardSet.Name,
                    user?.Login,
                    request.Status
                ));
            }
          
            return result;

        }

    }
}
