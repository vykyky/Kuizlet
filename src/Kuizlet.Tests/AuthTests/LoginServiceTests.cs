using Kuizlet.Application.Services;
using Kuizlet.Application.Interfaces.Repositories;
using Kuizlet.Domain.Entities;
using Moq;

namespace Kuizlet.Tests.AuthTests
{
    public class LoginServiceTests
    {
        private readonly Mock<IUserRepository> _userRepoMock = new();
        private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
        private readonly Mock<ITokenGenerator> _tokenGeneratorMock = new();
        private readonly LoginService _loginService;

        public LoginServiceTests()
        {
            _loginService = new LoginService(
                _userRepoMock.Object,
                _passwordHasherMock.Object,
                _tokenGeneratorMock.Object);
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsToken()
        {
            var login = "testuser";
            var password = "password";
            var token = "jwt_token";
            var user = new User
            {
                Login = login,
                PasswordHash = "hashed_password",
                Salt = "salt"
            };

            //настроили что пользователь с таким логином есть
            _userRepoMock.Setup(x => x.GetByLoginAsync(login)).ReturnsAsync(user);
            //что пароль годный
            _passwordHasherMock.Setup(x => x.Verify(password, user.PasswordHash, user.Salt)).Returns(true);
            //что нам вернут токен
            _tokenGeneratorMock.Setup(x => x.GenerateToken(login)).Returns(token); 

            var result = await _loginService.LoginAsync(login, password);

            //проверка что оба токена совпадают
            Assert.Equal(token, result); 
        }

        [Fact]
        public async Task LoginAsync_InvalidPassword_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var login = "testuser";
            var password = "password";
            var user = new User
            {
                Login = login,
                PasswordHash = "hashed_password",
                Salt = "salt"
            };

            _userRepoMock.Setup(x => x.GetByLoginAsync(login)).ReturnsAsync(user);

            _passwordHasherMock.Setup(x => x.Verify(password, user.PasswordHash, user.Salt)).Returns(false);

            // проверка что пароль неверный и есть исключение
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _loginService.LoginAsync(login, password));
        }

        [Fact]
        public async Task LoginAsync_NonExistingUser_ThrowsUnauthorizedAccessException()
        {
            var login = "testuser";
            var password = "password";

            _userRepoMock.Setup(x => x.GetByLoginAsync(login)).ReturnsAsync((User)null);

            //проверка что пользователя нет и есть исключение
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _loginService.LoginAsync(login, password));
        }

        [Fact]
        public async Task LoginAsync_EmptyCredentials_ThrowsArgumentException()
        {
            var login = "";
            var password = "";

            //проверка что есть исключения на пустые 
            await Assert.ThrowsAsync<ArgumentException>(
                () => _loginService.LoginAsync(login, password));
        }
    }
}
