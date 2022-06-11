using TCC.Identity.Models;

namespace TCC.Identity.Interfaces
{
    public interface IAuthenticationService
    {
        Task<AuthenticationDetails> AuthenticateAsync(AuthenticationRequest request);
        Task<RegistrationDetails> RegisterAsync(RegistrationRequest request);
    }
}
