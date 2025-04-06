using DnsClient.Protocol;
using Kuizlet.Application.Interfaces;
using Kuizlet.Application.Interfaces.Repositories;
using Kuizlet.Application.Models;
using Kuizlet.Domain.Entities;
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

        public CardService(ICardRepository cardRepository, ICardSetRepository cardSetRepository)
        {
            _cardRepository = cardRepository;
            _cardSetRepository = cardSetRepository;
        }

        public async Task<List<CardDto>> GetAllCardsByCardSetIdAsync(Guid cardSetId)
        {
            var cards = await _cardRepository.GetByCardSetIdAsync(cardSetId);
            return cards.Select(ConvertToRecord).ToList();
        }

        public async Task<CardDto> AddCardAsync(Guid cardSetId, CardDto cardRecord)
        {
            var cardSet = await _cardSetRepository.GetByIdAsync(cardSetId)
                ?? throw new Exception("CardSet not found");

            var card = new Card
            {
                Id = Guid.NewGuid(),
                Term = cardRecord.Term,
                Definition = cardRecord.Definition,
                CardSetId = cardSetId
            };

            await _cardRepository.AddAsync(card);
            return ConvertToRecord(card);
        }

        public async Task<CardDto> UpdateCardAsync(Guid cardId, CardDto cardRecord)
        {
            var existingCard = await _cardRepository.GetByIdAsync(cardId)
                ?? throw new Exception($"Card not found with id: {cardId}");

            existingCard.Term = cardRecord.Term;
            existingCard.Definition = cardRecord.Definition;

            await _cardRepository.UpdateAsync(existingCard);
            return ConvertToRecord(existingCard);
        }

        private CardDto ConvertToRecord(Card card)
        {
            return new CardDto(card.Id, card.Term, card.Definition);
        }

    }
}
