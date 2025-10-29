using GestaoEstoque.Models;
using GestaoEstoque.Repositories;

namespace GestaoEstoque.Services;

/// <summary>
/// Serviço de Relatórios - 0.5 pontos
/// - Calcular valor total do estoque (quantidade × preço)
/// - Listar produtos que vencerão em até 7 dias
/// - Identificar produtos com estoque abaixo do mínimo
/// </summary>
public class RelatorioService : IRelatorioService
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IMovimentacaoEstoqueRepository _movimentacaoRepository;

    public RelatorioService(
        IProdutoRepository produtoRepository,
        IMovimentacaoEstoqueRepository movimentacaoRepository)
    {
        _produtoRepository = produtoRepository;
        _movimentacaoRepository = movimentacaoRepository;
    }

    /// <summary>
    /// Calcular valor total do estoque (quantidade × preço)
    /// </summary>
    public async Task<decimal> CalcularValorTotalEstoqueAsync()
    {
        var produtos = await _produtoRepository.GetAllAsync();
        return produtos.Sum(p => p.QuantidadeAtual * p.PrecoUnitario);
    }

    /// <summary>
    /// Listar produtos que vencerão em até 7 dias
    /// </summary>
    public async Task<IEnumerable<MovimentacaoEstoque>> ListarProdutosProximosVencimentoAsync(int dias = 7)
    {
        return await _movimentacaoRepository.GetProdutosProximosVencimentoAsync(dias);
    }

    /// <summary>
    /// Identificar produtos com estoque abaixo do mínimo
    /// </summary>
    public async Task<IEnumerable<Produto>> IdentificarProdutosEstoqueBaixoAsync()
    {
        return await _produtoRepository.GetProdutosComEstoqueBaixoAsync();
    }

    /// <summary>
    /// Obtém um dashboard consolidado com todas as informações importantes
    /// </summary>
    public async Task<Dictionary<string, object>> ObterDashboardAsync()
    {
        var valorTotal = await CalcularValorTotalEstoqueAsync();
        var produtosEstoqueBaixo = await IdentificarProdutosEstoqueBaixoAsync();
        var produtosProximosVencimento = await ListarProdutosProximosVencimentoAsync();
        var produtosVencidos = await _movimentacaoRepository.GetProdutosVencidosAsync();
        var totalProdutos = (await _produtoRepository.GetAllAsync()).Count();

        return new Dictionary<string, object>
        {
            { "valorTotalEstoque", valorTotal },
            { "totalProdutos", totalProdutos },
            { "produtosEstoqueBaixo", produtosEstoqueBaixo.Count() },
            { "produtosProximosVencimento", produtosProximosVencimento.Count() },
            { "produtosVencidos", produtosVencidos.Count() },
            { "alertas", new 
                {
                    estoqueBaixo = produtosEstoqueBaixo.Select(p => new { p.CodigoSku, p.Nome, p.QuantidadeAtual, p.QuantidadeMinima }),
                    proximosVencimento = produtosProximosVencimento.Select(m => new { m.Produto?.Nome, m.Lote, m.DataValidade }),
                    vencidos = produtosVencidos.Select(m => new { m.Produto?.Nome, m.Lote, m.DataValidade })
                }
            }
        };
    }
}
