using GestaoEstoque.Models;
using GestaoEstoque.Models.Enums;

namespace GestaoEstoque.Repositories;

/// <summary>
/// Interface para operações de repositório da entidade MovimentacaoEstoque.
/// </summary>
public interface IMovimentacaoEstoqueRepository
{
    Task<MovimentacaoEstoque?> GetByIdAsync(long id);
    Task<IEnumerable<MovimentacaoEstoque>> GetAllAsync();
    Task<IEnumerable<MovimentacaoEstoque>> GetByProdutoIdAsync(long produtoId);
    Task<IEnumerable<MovimentacaoEstoque>> GetByTipoAsync(TipoMovimentacao tipo);
    Task<IEnumerable<MovimentacaoEstoque>> GetByLoteAsync(string lote);
    Task<IEnumerable<MovimentacaoEstoque>> GetByPeriodoAsync(DateTime dataInicio, DateTime dataFim);
    Task<IEnumerable<MovimentacaoEstoque>> GetProdutosProximosVencimentoAsync(int dias = 7);
    Task<IEnumerable<MovimentacaoEstoque>> GetProdutosVencidosAsync();
    Task<MovimentacaoEstoque> AddAsync(MovimentacaoEstoque movimentacao);
    Task UpdateAsync(MovimentacaoEstoque movimentacao);
    Task DeleteAsync(long id);
}
