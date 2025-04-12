using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Kuizlet.Tests.AuthTests
{
    public class AuthApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;                 

        public AuthApiTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Register_ValidData_ReturnsCreated()
        {
            // Arrange
            var request = new    //dto
            {
                Name = "Test User",
                Login = "testuser",
                Password = "P@ssw0rd"
            };

            var response = await _client.PostAsJsonAsync("/register", request); //посылаем запрос(куда и что)

            Assert.Equal(HttpStatusCode.Created, response.StatusCode); //201
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsToken()
        {
            // регистрация сначала, затем ууже вход. без регистрации
            var registerRequest = new
            {
                Name = "Test User",
                Login = "testuser",
                Password = "P@ssw0rd"
            };
            await _client.PostAsJsonAsync("/register", registerRequest);

            // Затем пробуем войти
            var loginRequest = new
            {
                Login = "testuser",
                Password = "P@ssw0rd"
            };
            var response = await _client.PostAsJsonAsync("/login", loginRequest);

            response.EnsureSuccessStatusCode();
            var token = await response.Content.ReadAsStringAsync();
            Assert.NotEmpty(token);
        }
    }
}
