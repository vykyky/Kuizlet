using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Kuizlet.Application.Models;
using Kuizlet.Application.Interfaces;

namespace Kuizlet.Web.Controllers
{
    [ApiController]
    [Route("register")]
    public class RegisterController : ControllerBase
    {
        private readonly IRegisterService _registerService;

        public RegisterController(IRegisterService registerService)
        {
            _registerService = registerService;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            try
            {
                await _registerService.RegisterAsync(request.Name, request.Login, request.Password);
                return StatusCode(201, "Registration successful");
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
            {
                return Conflict(ex.Message); // 409
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // 400 
            }
        }
    }
}
