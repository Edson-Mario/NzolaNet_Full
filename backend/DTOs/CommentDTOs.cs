using System.ComponentModel.DataAnnotations;

namespace NzolaNet.API.DTOs
{
    public class CreateCommentDto
    {
        [Required]
        public int PublicacaoId { get; set; }

        [Required]
        public string Texto { get; set; } = string.Empty;
    }

    public class CommentDto
    {
        public int Id { get; set; }
        public int PublicacaoId { get; set; }
        public int UtilizadorId { get; set; }
        public string AutorNome { get; set; } = string.Empty;
        public string? AutorFoto { get; set; }
        public string Texto { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class UpdateCommentDto
    {
        [Required]
        public string Texto { get; set; } = string.Empty;
    }
}
