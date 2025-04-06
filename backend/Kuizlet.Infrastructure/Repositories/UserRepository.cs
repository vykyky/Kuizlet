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
    public class UserRepository : IUserRepository
    {
        private static readonly ConcurrentDictionary<string, User> _users = new();

        public Task AddAsync(User user)
        {
            _users.TryAdd(user.Login, user);
            return Task.CompletedTask;
        }
        public Task<bool> ExistsByLoginAsync(string login)
        {
            return Task.FromResult(_users.ContainsKey(login));
        }
        public Task<User?> GetByLoginAsync(string login)
        {
            _users.TryGetValue(login, out User? user);
            return Task.FromResult(user);
        }
        public Task<Guid?> GetIdByLoginAsync(string login)
        {
            if (_users.TryGetValue(login, out var user))
                return Task.FromResult<Guid?>(user.Id);

            return Task.FromResult<Guid?>(null);
        }
        public Task<User?> GetByIdAsync(Guid id)
        {
            var user = _users.Values.FirstOrDefault(u => u.Id == id);
            return Task.FromResult(user);
        }

    }
}
