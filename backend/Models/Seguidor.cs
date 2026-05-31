using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NzolaNet.API.Models
{
    public class Seguidor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SeguidorId { get; set; }

        [ForeignKey("SeguidorId")]
        public Utilizador SeguidorUser { get; set; } = null!;

        [Required]
        public int SeguidoId { get; set; }

        [ForeignKey("SeguidoId")]
        public Utilizador SeguidoUser { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
