using GestaoEstoque.Models;
using GestaoEstoque.Models.Enums;

namespace GestaoEstoque.Repositories;

/// <summary>
/// Interface para operações de repositório da entidade Produto.
/// </summary>
public interface IProdutoRepository
{
    Task<Produto?> GetByIdAsync(long id);
    Task<Produto?> GetByCodigoSkuAsync(string codigoSku);
    Task<IEnumerable<Produto>> GetAllAsync();
    Task<IEnumerable<Produto>> GetByCategoriaAsync(CategoriaProduto categoria);
    Task<IEnumerable<Produto>> GetProdutosComEstoqueBaixoAsync();
    Task<IEnumerable<Produto>> SearchByNomeAsync(string nome);
    Task<bool> ExistsByCodigoSkuAsync(string codigoSku);
    Task<Produto> AddAsync(Produto produto);
    Task UpdateAsync(Produto produto);
    Task DeleteAsync(long id);
}
