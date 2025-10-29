using GestaoEstoque.Models;
using GestaoEstoque.Models.Enums;

namespace GestaoEstoque.Services;

/// <summary>
/// Interface para o serviço de Movimentação de Estoque.
/// </summary>
public interface IMovimentacaoEstoqueService
{
    Task<MovimentacaoEstoque> RegistrarEntradaAsync(long produtoId, int quantidade, string? lote = null, DateTime? dataValidade = null, string? observacao = null);
    Task<MovimentacaoEstoque> RegistrarSaidaAsync(long produtoId, int quantidade, string? observacao = null);
    Task<IEnumerable<MovimentacaoEstoque>> ObterTodasAsync();
    Task<IEnumerable<MovimentacaoEstoque>> ObterPorProdutoAsync(long produtoId);
    Task<IEnumerable<MovimentacaoEstoque>> ObterPorTipoAsync(TipoMovimentacao tipo);
    Task<IEnumerable<MovimentacaoEstoque>> ObterPorPeriodoAsync(DateTime dataInicio, DateTime dataFim);
}
