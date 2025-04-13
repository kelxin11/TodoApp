using TodoApp.DTO;
using TodoApp.Models;

namespace TodoApp.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> RegisterAsync(UserRegisterDto dto);
        Task<(User user, string token, string refreshToken)> LoginAsync(UserLoginDto dto);
    }

}
