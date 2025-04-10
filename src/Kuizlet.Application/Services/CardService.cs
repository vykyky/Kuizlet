using DnsClient.Protocol;
using Kuizlet.Application.Interfaces;
using Kuizlet.Application.Interfaces.Repositories;
using Kuizlet.Application.Models;
using Kuizlet.Domain.Entities;
using Kuizlet.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuizlet.Application.Services
{
    public class CardService : ICardService
    {
        private readonly ICardRepository _cardRepository;
        private readonly ICardSetRepository _cardSetRepository;

        public CardService(
            ICardRepository cardRepository,
            ICardSetRepository cardSetRepository)
        {
            _cardRepository = cardRepository;
            _cardSetRepository = cardSetRepository;
        }

        public async Task<List<CardDto>> GetCardsByCardSetIdAsync(Guid cardSetId)
        {
            var cards = await _cardRepository.GetByCardSetIdAsync(cardSetId);
            return cards.Select(ConvertToDto).ToList();
        }

        public async Task<CardDto> AddCardAsync(Guid cardSetId, CardDto cardDto)
        {
            var cardSet = await _cardSetRepository.GetByIdAsync(cardSetId)
                ?? throw new CardSetNotFoundException("CardSet not found");   // там было runtime

            var card = new Card
            {
                Id = Guid.NewGuid(),
                Term = cardDto.Term,
                Definition = cardDto.Definition,
                CardSetId = cardSetId
            };

            var createdCard = await _cardRepository.AddAsync(card);
            return ConvertToDto(createdCard);
        }

        public async Task<CardDto> UpdateCardAsync(Guid cardId, CardDto cardDto)
        {
            var existingCard = await _cardRepository.GetByIdAsync(cardId)
                ?? throw new CardNotFoundException($"Card not found with id: {cardId}");

            existingCard.Term = cardDto.Term;
            existingCard.Definition = cardDto.Definition;

            var updatedCard = await _cardRepository.UpdateAsync(existingCard);
            return ConvertToDto(updatedCard);
        }

        private CardDto ConvertToDto(Card card)
        {
            return new CardDto(
                card.Id,
                card.Term,
                card.Definition
            );
        }

    }
}
