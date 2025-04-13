using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.DTO;
using TodoApp.DTOs;
using TodoApp.Models;
using TodoApp.Services.Interfaces;
/// <summary>
/// Handles authentication-related endpoints such as registration, login, and token refresh.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IJwtTokenService _jwtService;
    private readonly AppDbContext _context;

    public AuthController(IUserService userService, IJwtTokenService jwtService, AppDbContext context)
    {
        _userService = userService;
        _jwtService = jwtService;
        _context = context;
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="dto">User registration details.</param>
    /// <returns>Confirmation message and user ID.</returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegisterDto dto)
    {
        var user = await _userService.RegisterAsync(dto);
        return Ok(new { message = "User registered", user.Id });
    }

    /// <summary>
    /// Logs a user into the system.
    /// </summary>
    /// <param name="dto">User login credentials.</param>
    /// <returns>JWT token for authentication.</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginDto dto)
    {
        (User user, string token, string refreshToken) user = await _userService.LoginAsync(dto);
        var token = _jwtService.GenerateToken(user.user);
        return Ok(new { token });
    }

    /// <summary>
    /// Refreshes the JWT token using a valid refresh token.
    /// </summary>
    /// <param name="dto">Contains the user's email and refresh token.</param>
    /// <returns>New JWT token and refresh token.</returns>
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u =>
            u.Email == dto.Email && u.RefreshToken == dto.RefreshToken);

        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            return Unauthorized("Invalid or expired refresh token");

        var newToken = _jwtService.GenerateToken(user);
        var newRefreshToken = _jwtService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _context.SaveChangesAsync();

        return Ok(new { token = newToken, refreshToken = newRefreshToken });
    }
}
