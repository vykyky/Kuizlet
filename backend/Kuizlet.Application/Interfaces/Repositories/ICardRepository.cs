using Kuizlet.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuizlet.Application.Interfaces.Repositories
{
    public interface ICardRepository
    {
        Task AddAsync(Card card);
        Task<List<Card>> GetByCardSetIdAsync(Guid cardSetId);
        Task<Card?> GetByIdAsync(Guid id);
        Task UpdateAsync(Card card);
    }
}
