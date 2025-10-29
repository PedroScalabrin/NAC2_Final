using Microsoft.EntityFrameworkCore;
using GestaoEstoque.Data;
using GestaoEstoque.Models;
using GestaoEstoque.Models.Enums;

namespace GestaoEstoque.Repositories;

/// <summary>
/// Repositório para operações de banco de dados da entidade MovimentacaoEstoque.
/// </summary>
public class MovimentacaoEstoqueRepository : IMovimentacaoEstoqueRepository
{
    private readonly AppDbContext _context;

    public MovimentacaoEstoqueRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<MovimentacaoEstoque?> GetByIdAsync(long id)
    {
        return await _context.MovimentacoesEstoque
            .Include(m => m.Produto)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<IEnumerable<MovimentacaoEstoque>> GetAllAsync()
    {
        return await _context.MovimentacoesEstoque
            .Include(m => m.Produto)
            .OrderByDescending(m => m.DataMovimentacao)
            .ToListAsync();
    }

    public async Task<IEnumerable<MovimentacaoEstoque>> GetByProdutoIdAsync(long produtoId)
    {
        return await _context.MovimentacoesEstoque
            .Include(m => m.Produto)
            .Where(m => m.ProdutoId == produtoId)
            .OrderByDescending(m => m.DataMovimentacao)
            .ToListAsync();
    }

    public async Task<IEnumerable<MovimentacaoEstoque>> GetByTipoAsync(TipoMovimentacao tipo)
    {
        return await _context.MovimentacoesEstoque
            .Include(m => m.Produto)
            .Where(m => m.Tipo == tipo)
            .OrderByDescending(m => m.DataMovimentacao)
            .ToListAsync();
    }

    public async Task<IEnumerable<MovimentacaoEstoque>> GetByLoteAsync(string lote)
    {
        return await _context.MovimentacoesEstoque
            .Include(m => m.Produto)
            .Where(m => m.Lote == lote)
            .ToListAsync();
    }

    public async Task<IEnumerable<MovimentacaoEstoque>> GetByPeriodoAsync(DateTime dataInicio, DateTime dataFim)
    {
        return await _context.MovimentacoesEstoque
            .Include(m => m.Produto)
            .Where(m => m.DataMovimentacao >= dataInicio && m.DataMovimentacao <= dataFim)
            .OrderByDescending(m => m.DataMovimentacao)
            .ToListAsync();
    }

    public async Task<IEnumerable<MovimentacaoEstoque>> GetProdutosProximosVencimentoAsync(int dias = 7)
    {
        var dataLimite = DateTime.Now.AddDays(dias);
        return await _context.MovimentacoesEstoque
            .Include(m => m.Produto)
            .Where(m => m.DataValidade != null && m.DataValidade <= dataLimite && m.DataValidade >= DateTime.Now)
            .OrderBy(m => m.DataValidade)
            .ToListAsync();
    }

    public async Task<IEnumerable<MovimentacaoEstoque>> GetProdutosVencidosAsync()
    {
        return await _context.MovimentacoesEstoque
            .Include(m => m.Produto)
            .Where(m => m.DataValidade != null && m.DataValidade < DateTime.Now)
            .OrderBy(m => m.DataValidade)
            .ToListAsync();
    }

    public async Task<MovimentacaoEstoque> AddAsync(MovimentacaoEstoque movimentacao)
    {
        _context.MovimentacoesEstoque.Add(movimentacao);
        await _context.SaveChangesAsync();
        return movimentacao;
    }

    public async Task UpdateAsync(MovimentacaoEstoque movimentacao)
    {
        _context.Entry(movimentacao).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var movimentacao = await _context.MovimentacoesEstoque.FindAsync(id);
        if (movimentacao != null)
        {
            _context.MovimentacoesEstoque.Remove(movimentacao);
            await _context.SaveChangesAsync();
        }
    }
}
