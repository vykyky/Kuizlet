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

    }
}
