using AiPmaPlatform.Domain.Common;

namespace AiPmaPlatform.Domain.Entities.Identity
{
    public class User : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "Developer"; // Admin, PM, Developer, Customer, etc.
    }
}