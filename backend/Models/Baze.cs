using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NzolaNet.API.Models
{
    public class Baze
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PublicacaoId { get; set; }

        [ForeignKey("PublicacaoId")]
        public Publicacao Publicacao { get; set; } = null!;

        [Required]
        public int UtilizadorId { get; set; }

        [ForeignKey("UtilizadorId")]
        public Utilizador Utilizador { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
