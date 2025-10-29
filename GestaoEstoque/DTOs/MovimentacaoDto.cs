using System.ComponentModel.DataAnnotations;

namespace GestaoEstoque.DTOs;

public class EntradaEstoqueRequestDto
{
    [Required(ErrorMessage = "ID do produto é obrigatório")]
    public long ProdutoId { get; set; }

    [Required(ErrorMessage = "Quantidade é obrigatória")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero")]
    public int Quantidade { get; set; }

    public string? Lote { get; set; }

    public DateTime? DataValidade { get; set; }

    public string? Observacao { get; set; }
}

public class SaidaEstoqueRequestDto
{
    [Required(ErrorMessage = "ID do produto é obrigatório")]
    public long ProdutoId { get; set; }

    [Required(ErrorMessage = "Quantidade é obrigatória")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero")]
    public int Quantidade { get; set; }

    public string? Observacao { get; set; }
}

public class MovimentacaoResponseDto
{
    public long Id { get; set; }
    public long ProdutoId { get; set; }
    public string? ProdutoNome { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public DateTime DataMovimentacao { get; set; }
    public string? Lote { get; set; }
    public DateTime? DataValidade { get; set; }
    public string? Observacao { get; set; }
    public bool ProximoVencimento { get; set; }
    public bool Vencido { get; set; }
}
