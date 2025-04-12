using Kuizlet.Application.Interfaces.Repositories;
using Kuizlet.Application.Models;
using Kuizlet.Domain.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuizlet.Infrastructure.Repositories
{
    public class CardSetRepository : ICardSetRepository
    {
        private static readonly ConcurrentDictionary<Guid, CardSet> _cardSets = new();

        public Task<CardSet?> GetByIdAsync(Guid id)
        {
            _cardSets.TryGetValue(id, out var cardSet);
            return Task.FromResult(cardSet);
        }

        public Task<List<CardSet>> FindAllPublicAndAccessibleCardsetsAsync(Guid? currentUserId)
        {
            var result = _cardSets.Values
                .Where(c => c.CreatorId == currentUserId
                         || c.IsPublic
                         || c.ApprovedUserIds.Contains(currentUserId.GetValueOrDefault()))
                .ToList();

            return Task.FromResult(result);
        }

        public Task<CardSet?> FindByIdAsync(Guid setId)
        {
            var cardSet = _cardSets.Values
                .FirstOrDefault(c => c.Id == setId);
            return Task.FromResult(cardSet);
        }

        /*public Task<string?> FindOwnerLoginByCardSetIdAsync(Guid cardSetId)
        {
            if (_cardSets.TryGetValue(cardSetId, out var cardSet) &&
                _users.TryGetValue(cardSet.CreatorId, out var user))
            {
                return Task.FromResult(user.Login);
            }
            return Task.FromResult<string?>(null);
        }*/

        public Task AddApprovedUserAsync(Guid cardSetId, Guid userId)
        {
            if (_cardSets.TryGetValue(cardSetId, out var cardSet))
            {
                cardSet.ApprovedUserIds.Add(userId);
            }
            return Task.CompletedTask;
        }

        public Task<CardSet?> FindPublicAndAccessibleCardsetsAsync(Guid cardSetId, Guid userId)
        {
            var cardSet = _cardSets.Values
                .FirstOrDefault(c =>
                    c.Id == cardSetId &&
                    (c.CreatorId == userId ||
                     c.IsPublic ||
                     c.ApprovedUserIds.Contains(userId))
                );

            return Task.FromResult(cardSet);
        }

        public Task AddAsync(CardSet cardSet)
        {
            _cardSets[cardSet.Id] = cardSet; 
            return Task.CompletedTask;
        }

        public Task UpdateAsync(CardSet cardSet)
        {
            if (_cardSets.ContainsKey(cardSet.Id))
            {
                _cardSets[cardSet.Id] = cardSet; 
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid cardSetId)
        {
            _cardSets.TryRemove(cardSetId, out var removedCardSet);
            return Task.CompletedTask;
        }

    }
}
