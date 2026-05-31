using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NzolaNet.API.Models
{
    public class Publicacao
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UtilizadorId { get; set; }

        [ForeignKey("UtilizadorId")]
        public Utilizador Utilizador { get; set; } = null!;

        [Required]
        public string Texto { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Imagem { get; set; }

        [MaxLength(500)]
        public string? Video { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();
        public ICollection<Baze> Bazes { get; set; } = new List<Baze>();
    }
}
