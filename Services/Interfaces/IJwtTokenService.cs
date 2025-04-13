using TodoApp.Models;

namespace TodoApp.Services.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);

        string GenerateRefreshToken();
    }

}
