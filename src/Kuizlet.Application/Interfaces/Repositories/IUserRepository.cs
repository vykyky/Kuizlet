using Kuizlet.Domain.Entities;

namespace Kuizlet.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task AddAsync(User user);
        public Task<bool> ExistsByLoginAsync(string login);
        Task<User?> GetByLoginAsync(string login); // Добавляем новый метод
        Task<Guid?> GetIdByLoginAsync(string login);
        public Task<User?> GetByIdAsync(Guid id);
    }
}
