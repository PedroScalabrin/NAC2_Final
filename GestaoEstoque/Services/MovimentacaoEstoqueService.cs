using GestaoEstoque.Models;
using GestaoEstoque.Models.Enums;
using GestaoEstoque.Repositories;
using GestaoEstoque.Exceptions;

namespace GestaoEstoque.Services;

/// <summary>
/// Serviço de Movimentação - 2.5 pontos
/// - Validar quantidade positiva
/// - Verificar estoque suficiente para saídas
/// - Atualizar saldo do produto automaticamente
/// - Validar data de validade para perecíveis
/// - Implementar entrada e saída de estoque
/// </summary>
public class MovimentacaoEstoqueService : IMovimentacaoEstoqueService
{
    private readonly IMovimentacaoEstoqueRepository _movimentacaoRepository;
    private readonly IProdutoRepository _produtoRepository;

    public MovimentacaoEstoqueService(
        IMovimentacaoEstoqueRepository movimentacaoRepository,
        IProdutoRepository produtoRepository)
    {
        _movimentacaoRepository = movimentacaoRepository;
        _produtoRepository = produtoRepository;
    }

    /// <summary>
    /// Implementar entrada de estoque
    /// </summary>
    public async Task<MovimentacaoEstoque> RegistrarEntradaAsync(
        long produtoId, 
        int quantidade, 
        string? lote = null, 
        DateTime? dataValidade = null, 
        string? observacao = null)
    {
        // Validar quantidade positiva
        if (quantidade <= 0)
        {
            throw new ValidationException("Quantidade deve ser maior que zero");
        }

        var produto = await _produtoRepository.GetByIdAsync(produtoId);
        if (produto == null)
        {
            throw new NotFoundException($"Produto com ID {produtoId} não encontrado");
        }

        var movimentacao = new MovimentacaoEstoque
        {
            ProdutoId = produtoId,
            Produto = produto,
            Tipo = TipoMovimentacao.ENTRADA,
            Quantidade = quantidade,
            Lote = lote,
            DataValidade = dataValidade,
            Observacao = observacao,
            DataMovimentacao = DateTime.Now
        };

        // Validar data de validade para perecíveis
        ValidarMovimentacao(movimentacao);

        // Atualizar saldo do produto automaticamente
        produto.AdicionarEstoque(quantidade);
        await _produtoRepository.UpdateAsync(produto);

        return await _movimentacaoRepository.AddAsync(movimentacao);
    }

    /// <summary>
    /// Implementar saída de estoque
    /// </summary>
    public async Task<MovimentacaoEstoque> RegistrarSaidaAsync(
        long produtoId, 
        int quantidade, 
        string? observacao = null)
    {
        // Validar quantidade positiva
        if (quantidade <= 0)
        {
            throw new ValidationException("Quantidade deve ser maior que zero");
        }

        var produto = await _produtoRepository.GetByIdAsync(produtoId);
        if (produto == null)
        {
            throw new NotFoundException($"Produto com ID {produtoId} não encontrado");
        }

        // Verificar estoque suficiente para saídas
        if (!produto.TemEstoqueSuficiente(quantidade))
        {
            throw new EstoqueException(
                $"Estoque insuficiente. Disponível: {produto.QuantidadeAtual}, Solicitado: {quantidade}");
        }

        var movimentacao = new MovimentacaoEstoque
        {
            ProdutoId = produtoId,
            Produto = produto,
            Tipo = TipoMovimentacao.SAIDA,
            Quantidade = quantidade,
            Observacao = observacao,
            DataMovimentacao = DateTime.Now
        };

        // Atualizar saldo do produto automaticamente
        produto.RemoverEstoque(quantidade);
        await _produtoRepository.UpdateAsync(produto);

        return await _movimentacaoRepository.AddAsync(movimentacao);
    }

    public async Task<IEnumerable<MovimentacaoEstoque>> ObterTodasAsync()
    {
        return await _movimentacaoRepository.GetAllAsync();
    }

    public async Task<IEnumerable<MovimentacaoEstoque>> ObterPorProdutoAsync(long produtoId)
    {
        return await _movimentacaoRepository.GetByProdutoIdAsync(produtoId);
    }

    public async Task<IEnumerable<MovimentacaoEstoque>> ObterPorTipoAsync(TipoMovimentacao tipo)
    {
        return await _movimentacaoRepository.GetByTipoAsync(tipo);
    }

    public async Task<IEnumerable<MovimentacaoEstoque>> ObterPorPeriodoAsync(DateTime dataInicio, DateTime dataFim)
    {
        return await _movimentacaoRepository.GetByPeriodoAsync(dataInicio, dataFim);
    }

    /// <summary>
    /// Validação completa da movimentação
    /// </summary>
    private void ValidarMovimentacao(MovimentacaoEstoque movimentacao)
    {
        if (movimentacao.Produto == null)
        {
            throw new ValidationException("Produto não pode ser nulo");
        }

        // Validar data de validade para perecíveis
        if (movimentacao.Produto.IsPerecivel())
        {
            if (string.IsNullOrWhiteSpace(movimentacao.Lote))
            {
                throw new ValidationException("Lote é obrigatório para produtos perecíveis");
            }

            if (movimentacao.DataValidade == null)
            {
                throw new ValidationException("Data de validade é obrigatória para produtos perecíveis");
            }

            if (movimentacao.DataValidade.Value.Date < DateTime.Now.Date)
            {
                throw new ValidationException("Data de validade não pode ser anterior à data atual");
            }

            // Produto perecível não pode ter movimentação após data de validade
            if (movimentacao.DataValidade.Value.Date < DateTime.Now.Date)
            {
                throw new ValidationException("Não é permitido movimentar produto com data de validade vencida");
            }
        }
    }
}
