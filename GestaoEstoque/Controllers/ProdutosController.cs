using Microsoft.AspNetCore.Mvc;
using GestaoEstoque.Services;
using GestaoEstoque.DTOs;
using GestaoEstoque.Models;
using GestaoEstoque.Models.Enums;
using GestaoEstoque.Exceptions;

namespace GestaoEstoque.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoService _produtoService;

    public ProdutosController(IProdutoService produtoService)
    {
        _produtoService = produtoService;
    }

    /// <summary>
    /// Lista todos os produtos
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProdutoResponseDto>>> GetAll()
    {
        try
        {
            var produtos = await _produtoService.ObterTodosAsync();
            var response = produtos.Select(MapToDto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao buscar produtos", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtém um produto por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProdutoResponseDto>> GetById(long id)
    {
        try
        {
            var produto = await _produtoService.ObterPorIdAsync(id);
            return produto == null ? NotFound() : Ok(MapToDto(produto));
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao buscar produto", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtém um produto por Código SKU
    /// </summary>
    [HttpGet("sku/{codigoSku}")]
    public async Task<ActionResult<ProdutoResponseDto>> GetBySku(string codigoSku)
    {
        try
        {
            var produto = await _produtoService.ObterPorCodigoSkuAsync(codigoSku);
            return produto == null ? NotFound() : Ok(MapToDto(produto));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao buscar produto", error = ex.Message });
        }
    }

    /// <summary>
    /// Lista produtos por categoria
    /// </summary>
    [HttpGet("categoria/{categoria}")]
    public async Task<ActionResult<IEnumerable<ProdutoResponseDto>>> GetByCategoria(CategoriaProduto categoria)
    {
        try
        {
            var produtos = await _produtoService.ObterPorCategoriaAsync(categoria);
            var response = produtos.Select(MapToDto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao buscar produtos", error = ex.Message });
        }
    }

    /// <summary>
    /// Lista produtos com estoque abaixo do mínimo (ALERTA)
    /// </summary>
    [HttpGet("estoque-baixo")]
    public async Task<ActionResult<IEnumerable<ProdutoResponseDto>>> GetEstoqueBaixo()
    {
        try
        {
            var produtos = await _produtoService.ObterProdutosComEstoqueBaixoAsync();
            var response = produtos.Select(MapToDto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao buscar produtos", error = ex.Message });
        }
    }

    /// <summary>
    /// Cadastra um novo produto
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ProdutoResponseDto>> Create([FromBody] ProdutoRequestDto request)
    {
        try
        {
            var produto = new Produto
            {
                CodigoSku = request.CodigoSku,
                Nome = request.Nome,
                Categoria = request.Categoria,
                PrecoUnitario = request.PrecoUnitario,
                QuantidadeMinima = request.QuantidadeMinima
            };

            var novoProduto = await _produtoService.CadastrarProdutoAsync(produto);
            return CreatedAtAction(nameof(GetById), new { id = novoProduto.Id }, MapToDto(novoProduto));
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao cadastrar produto", error = ex.Message });
        }
    }

    /// <summary>
    /// Atualiza um produto existente
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ProdutoResponseDto>> Update(long id, [FromBody] ProdutoRequestDto request)
    {
        try
        {
            var produto = await _produtoService.ObterPorIdAsync(id);
            if (produto == null) return NotFound();

            produto.CodigoSku = request.CodigoSku;
            produto.Nome = request.Nome;
            produto.Categoria = request.Categoria;
            produto.PrecoUnitario = request.PrecoUnitario;
            produto.QuantidadeMinima = request.QuantidadeMinima;

            var produtoAtualizado = await _produtoService.AtualizarProdutoAsync(produto);
            return Ok(MapToDto(produtoAtualizado));
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao atualizar produto", error = ex.Message });
        }
    }

    /// <summary>
    /// Deleta um produto
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(long id)
    {
        try
        {
            await _produtoService.DeletarProdutoAsync(id);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao deletar produto", error = ex.Message });
        }
    }

    private ProdutoResponseDto MapToDto(Produto produto)
    {
        return new ProdutoResponseDto
        {
            Id = produto.Id,
            CodigoSku = produto.CodigoSku,
            Nome = produto.Nome,
            Categoria = produto.Categoria.ToString(),
            PrecoUnitario = produto.PrecoUnitario,
            QuantidadeMinima = produto.QuantidadeMinima,
            QuantidadeAtual = produto.QuantidadeAtual,
            DataCriacao = produto.DataCriacao,
            EstoqueBaixo = produto.IsEstoqueBaixo(),
            ValorTotalEstoque = produto.QuantidadeAtual * produto.PrecoUnitario
        };
    }
}
