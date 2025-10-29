using Microsoft.EntityFrameworkCore;
using GestaoEstoque.Data;
using GestaoEstoque.Models;
using GestaoEstoque.Models.Enums;

namespace GestaoEstoque.Repositories;

/// <summary>
/// Repositório para operações de banco de dados da entidade Produto.
/// </summary>
public class ProdutoRepository : IProdutoRepository
{
    private readonly AppDbContext _context;

    public ProdutoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Produto?> GetByIdAsync(long id)
    {
        return await _context.Produtos
            .Include(p => p.Movimentacoes)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Produto?> GetByCodigoSkuAsync(string codigoSku)
    {
        return await _context.Produtos
            .Include(p => p.Movimentacoes)
            .FirstOrDefaultAsync(p => p.CodigoSku == codigoSku);
    }

    public async Task<IEnumerable<Produto>> GetAllAsync()
    {
        return await _context.Produtos
            .Include(p => p.Movimentacoes)
            .ToListAsync();
    }

    public async Task<IEnumerable<Produto>> GetByCategoriaAsync(CategoriaProduto categoria)
    {
        return await _context.Produtos
            .Where(p => p.Categoria == categoria)
            .ToListAsync();
    }

    public async Task<IEnumerable<Produto>> GetProdutosComEstoqueBaixoAsync()
    {
        return await _context.Produtos
            .Where(p => p.QuantidadeAtual < p.QuantidadeMinima)
            .ToListAsync();
    }

    public async Task<IEnumerable<Produto>> SearchByNomeAsync(string nome)
    {
        return await _context.Produtos
            .Where(p => p.Nome.ToLower().Contains(nome.ToLower()))
            .ToListAsync();
    }

    public async Task<bool> ExistsByCodigoSkuAsync(string codigoSku)
    {
        return await _context.Produtos.AnyAsync(p => p.CodigoSku == codigoSku);
    }

    public async Task<Produto> AddAsync(Produto produto)
    {
        _context.Produtos.Add(produto);
        await _context.SaveChangesAsync();
        return produto;
    }

    public async Task UpdateAsync(Produto produto)
    {
        _context.Entry(produto).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var produto = await _context.Produtos.FindAsync(id);
        if (produto != null)
        {
            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();
        }
    }
}
