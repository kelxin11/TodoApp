using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TodoApp.Data;
using TodoApp.DTO;
using TodoApp.Exceptions;
using TodoApp.Models;
using TodoApp.Services.Interfaces;

namespace TodoApp.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IJwtTokenService _jwtService;
        private readonly IConfiguration _config;
        private readonly ILogger<UserService> _logger;
        private readonly ILogger<JwtTokenService> _loggertoken;

        public UserService(AppDbContext context, IJwtTokenService jwtService, IConfiguration config, ILogger<UserService> logger, ILogger<JwtTokenService> loggertoken)
        {
            _context = context;
            _jwtService = jwtService;
            _config = config;
            _logger = logger;
            _loggertoken = loggertoken;
        }

        public async Task<User> RegisterAsync(UserRegisterDto dto)
        {
            try
            {
                if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                    throw new ValidationException("Email already exists");

                var user = new User
                {
                    Username = dto.Username,
                    Email = dto.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User {Username} registered successfully", user.Username);
                return user;
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error occurred during user registration");
                throw; // rethrowing the exception after logging it
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering the user");
                throw; // rethrowing the exception after logging it
            }
        }

        public async Task<(User user, string token, string refreshToken)> LoginAsync(UserLoginDto dto)
        {
            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == dto.Username);
                if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                    throw new UnauthorizedException("Invalid credentials");

                user.LastLoginDate = DateTime.UtcNow;

                var refreshToken = new JwtTokenService(_config, _loggertoken).GenerateRefreshToken();
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

                await _context.SaveChangesAsync();

                var token = _jwtService.GenerateToken(user);

                _logger.LogInformation("User {Username} logged in successfully", user.Username);
                return (user, token, refreshToken);
            }
            catch (UnauthorizedException ex)
            {
                _logger.LogWarning(ex, "Unauthorized login attempt for user {Username}", dto.Username);
                throw; // rethrowing the exception after logging it
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while logging in user {Username}", dto.Username);
                throw; // rethrowing the exception after logging it
            }
        }
    }
}
