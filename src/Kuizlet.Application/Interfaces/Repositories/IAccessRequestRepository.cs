using Kuizlet.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuizlet.Application.Interfaces.Repositories
{
    public interface IAccessRequestRepository
    {
        Task<List<AccessRequest>> GetRequestsByUserIdAsync(Guid userId);
        Task<AccessRequest?> GetByIdAsync(Guid id);
        Task<AccessRequest?> FindByCardSetAndUserAsync(Guid cardSetId, Guid userId);
        Task<List<AccessRequest>> GetPendingByCardSetAsync(Guid cardSetId);
        Task AddAsync(AccessRequest request);
        Task UpdateAsync(AccessRequest request);
        Task DeleteAsync(Guid id);
    }
}
