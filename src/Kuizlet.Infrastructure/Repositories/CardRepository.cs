using Kuizlet.Application.Interfaces.Repositories;
using Kuizlet.Domain.Entities;
using System;
using System.Collections.Concurrent;

namespace Kuizlet.Infrastructure.Repositories
{
    public class CardRepository : ICardRepository
    {
        private static readonly ConcurrentDictionary<Guid, Card> _cards = new();

        public Task<List<Card>> GetByCardSetIdAsync(Guid cardSetId)
        {
            var cards = _cards.Values
                .Where(c => c.CardSetId == cardSetId)
                .ToList();

            return Task.FromResult(cards);
        }

        public Task<Card?> GetByIdAsync(Guid id)
        {
            _cards.TryGetValue(id, out var card);
            return Task.FromResult(card);
        }

        public Task<Card> AddAsync(Card card)
        {
            if (card.Id == Guid.Empty)
                card.Id = Guid.NewGuid();

            _cards[card.Id] = card;
            return Task.FromResult(card);
        }

        public Task<Card> UpdateAsync(Card card)
        {
            if (_cards.ContainsKey(card.Id))
            {
                _cards[card.Id] = card;
            }
            return Task.FromResult(card);
        }

        public Task DeleteAsync(Guid id)
        {
            _cards.TryRemove(id, out _);
            return Task.CompletedTask;
        }

       
    }
}
