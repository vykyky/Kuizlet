using Kuizlet.Application.Interfaces;
using Kuizlet.Application.Interfaces.Repositories;
using Kuizlet.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuizlet.Application.Services
{
    public class LoginService : ILoginService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenGenerator _tokenGenerator;

        public LoginService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            ITokenGenerator tokenGenerator)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<string> LoginAsync(string login, string password)
        {
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Username and password cannot be null or empty");
            }

            // Получаем пользователя из репозитория
            var user = await _userRepository.GetByLoginAsync(login);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid username or password");
            }

            // Проверяем пароль
            bool isPasswordValid = _passwordHasher.Verify(
                password,
                user.PasswordHash,
                user.Salt);

            if (!isPasswordValid)
            {
                throw new UnauthorizedAccessException("Invalid username or password");
            }

            // Генерируем JWT токен
            return _tokenGenerator.GenerateToken(user.Login);
        }
    }
}

