using Kuizlet.Application.Interfaces.Repositories;
using Kuizlet.Domain.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuizlet.Infrastructure.Repositories
{
    public class CardRepository : ICardRepository
    {

        private static readonly ConcurrentDictionary<Guid, Card> _cards = new();

        public Task AddAsync(Card card)
        {
            _cards.TryAdd(card.Id, card);
            return Task.CompletedTask;
        }

        public Task<Card?> GetByIdAsync(Guid id)
        {
            _cards.TryGetValue(id, out var card);
            return Task.FromResult(card);
        }

        public Task<List<Card>> GetByCardSetIdAsync(Guid cardSetId)
        {
            var cards = _cards.Values
                .Where(c => c.CardSetId == cardSetId)
                .ToList();

            return Task.FromResult(cards);
        }
        public Task UpdateAsync(Card card)
        {
            if (card == null)
                throw new ArgumentNullException(nameof(card));

            // В ConcurrentDictionary обновление происходит атомарно
            _cards.AddOrUpdate(card.Id,
                addValueFactory: id => card,
                updateValueFactory: (id, existing) => card);

            return Task.CompletedTask;
        }
    }
}
