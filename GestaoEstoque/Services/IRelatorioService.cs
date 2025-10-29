using GestaoEstoque.Models;

namespace GestaoEstoque.Services;

/// <summary>
/// Interface para o serviço de Relatórios.
/// </summary>
public interface IRelatorioService
{
    Task<decimal> CalcularValorTotalEstoqueAsync();
    Task<IEnumerable<MovimentacaoEstoque>> ListarProdutosProximosVencimentoAsync(int dias = 7);
    Task<IEnumerable<Produto>> IdentificarProdutosEstoqueBaixoAsync();
    Task<Dictionary<string, object>> ObterDashboardAsync();
}
