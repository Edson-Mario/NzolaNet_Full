using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NzolaNet.API.Models
{
    public class Utilizador
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Senha { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? FotoPerfil { get; set; }

        public string? Bio { get; set; }

        [MaxLength(20)]
        public string Privacidade { get; set; } = "publico";

        [MaxLength(20)]
        public string Role { get; set; } = "user";

        public bool IsActive { get; set; } = true;

        public DateTime? DataNascimento { get; set; }

        [MaxLength(200)]
        public string? Endereco { get; set; }

        [MaxLength(50)]
        public string? Nacionalidade { get; set; }

        [MaxLength(20)]
        public string? Sexo { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Publicacao> Publicacoes { get; set; } = new List<Publicacao>();
        public ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();
        public ICollection<Baze> Bazes { get; set; } = new List<Baze>();
        public ICollection<Notificacao> Notificacoes { get; set; } = new List<Notificacao>();
    }
}
