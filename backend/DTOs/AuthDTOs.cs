using System.ComponentModel.DataAnnotations;

namespace NzolaNet.API.DTOs
{
    public class RegisterDto
    {
        [Required]
        [MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Senha { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string ConfirmarSenha { get; set; } = string.Empty;

        public string? FotoPerfil { get; set; }
    }

    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Senha { get; set; } = string.Empty;
    }

    public class AuthResponseDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? FotoPerfil { get; set; }
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string Token { get; set; } = string.Empty;
    }

    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }

    public class ChangeEmailDto
    {
        [Required]
        [EmailAddress]
        public string NovoEmail { get; set; } = string.Empty;
    }

    public class ChangePasswordDto
    {
        [Required]
        public string SenhaAtual { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string NovaSenha { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string ConfirmarSenha { get; set; } = string.Empty;
    }

    public class CreateAdminDto
    {
        [Required]
        [MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Senha { get; set; } = string.Empty;
    }
}
