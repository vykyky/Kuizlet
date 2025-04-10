using Kuizlet.Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Kuizlet.Application.Interfaces;

namespace Kuizlet.Web.Controllers
{

    [ApiController]
    [Route("login")]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;

        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                var token = await _loginService.LoginAsync(request.Login, request.Password);
                return Ok(token);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid credentials");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
