namespace NzolaNet.API.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? FotoPerfil { get; set; }
        public string? Bio { get; set; }
        public string Privacidade { get; set; } = "publico";
        public string Role { get; set; } = "user";
        public bool IsActive { get; set; }
        public DateTime? DataNascimento { get; set; }
        public string? Endereco { get; set; }
        public string? Nacionalidade { get; set; }
        public string? Sexo { get; set; }
        public int PublicacoesCount { get; set; }
        public int SeguidoresCount { get; set; }
        public int SeguindoCount { get; set; }
        public bool IsFollowing { get; set; }
    }

    public class UpdateProfileDto
    {
        public string? Nome { get; set; }
        public string? Bio { get; set; }
        public string? FotoPerfil { get; set; }
        public string? Privacidade { get; set; }
        public DateTime? DataNascimento { get; set; }
        public string? Endereco { get; set; }
        public string? Nacionalidade { get; set; }
        public string? Sexo { get; set; }
    }

    public class AdminUserDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime? DataNascimento { get; set; }
        public string? Endereco { get; set; }
        public string? Nacionalidade { get; set; }
        public string? Sexo { get; set; }
        public DateTime CreatedAt { get; set; }
        public int PublicacoesCount { get; set; }
        public int BazesCount { get; set; }
        public int SeguidoresCount { get; set; }
        public int SeguindoCount { get; set; }
    }
}
