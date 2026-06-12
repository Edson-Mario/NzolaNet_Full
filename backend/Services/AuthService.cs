using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using NzolaNet.API.DTOs;
using NzolaNet.API.Interfaces;
using NzolaNet.API.Models;

namespace NzolaNet.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUtilizadorRepository _userRepo;
        private readonly IConfiguration _config;

        public AuthService(IUtilizadorRepository userRepo, IConfiguration config)
        {
            _userRepo = userRepo;
            _config = config;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            if (dto.Senha != dto.ConfirmarSenha)
                throw new InvalidOperationException("As senhas nao coincidem.");

            if (await _userRepo.EmailExistsAsync(dto.Email))
                throw new InvalidOperationException("Este email ja existe no sistema.");

            var user = new Utilizador
            {
                Nome = dto.Nome,
                Email = dto.Email,
                Senha = BCrypt.Net.BCrypt.HashPassword(dto.Senha),
                FotoPerfil = dto.FotoPerfil,
            };

            await _userRepo.CreateAsync(user);

            return new AuthResponseDto
            {
                Id = user.Id,
                Nome = user.Nome,
                Email = user.Email,
                FotoPerfil = user.FotoPerfil,
                Role = user.Role,
                IsActive = user.IsActive,
                Token = GenerateJwtToken(user.Id, user.Email, user.Role)
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userRepo.GetByEmailAsync(dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Senha, user.Senha))
                throw new UnauthorizedAccessException("Email ou senha invalidos.");

            if (!user.IsActive)
                throw new UnauthorizedAccessException("A sua conta foi desativada. Contacte o administrador.");

            return new AuthResponseDto
            {
                Id = user.Id,
                Nome = user.Nome,
                Email = user.Email,
                FotoPerfil = user.FotoPerfil,
                Role = user.Role,
                IsActive = user.IsActive,
                Token = GenerateJwtToken(user.Id, user.Email, user.Role)
            };
        }

        public string GenerateJwtToken(int userId, string email, string role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
