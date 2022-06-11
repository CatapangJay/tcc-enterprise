using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TCC.Identity.Interfaces;
using TCC.Identity.Models;
using TCC.Identity.MS.Models;

namespace TCC.Identity.MS.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtSettings _jwtSettings;

        public AuthenticationService(UserManager<ApplicationUser> userManager,
                                     IOptions<JwtSettings> jwtSettings,
                                     SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
            _signInManager = signInManager;
        }

        public async Task<AuthenticationDetails> AuthenticateAsync(AuthenticationRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
                throw new Exception($"User with {request.Email} not found");

            var result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, false, false);

            if (!result.Succeeded)
                throw new Exception($"Credentials for {request.Email} are not valid.");

            JwtSecurityToken jwtSecurityToken = await GenerateToken(user);

            var securityDetails = new AuthenticationDetails
            {
                Id = user.Id,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Email = request.Email,
                Username = user.UserName,
            };

            return securityDetails;
        }

        public async Task<RegistrationDetails> RegisterAsync(RegistrationRequest request)
        {
            var existingUser = await _userManager.FindByNameAsync(request.Username);

            if (existingUser != null)
                throw new Exception($"Username {request.Username} already exists.");

            var existingEmail = await _userManager.FindByEmailAsync(request.Email);

            if (existingEmail != null)
                throw new Exception($"Email {request.Email} already exists.");

            var user = new ApplicationUser
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.Username,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
                return new RegistrationDetails() { UserId = user.Id };

            throw new Exception($"{result.Errors}");
        }

        private async Task<JwtSecurityToken> GenerateToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var roleClaims = new List<Claim>();

            for (int i = 0; i < roles.Count; i++)
            {
                roleClaims.Add(new Claim("roles", roles[i]));
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signInCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: signInCredentials);

            return jwtSecurityToken;
        }
    }
}
