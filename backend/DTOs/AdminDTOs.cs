namespace NzolaNet.API.DTOs
{
    public class DashboardStatsDto
    {
        public int TotalUtilizadores { get; set; }
        public int TotalPublicacoes { get; set; }
        public int TotalComentarios { get; set; }
        public int TotalBazes { get; set; }
        public int UtilizadoresAtivos { get; set; }
        public int UtilizadoresDesativados { get; set; }
    }

    public class TopBazesUserDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? FotoPerfil { get; set; }
        public int BazesCount { get; set; }
        public int PublicacoesCount { get; set; }
    }

    public class PostListItemDto
    {
        public int Id { get; set; }
        public string AutorNome { get; set; } = string.Empty;
        public string? AutorFoto { get; set; }
        public string Texto { get; set; } = string.Empty;
        public int BazesCount { get; set; }
        public int ComentariosCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CommentListItemDto
    {
        public int Id { get; set; }
        public int PublicacaoId { get; set; }
        public string AutorNome { get; set; } = string.Empty;
        public string Texto { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
