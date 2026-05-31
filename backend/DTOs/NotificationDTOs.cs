namespace NzolaNet.API.DTOs
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public int UtilizadorId { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Mensagem { get; set; } = string.Empty;
        public bool Lida { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
