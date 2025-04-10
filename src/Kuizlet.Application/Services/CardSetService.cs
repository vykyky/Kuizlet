using Kuizlet.Application.Interfaces;
using Kuizlet.Application.Interfaces.Repositories;
using Kuizlet.Application.Models;
using Kuizlet.Domain.Entities;
using Kuizlet.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using DnsClient;



namespace Kuizlet.Application.Services
{
    public class CardSetService : ICardSetService
    {
        private readonly ICardSetRepository _cardSetRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserInfo _currentUserInfo;
        private readonly IAccessRequestRepository _accessRequestRepository;
        public CardSetService(
            ICardSetRepository cardSetRepository,
            ICurrentUserInfo currentUserInfo,
            IUserRepository userRepository,
            IAccessRequestRepository accessRequestRepository)
        {
            _cardSetRepository = cardSetRepository;
            _currentUserInfo = currentUserInfo;
            _userRepository = userRepository;
            _accessRequestRepository = accessRequestRepository;
        }

        public async Task<List<CardSetDto>> GetAllCardSetsAsync()
        {
            var currentLogin = _currentUserInfo.GetCurrentLogin();
            var currentUserId = await _userRepository.GetIdByLoginAsync(currentLogin);

            var cardSets = await _cardSetRepository.FindAllPublicAndAccessibleCardsetsAsync(currentUserId);
            var result = new List<CardSetDto>();

            foreach (var cardSet in cardSets)
            {
                var creator = await _userRepository.GetByIdAsync(cardSet.CreatorId);
                var creatorLogin = creator?.Login;

                // Теперь передаем _accessRequestRepository в GetAccessStatus
                var accessType = await GetAccessStatus(
                    cardSet,
                    currentUserId.Value,
                    _accessRequestRepository);

                result.Add(new CardSetDto(
                    cardSet.Id,
                    cardSet.Name,
                    cardSet.IsPublic,
                    cardSet.CreatorId,
                    creatorLogin,
                    accessType
                ));
            }

            return result;
        }

        public async Task<CardSetDto> GetCardSetByIdAsync(Guid cardSetId)
        {
            var currentLogin = _currentUserInfo.GetCurrentLogin();
            var currentUserId = await _userRepository.GetIdByLoginAsync(currentLogin);
            if (!currentUserId.HasValue)
                throw new UnauthorizedAccessException("Current user not found");

            var cardSet = await _cardSetRepository.FindPublicAndAccessibleCardsetsAsync(cardSetId, currentUserId.Value);

            if (cardSet == null)
            {
                var accessRequests = await _accessRequestRepository.GetRequestsByUserIdAsync(currentUserId.Value);
                var hasRequested = accessRequests.Any(ar => ar.CardSetId == cardSetId);

                if (!hasRequested)
                    throw new UnauthorizedAccessException("Access denied or Card Set not found");

                cardSet = await _cardSetRepository.FindByIdAsync(cardSetId);
                if (cardSet == null)
                    throw new KeyNotFoundException("Card set not found");

                var creator = await _userRepository.GetByIdAsync(cardSet.CreatorId);
                return new CardSetDto(
                    cardSet.Id,
                    cardSet.Name,
                    cardSet.IsPublic,
                    cardSet.CreatorId,
                    creator?.Login,
                    "REQUESTED"
                );
            }

            // Если карточка доступна напрямую
            var creatorUser = await _userRepository.GetByIdAsync(cardSet.CreatorId);
            var accessType = await GetAccessStatus(cardSet, currentUserId.Value, _accessRequestRepository);
            return new CardSetDto(
                cardSet.Id,
                cardSet.Name,
                cardSet.IsPublic,
                cardSet.CreatorId,
                creatorUser?.Login,
                accessType
            );
        }

        //норм
        public async Task<CardSetDto> CreateCardSetAsync(CardSetDto cardSetDto)
        {
            var currentUser = await _userRepository.GetByLoginAsync(_currentUserInfo.GetCurrentLogin())
                ?? throw new UserNotFoundException("User not found");

            var currentLogin = _currentUserInfo.GetCurrentLogin();
            Debug.WriteLine("Текущий пользователь:", currentLogin);
            var cardSet = new CardSet
            {
                Id = Guid.NewGuid(),
                Name = cardSetDto.Name,
                IsPublic = cardSetDto.IsPublic,
                CreatorId = currentUser.Id,
                ApprovedUserIds = new List<Guid?>()
            };

            await _cardSetRepository.AddAsync(cardSet);
            return ConvertCardSetToRecord(cardSet, "OWNER");
        }

        public async Task DeleteCardSetAsync(Guid id)
        {
            var cardSet = await FindCardSetByIdAndVerifyOwnerAsync(id);
            await _cardSetRepository.DeleteAsync(cardSet.Id);
        }

        public async Task<CardSetDto> UpdateCardSetAsync(Guid id, CardSetDto cardSetDto)
        {
            var cardSet = await FindCardSetByIdAndVerifyOwnerAsync(id);

            cardSet.Name = cardSetDto.Name;
            cardSet.IsPublic = cardSetDto.IsPublic;

            await _cardSetRepository.UpdateAsync(cardSet);
            return ConvertCardSetToRecord(cardSet, cardSetDto.AccessType);
        }

        private async Task<CardSet> FindCardSetByIdAndVerifyOwnerAsync(Guid id)
        {
            var currentLogin = _currentUserInfo.GetCurrentLogin();
            Debug.WriteLine($"Текущий пользователь: {currentLogin}");

            var cardSet = await _cardSetRepository.FindByIdAsync(id)
                ?? throw new UnauthorizedAccessException("Access denied");

            var creator = await _userRepository.GetByIdAsync(cardSet.CreatorId)
                ?? throw new UnauthorizedAccessException("Access denied");

            if (creator.Login != currentLogin)
                throw new UnauthorizedAccessException("Access denied");

            return cardSet;

        }

        //не работает отображение approved
        private async Task<string> GetAccessStatus(
    CardSet cardSet,
    Guid currentUserId,
    IAccessRequestRepository accessRequestRepository) // Добавляем зависимость от репозитория
        {
            if (cardSet.CreatorId == currentUserId)
                return "OWNER";

            if (cardSet.ApprovedUserIds.Contains(currentUserId))
                return "ACCESSIBLE";

            // Запрашиваем запрос на доступ для этого набора и пользователя
            var request = await accessRequestRepository.FindByCardSetAndUserAsync(cardSet.Id, currentUserId);

            if (request != null)
            {
                return request.Status switch
                {
                    "PENDING" => "PENDING",
                    "REJECTED" => "DECLINED"
                };
            }

            if (cardSet.IsPublic)
                return "PUBLIC";

            return "NONE";
        }

        private CardSetDto ConvertCardSetToRecord(CardSet cardSet, string accessType)
        {
            return new CardSetDto(
                cardSet.Id,
                cardSet.Name,
                cardSet.IsPublic,
                cardSet.CreatorId,
                null, // username можно добавить при необходимости
                accessType
            );
        }
    }
}
