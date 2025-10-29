using Microsoft.AspNetCore.Mvc;
using GestaoEstoque.Services;
using GestaoEstoque.DTOs;
using GestaoEstoque.Models;
using GestaoEstoque.Models.Enums;
using GestaoEstoque.Exceptions;

namespace GestaoEstoque.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MovimentacoesController : ControllerBase
{
    private readonly IMovimentacaoEstoqueService _movimentacaoService;

    public MovimentacoesController(IMovimentacaoEstoqueService movimentacaoService)
    {
        _movimentacaoService = movimentacaoService;
    }

    /// <summary>
    /// Lista todas as movimentações
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MovimentacaoResponseDto>>> GetAll()
    {
        try
        {
            var movimentacoes = await _movimentacaoService.ObterTodasAsync();
            var response = movimentacoes.Select(MapToDto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao buscar movimentações", error = ex.Message });
        }
    }

    /// <summary>
    /// Lista movimentações por produto
    /// </summary>
    [HttpGet("produto/{produtoId}")]
    public async Task<ActionResult<IEnumerable<MovimentacaoResponseDto>>> GetByProduto(long produtoId)
    {
        try
        {
            var movimentacoes = await _movimentacaoService.ObterPorProdutoAsync(produtoId);
            var response = movimentacoes.Select(MapToDto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao buscar movimentações", error = ex.Message });
        }
    }

    /// <summary>
    /// Lista movimentações por tipo
    /// </summary>
    [HttpGet("tipo/{tipo}")]
    public async Task<ActionResult<IEnumerable<MovimentacaoResponseDto>>> GetByTipo(TipoMovimentacao tipo)
    {
        try
        {
            var movimentacoes = await _movimentacaoService.ObterPorTipoAsync(tipo);
            var response = movimentacoes.Select(MapToDto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao buscar movimentações", error = ex.Message });
        }
    }

    /// <summary>
    /// Registra uma ENTRADA de estoque
    /// </summary>
    [HttpPost("entrada")]
    public async Task<ActionResult<MovimentacaoResponseDto>> RegistrarEntrada([FromBody] EntradaEstoqueRequestDto request)
    {
        try
        {
            var movimentacao = await _movimentacaoService.RegistrarEntradaAsync(
                request.ProdutoId,
                request.Quantidade,
                request.Lote,
                request.DataValidade,
                request.Observacao
            );

            return CreatedAtAction(nameof(GetAll), MapToDto(movimentacao));
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao registrar entrada", error = ex.Message });
        }
    }

    /// <summary>
    /// Registra uma SAÍDA de estoque
    /// </summary>
    [HttpPost("saida")]
    public async Task<ActionResult<MovimentacaoResponseDto>> RegistrarSaida([FromBody] SaidaEstoqueRequestDto request)
    {
        try
        {
            var movimentacao = await _movimentacaoService.RegistrarSaidaAsync(
                request.ProdutoId,
                request.Quantidade,
                request.Observacao
            );

            return CreatedAtAction(nameof(GetAll), MapToDto(movimentacao));
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (EstoqueException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao registrar saída", error = ex.Message });
        }
    }

    private MovimentacaoResponseDto MapToDto(MovimentacaoEstoque movimentacao)
    {
        return new MovimentacaoResponseDto
        {
            Id = movimentacao.Id,
            ProdutoId = movimentacao.ProdutoId,
            ProdutoNome = movimentacao.Produto?.Nome,
            Tipo = movimentacao.Tipo.ToString(),
            Quantidade = movimentacao.Quantidade,
            DataMovimentacao = movimentacao.DataMovimentacao,
            Lote = movimentacao.Lote,
            DataValidade = movimentacao.DataValidade,
            Observacao = movimentacao.Observacao,
            ProximoVencimento = movimentacao.IsProximoVencimento(),
            Vencido = movimentacao.IsVencido()
        };
    }
}
