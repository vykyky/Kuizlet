using Kuizlet.Application.Interfaces;
using Kuizlet.Application.Interfaces.Repositories;
using Kuizlet.Application.Models;
using Kuizlet.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuizlet.Application.Services
{
    public class RegisterService : IRegisterService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public RegisterService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task RegisterAsync(string name, string login, string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Password cannot be null or empty");
            }

            if (await _userRepository.ExistsByLoginAsync(login))
            {
                throw new InvalidOperationException("User with this username already exists");
            }
            //можно добавить проверку на то что пользователь уже существует


            var (hash, salt) = _passwordHasher.CreateHashWithSalt(password);

            var user = new User
            {
                Login = login,
                PasswordHash = hash,
                Salt = salt, 
                FullName = name
            };

            await _userRepository.AddAsync(user);
        }

    }
}
