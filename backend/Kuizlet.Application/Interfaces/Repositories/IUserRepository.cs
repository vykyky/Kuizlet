using Kuizlet.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
