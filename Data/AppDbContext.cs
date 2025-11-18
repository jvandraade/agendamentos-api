using AgendamentosApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AgendamentosApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Agendamento> Agendamentos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Agendamento>(entity =>
            {
                entity.ToTable("agendamentos");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Nome)
                    .HasColumnName("nome")
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(e => e.Servico)
                    .HasColumnName("servico")
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(e => e.Data)
                    .HasColumnName("data")
                    .IsRequired();

                entity.Property(e => e.Hora)
                    .HasColumnName("hora")
                    .IsRequired();

                entity.Property(e => e.DataCriacao)
                    .HasColumnName("criado_em")
                    .HasColumnType("timestamp with time zone")
                    .IsRequired();

                // Índice para buscar por data
                entity.HasIndex(e => e.Data)
                    .HasDatabaseName("idx_agendamentos_data");
            });
        }
    }
}