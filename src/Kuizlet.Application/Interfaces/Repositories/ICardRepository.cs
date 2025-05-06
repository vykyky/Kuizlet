using Kuizlet.Domain.Entities;

namespace Kuizlet.Application.Interfaces.Repositories
{
    public interface ICardRepository
    {
        Task<List<Card>> GetByCardSetIdAsync(Guid cardSetId);
        Task<Card?> GetByIdAsync(Guid id);
        Task<Card> AddAsync(Card card);
        Task<Card> UpdateAsync(Card card);
        Task DeleteAsync(Guid id);

    }
}
