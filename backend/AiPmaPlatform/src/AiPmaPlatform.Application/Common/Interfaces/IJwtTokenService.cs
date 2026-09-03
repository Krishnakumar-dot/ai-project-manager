using AiPmaPlatform.Domain.Entities.Identity;

namespace AiPmaPlatform.Application.Common.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
    }
}