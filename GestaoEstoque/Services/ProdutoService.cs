using GestaoEstoque.Models;
using GestaoEstoque.Models.Enums;
using GestaoEstoque.Repositories;
using GestaoEstoque.Exceptions;

namespace GestaoEstoque.Services;

/// <summary>
/// Serviço de Produto - 2.0 pontos
/// - Implementar validação de categoria vs dados obrigatórios
/// - Criar método para verificar produtos abaixo do estoque mínimo
/// - Cadastro completo de produtos
/// </summary>
public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _repository;

    public ProdutoService(IProdutoRepository repository)
    {
        _repository = repository;
    }

    public async Task<Produto> CadastrarProdutoAsync(Produto produto)
    {
        // Validação de categoria vs dados obrigatórios
        ValidarProduto(produto);

        // Verifica se SKU já existe
        if (await _repository.ExistsByCodigoSkuAsync(produto.CodigoSku))
        {
            throw new ValidationException($"Código SKU '{produto.CodigoSku}' já cadastrado");
        }

        produto.DataCriacao = DateTime.Now;
        produto.QuantidadeAtual = 0;

        return await _repository.AddAsync(produto);
    }

    public async Task<Produto?> ObterPorIdAsync(long id)
    {
        var produto = await _repository.GetByIdAsync(id);
        if (produto == null)
        {
            throw new NotFoundException($"Produto com ID {id} não encontrado");
        }
        return produto;
    }

    public async Task<Produto?> ObterPorCodigoSkuAsync(string codigoSku)
    {
        return await _repository.GetByCodigoSkuAsync(codigoSku);
    }

    public async Task<IEnumerable<Produto>> ObterTodosAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<IEnumerable<Produto>> ObterPorCategoriaAsync(CategoriaProduto categoria)
    {
        return await _repository.GetByCategoriaAsync(categoria);
    }

    /// <summary>
    /// Método para verificar produtos abaixo do estoque mínimo (Alerta)
    /// </summary>
    public async Task<IEnumerable<Produto>> ObterProdutosComEstoqueBaixoAsync()
    {
        return await _repository.GetProdutosComEstoqueBaixoAsync();
    }

    public async Task<Produto> AtualizarProdutoAsync(Produto produto)
    {
        ValidarProduto(produto);

        var produtoExistente = await _repository.GetByIdAsync(produto.Id);
        if (produtoExistente == null)
        {
            throw new NotFoundException($"Produto com ID {produto.Id} não encontrado");
        }

        await _repository.UpdateAsync(produto);
        return produto;
    }

    public async Task DeletarProdutoAsync(long id)
    {
        var produto = await _repository.GetByIdAsync(id);
        if (produto == null)
        {
            throw new NotFoundException($"Produto com ID {id} não encontrado");
        }

        await _repository.DeleteAsync(id);
    }

    /// <summary>
    /// Validação de categoria vs dados obrigatórios
    /// </summary>
    private void ValidarProduto(Produto produto)
    {
        if (string.IsNullOrWhiteSpace(produto.CodigoSku))
        {
            throw new ValidationException("Código SKU é obrigatório");
        }

        if (string.IsNullOrWhiteSpace(produto.Nome))
        {
            throw new ValidationException("Nome do produto é obrigatório");
        }

        if (produto.PrecoUnitario <= 0)
        {
            throw new ValidationException("Preço unitário deve ser maior que zero");
        }

        if (produto.QuantidadeMinima < 0)
        {
            throw new ValidationException("Quantidade mínima não pode ser negativa");
        }
    }
}
