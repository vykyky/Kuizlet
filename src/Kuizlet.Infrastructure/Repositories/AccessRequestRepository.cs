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
    public class AccessRequestRepository : IAccessRequestRepository
    {
        private static readonly ConcurrentDictionary<Guid, AccessRequest> _accessRequests = new();
        public Task<List<AccessRequest>> GetRequestsByUserIdAsync(Guid userId)
        {
            var result = _accessRequests.Values
                .Where(r => r.RequesterId == userId)
                .ToList();

            return Task.FromResult(result);
        }
        public Task<AccessRequest?> GetByIdAsync(Guid id)
        {
            _accessRequests.TryGetValue(id, out var request);
            return Task.FromResult(request);
        }

        public Task<AccessRequest?> FindByCardSetAndUserAsync(Guid cardSetId, Guid userId)
        {
            var request = _accessRequests.Values
                .FirstOrDefault(r => r.CardSetId == cardSetId && r.RequesterId == userId);

            return Task.FromResult(request);
        }

        public Task<List<AccessRequest>> GetPendingByCardSetAsync(Guid cardSetId)
        {
            var requests = _accessRequests.Values
                .Where(r => r.CardSetId == cardSetId && r.Status == "PENDING")
                .ToList();

            return Task.FromResult(requests);
        }

        public Task AddAsync(AccessRequest request)
        {
            if (request.Id == Guid.Empty)
            {
                request.Id = Guid.NewGuid();
            }

            _accessRequests[request.Id] = request;
            return Task.CompletedTask;
        }

        public Task UpdateAsync(AccessRequest request)
        {
            if (_accessRequests.ContainsKey(request.Id))
            {
                _accessRequests[request.Id] = request;
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id)
        {
            _accessRequests.TryRemove(id, out _);
            return Task.CompletedTask;
        }

    }
}
