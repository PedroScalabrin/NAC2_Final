using GestaoEstoque.Models;
using GestaoEstoque.Models.Enums;

namespace GestaoEstoque.Services;

/// <summary>
/// Interface para o serviço de Produto.
/// </summary>
public interface IProdutoService
{
    Task<Produto> CadastrarProdutoAsync(Produto produto);
    Task<Produto?> ObterPorIdAsync(long id);
    Task<Produto?> ObterPorCodigoSkuAsync(string codigoSku);
    Task<IEnumerable<Produto>> ObterTodosAsync();
    Task<IEnumerable<Produto>> ObterPorCategoriaAsync(CategoriaProduto categoria);
    Task<IEnumerable<Produto>> ObterProdutosComEstoqueBaixoAsync();
    Task<Produto> AtualizarProdutoAsync(Produto produto);
    Task DeletarProdutoAsync(long id);
}
