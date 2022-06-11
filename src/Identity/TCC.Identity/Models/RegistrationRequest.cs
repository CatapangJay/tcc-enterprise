using TCC.Common.Shared.Enums;

namespace TCC.Identity.Models
{
    public class RegistrationRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserType Type { get; set; }
    }
}
