using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NzolaNet.API.Models
{
    public class Notificacao
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UtilizadorId { get; set; }

        [ForeignKey("UtilizadorId")]
        public Utilizador Utilizador { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Tipo { get; set; } = string.Empty;

        [Required]
        public string Mensagem { get; set; } = string.Empty;

        public bool Lida { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
