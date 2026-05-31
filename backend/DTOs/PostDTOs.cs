using System.ComponentModel.DataAnnotations;

namespace NzolaNet.API.DTOs
{
    public class CreatePostDto
    {
        [Required]
        public string Texto { get; set; } = string.Empty;

        public string? Imagem { get; set; }
        public string? Video { get; set; }
    }

    public class UpdatePostDto
    {
        public string? Texto { get; set; }
        public string? Imagem { get; set; }
        public string? Video { get; set; }
    }

    public class PostDto
    {
        public int Id { get; set; }
        public int UtilizadorId { get; set; }
        public string AutorNome { get; set; } = string.Empty;
        public string? AutorFoto { get; set; }
        public string Texto { get; set; } = string.Empty;
        public string? Imagem { get; set; }
        public string? Video { get; set; }
        public int BazesCount { get; set; }
        public int ComentariosCount { get; set; }
        public bool IsBazed { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
