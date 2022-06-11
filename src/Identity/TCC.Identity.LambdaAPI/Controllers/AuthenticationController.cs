using Microsoft.AspNetCore.Mvc;
using TCC.Identity.Interfaces;
using TCC.Identity.Models;

namespace TCC.Identity.LambdaAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate(AuthenticationRequest request)
        {
            return Ok(await _authenticationService.AuthenticateAsync(request));
        }
    }
}
