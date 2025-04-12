using Kuizlet.Application.Interfaces.Repositories;
using Kuizlet.Application.Services;
using Kuizlet.Domain.Entities;
using Moq;

namespace Kuizlet.Tests.AuthTests
{
    public class RegisterServiceTests
    {
        private readonly Mock<IUserRepository> _userRepoMock = new(); //не зависим от работы репозитория
        private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
        private readonly RegisterService _registerService;

        public RegisterServiceTests()
        {
            _registerService = new RegisterService(
                _userRepoMock.Object,
                _passwordHasherMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_ValidData_RegistersUser() 
        {
            var name = "Test User";
            var login = "testuser";
            var password = "password";
            var hash = "hashed_password";
            var salt = "salt";

            //мы настроили что такого пользователя нет
            _userRepoMock.Setup(x => x.ExistsByLoginAsync(login)).ReturnsAsync(false);
            //тут вернули хэш и соль
            _passwordHasherMock.Setup(x => x.CreateHashWithSalt(password)).Returns((hash, salt));

            //вызываем сам метод
            await _registerService.RegisterAsync(name, login, password);

            //Проверка 
            _userRepoMock.Verify(x => x.AddAsync(It.Is<User>(u => // вызван AddAsync
                u.Login == login &&   // с правильными параметрами пользователя
                u.PasswordHash == hash &&
                u.Salt == salt &&
                u.FullName == name)),
                Times.Once);  // один раз
        }

        [Fact]
        public async Task RegisterAsync_EmptyPassword_ThrowsArgumentException()
        {
            var name = "Test User";
            var login = "testuser";
            var password = "";

            // проверка выбрасывается ли исключение что пароль не может быть пустым
            await Assert.ThrowsAsync<ArgumentException>(
                () => _registerService.RegisterAsync(name, login, password));
        }

        [Fact]
        public async Task RegisterAsync_ExistingLogin_ThrowsInvalidOperationException()
        {
            var name = "Test User";
            var login = "testuser";
            var password = "password";

            _userRepoMock.Setup(x => x.ExistsByLoginAsync(login))
                        .ReturnsAsync(true); //мы настроили что такой пользователь есть

            // проверка выбрасывается ли исключение что пользователь существует
            await Assert.ThrowsAsync<InvalidOperationException>(  
                () => _registerService.RegisterAsync(name, login, password));
        }
    }
}
