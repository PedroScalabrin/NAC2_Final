using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GestaoEstoque.Models.Enums;

namespace GestaoEstoque.Models;

/// <summary>
/// Entidade representando uma Movimentação de Estoque.
/// 
/// Regras de negócio:
/// - Produtos perecíveis devem ter lote e data de validade
/// - Não é permitido quantidade negativa
/// - Ao registrar saída, deve verificar estoque suficiente
/// </summary>
[Table("MovimentacoesEstoque")]
public class MovimentacaoEstoque
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required(ErrorMessage = "Produto é obrigatório")]
    public long ProdutoId { get; set; }

    [Required(ErrorMessage = "Tipo de movimentação é obrigatório")]
    public TipoMovimentacao Tipo { get; set; }

    [Required(ErrorMessage = "Quantidade é obrigatória")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero")]
    public int Quantidade { get; set; }

    [Required]
    public DateTime DataMovimentacao { get; set; } = DateTime.Now;

    [StringLength(100, ErrorMessage = "Lote deve ter no máximo 100 caracteres")]
    public string? Lote { get; set; }

    public DateTime? DataValidade { get; set; }

    [StringLength(500, ErrorMessage = "Observação deve ter no máximo 500 caracteres")]
    public string? Observacao { get; set; }

    // Relacionamento
    [ForeignKey("ProdutoId")]
    public virtual Produto? Produto { get; set; }

    /// <summary>
    /// Valida se a movimentação está conforme as regras de negócio.
    /// </summary>
    public void Validar()
    {
        if (Produto == null)
            throw new InvalidOperationException("Produto não pode ser nulo para validação");

        // Valida quantidade
        if (Quantidade <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero");

        // Valida lote e data de validade para produtos perecíveis
        if (Produto.IsPerecivel())
        {
            if (string.IsNullOrWhiteSpace(Lote))
                throw new ArgumentException("Lote é obrigatório para produtos perecíveis");

            if (DataValidade == null)
                throw new ArgumentException("Data de validade é obrigatória para produtos perecíveis");

            if (DataValidade.Value.Date < DateTime.Now.Date)
                throw new ArgumentException("Data de validade não pode ser anterior à data atual");
        }

        // Valida estoque suficiente para saída
        if (Tipo == TipoMovimentacao.SAIDA && !Produto.TemEstoqueSuficiente(Quantidade))
        {
            throw new InvalidOperationException(
                $"Estoque insuficiente. Disponível: {Produto.QuantidadeAtual}, Solicitado: {Quantidade}");
        }
    }

    /// <summary>
    /// Verifica se o produto está próximo do vencimento (dentro de 7 dias).
    /// </summary>
    public bool IsProximoVencimento()
    {
        if (DataValidade == null) return false;
        var dataLimite = DateTime.Now.AddDays(7);
        return DataValidade.Value.Date <= dataLimite.Date;
    }

    /// <summary>
    /// Verifica se o produto está vencido.
    /// </summary>
    public bool IsVencido()
    {
        if (DataValidade == null) return false;
        return DataValidade.Value.Date < DateTime.Now.Date;
    }
}
