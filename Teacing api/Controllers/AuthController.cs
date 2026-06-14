using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Teacing_api.Models;
using Teacing_api.Registr;

namespace Teacing_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        // Внедряем и контекст базы данных, и конфигурацию JWT
        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // 1. ЭНДПОИНТ РЕГИСТРАЦИИ
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            // Проверяем, нет ли уже пользователя с таким именем
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
            {
                return BadRequest(new { Message = "Пользователь с таким логином уже существует!" });
            }

            // МАГИЯ КРИПТОГРАФИИ: превращаем "12345" в хэш типа "$2a$11$ПотокСлучайныхСимволов..."
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // Создаем объект для базы данных
            var user = new User
            {
                Username = dto.Username,
                PasswordHash = passwordHash,
                Role = dto.Role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Регистрация прошла успешно!" });
        }

        // 2. ЭНДПОИНТ ВХОДА (ЛОГИН)
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            // Ищем пользователя в базе данных по имени
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);

            // Если пользователь не найден
            if (user == null)
            {
                return Unauthorized(new { Message = "Неверный логин или пароль!" });
            }

            // ПРОВЕРКА ПАРОЛЯ: BCrypt сам берет введенный пароль, хэширует его и сравнивает с тем, что в базе
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            if (!isPasswordValid)
            {
                return Unauthorized(new { Message = "Неверный логин или пароль!" });
            }

            // Если всё ок — собираем токен, используя данные НАСТОЯЩЕГО пользователя из базы
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role), // Роль теперь берется из БД!
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { Token = tokenString });
        }
    }
}