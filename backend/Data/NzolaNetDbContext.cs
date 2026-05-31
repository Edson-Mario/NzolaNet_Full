using Microsoft.EntityFrameworkCore;
using NzolaNet.API.Models;

namespace NzolaNet.API.Data
{
    public class NzolaNetDbContext : DbContext
    {
        public NzolaNetDbContext(DbContextOptions<NzolaNetDbContext> options) : base(options) { }

        public DbSet<Utilizador> Utilizadores => Set<Utilizador>();
        public DbSet<Publicacao> Publicacoes => Set<Publicacao>();
        public DbSet<Comentario> Comentarios => Set<Comentario>();
        public DbSet<Baze> Bazes => Set<Baze>();
        public DbSet<Seguidor> Seguidores => Set<Seguidor>();
        public DbSet<Notificacao> Notificacoes => Set<Notificacao>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Utilizador>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Privacidade).HasDefaultValue("publico");
                entity.Property(e => e.Role).HasDefaultValue("user");
            });

            modelBuilder.Entity<Baze>(entity =>
            {
                entity.HasIndex(e => new { e.PublicacaoId, e.UtilizadorId }).IsUnique();
            });

            modelBuilder.Entity<Seguidor>(entity =>
            {
                entity.HasIndex(e => new { e.SeguidorId, e.SeguidoId }).IsUnique();
                entity.HasOne(e => e.SeguidorUser)
                    .WithMany()
                    .HasForeignKey(e => e.SeguidorId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.SeguidoUser)
                    .WithMany()
                    .HasForeignKey(e => e.SeguidoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Publicacao>(entity =>
            {
                entity.HasOne(e => e.Utilizador)
                    .WithMany(u => u.Publicacoes)
                    .HasForeignKey(e => e.UtilizadorId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Comentario>(entity =>
            {
                entity.HasOne(e => e.Publicacao)
                    .WithMany(p => p.Comentarios)
                    .HasForeignKey(e => e.PublicacaoId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Utilizador)
                    .WithMany(u => u.Comentarios)
                    .HasForeignKey(e => e.UtilizadorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Baze>(entity =>
            {
                entity.HasOne(e => e.Publicacao)
                    .WithMany(p => p.Bazes)
                    .HasForeignKey(e => e.PublicacaoId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Utilizador)
                    .WithMany(u => u.Bazes)
                    .HasForeignKey(e => e.UtilizadorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Notificacao>(entity =>
            {
                entity.HasOne(e => e.Utilizador)
                    .WithMany(u => u.Notificacoes)
                    .HasForeignKey(e => e.UtilizadorId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
