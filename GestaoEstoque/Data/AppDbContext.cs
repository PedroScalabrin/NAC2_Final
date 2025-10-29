using Microsoft.EntityFrameworkCore;
using GestaoEstoque.Models;
using GestaoEstoque.Models.Enums;

namespace GestaoEstoque.Data;

/// <summary>
/// Contexto do banco de dados para o sistema de gestão de estoque.
/// Configurado para usar banco de dados em memória (InMemory).
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Produto> Produtos { get; set; }
    public DbSet<MovimentacaoEstoque> MovimentacoesEstoque { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuração da entidade Produto
        modelBuilder.Entity<Produto>(entity =>
        {
            entity.HasKey(p => p.Id);
            
            entity.HasIndex(p => p.CodigoSku).IsUnique();

            entity.Property(p => p.Categoria)
                .HasConversion<string>();

            // Relacionamento um-para-muitos
            entity.HasMany(p => p.Movimentacoes)
                .WithOne(m => m.Produto)
                .HasForeignKey(m => m.ProdutoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuração da entidade MovimentacaoEstoque
        modelBuilder.Entity<MovimentacaoEstoque>(entity =>
        {
            entity.HasKey(m => m.Id);

            entity.Property(m => m.Tipo)
                .HasConversion<string>();
        });
    }
}
