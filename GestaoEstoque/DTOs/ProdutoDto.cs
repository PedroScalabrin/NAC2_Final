using System.ComponentModel.DataAnnotations;
using GestaoEstoque.Models.Enums;

namespace GestaoEstoque.DTOs;

public class ProdutoRequestDto
{
    [Required(ErrorMessage = "Código SKU é obrigatório")]
    [StringLength(50)]
    public string CodigoSku { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(200)]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Categoria é obrigatória")]
    public CategoriaProduto Categoria { get; set; }

    [Required(ErrorMessage = "Preço unitário é obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Preço deve ser maior que zero")]
    public decimal PrecoUnitario { get; set; }

    [Required(ErrorMessage = "Quantidade mínima é obrigatória")]
    [Range(0, int.MaxValue, ErrorMessage = "Quantidade mínima não pode ser negativa")]
    public int QuantidadeMinima { get; set; }
}

public class ProdutoResponseDto
{
    public long Id { get; set; }
    public string CodigoSku { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public decimal PrecoUnitario { get; set; }
    public int QuantidadeMinima { get; set; }
    public int QuantidadeAtual { get; set; }
    public DateTime DataCriacao { get; set; }
    public bool EstoqueBaixo { get; set; }
    public decimal ValorTotalEstoque { get; set; }
}
