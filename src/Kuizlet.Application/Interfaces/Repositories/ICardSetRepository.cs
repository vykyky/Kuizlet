using Kuizlet.Domain.Entities;
using Kuizlet.Application.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuizlet.Application.Interfaces.Repositories
{
    public interface ICardSetRepository
    {
        public Task<List<CardSet>> FindAllPublicAndAccessibleCardsetsAsync(Guid? currentUserId);
        Task<CardSet?> FindByIdAsync(Guid setId);
       // Task<string?> FindOwnerLoginByCardSetIdAsync(Guid cardSetId);
        Task<CardSet?> FindPublicAndAccessibleCardsetsAsync(Guid cardSetId, Guid currentUserId);
        Task AddApprovedUserAsync(Guid cardSetId, Guid userId);
        Task<CardSet?> GetByIdAsync(Guid id);

        Task AddAsync(CardSet cardSet);
        Task UpdateAsync(CardSet cardSet);
        Task DeleteAsync(Guid cardSetId);

    }
}
