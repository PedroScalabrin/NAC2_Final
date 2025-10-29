using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GestaoEstoque.Models.Enums;

namespace GestaoEstoque.Models;

/// <summary>
/// Entidade representando um Produto no sistema de gestão de estoque.
/// 
/// Regras de negócio:
/// - Código SKU deve ser único
/// - Preço unitário deve ser positivo
/// - Quantidade mínima em estoque não pode ser negativa
/// </summary>
[Table("Produtos")]
public class Produto
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required(ErrorMessage = "Código SKU é obrigatório")]
    [StringLength(50, ErrorMessage = "Código SKU deve ter no máximo 50 caracteres")]
    public string CodigoSku { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nome do produto é obrigatório")]
    [StringLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Categoria é obrigatória")]
    public CategoriaProduto Categoria { get; set; }

    [Required(ErrorMessage = "Preço unitário é obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Preço unitário deve ser maior que zero")]
    [Column(TypeName = "decimal(10,2)")]
    public decimal PrecoUnitario { get; set; }

    [Required(ErrorMessage = "Quantidade mínima em estoque é obrigatória")]
    [Range(0, int.MaxValue, ErrorMessage = "Quantidade mínima não pode ser negativa")]
    public int QuantidadeMinima { get; set; }

    public int QuantidadeAtual { get; set; } = 0;

    [Required]
    public DateTime DataCriacao { get; set; } = DateTime.Now;

    // Relacionamento com Movimentações
    public virtual ICollection<MovimentacaoEstoque> Movimentacoes { get; set; } = new List<MovimentacaoEstoque>();

    /// <summary>
    /// Verifica se o produto está abaixo da quantidade mínima (alerta de estoque baixo).
    /// </summary>
    public bool IsEstoqueBaixo() => QuantidadeAtual < QuantidadeMinima;

    /// <summary>
    /// Verifica se é um produto perecível.
    /// </summary>
    public bool IsPerecivel() => Categoria == CategoriaProduto.PERECIVEL;

    /// <summary>
    /// Adiciona quantidade ao estoque atual.
    /// </summary>
    public void AdicionarEstoque(int quantidade) => QuantidadeAtual += quantidade;

    /// <summary>
    /// Remove quantidade do estoque atual.
    /// </summary>
    public void RemoverEstoque(int quantidade) => QuantidadeAtual -= quantidade;

    /// <summary>
    /// Verifica se há estoque suficiente para retirada.
    /// </summary>
    public bool TemEstoqueSuficiente(int quantidade) => QuantidadeAtual >= quantidade;
}
