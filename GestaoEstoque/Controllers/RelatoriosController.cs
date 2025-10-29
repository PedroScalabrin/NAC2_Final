using Microsoft.AspNetCore.Mvc;
using GestaoEstoque.Services;

namespace GestaoEstoque.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RelatoriosController : ControllerBase
{
    private readonly IRelatorioService _relatorioService;

    public RelatoriosController(IRelatorioService relatorioService)
    {
        _relatorioService = relatorioService;
    }

    /// <summary>
    /// Calcula o valor total do estoque (quantidade × preço)
    /// </summary>
    [HttpGet("valor-total-estoque")]
    public async Task<ActionResult<object>> GetValorTotalEstoque()
    {
        try
        {
            var valorTotal = await _relatorioService.CalcularValorTotalEstoqueAsync();
            return Ok(new { valorTotal, moeda = "R$" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao calcular valor total", error = ex.Message });
        }
    }

    /// <summary>
    /// Lista produtos que vencerão em até X dias (padrão: 7)
    /// </summary>
    [HttpGet("proximos-vencimento")]
    public async Task<ActionResult> GetProximosVencimento([FromQuery] int dias = 7)
    {
        try
        {
            var produtos = await _relatorioService.ListarProdutosProximosVencimentoAsync(dias);
            var response = produtos.Select(m => new
            {
                m.Id,
                m.ProdutoId,
                ProdutoNome = m.Produto?.Nome,
                m.Lote,
                m.DataValidade,
                DiasParaVencer = m.DataValidade.HasValue 
                    ? (m.DataValidade.Value - DateTime.Now).Days 
                    : 0
            });
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao buscar produtos próximos ao vencimento", error = ex.Message });
        }
    }

    /// <summary>
    /// Identifica produtos com estoque abaixo do mínimo
    /// </summary>
    [HttpGet("estoque-baixo")]
    public async Task<ActionResult> GetEstoqueBaixo()
    {
        try
        {
            var produtos = await _relatorioService.IdentificarProdutosEstoqueBaixoAsync();
            var response = produtos.Select(p => new
            {
                p.Id,
                p.CodigoSku,
                p.Nome,
                p.QuantidadeAtual,
                p.QuantidadeMinima,
                Deficit = p.QuantidadeMinima - p.QuantidadeAtual,
                PercentualDisponivel = p.QuantidadeMinima > 0 
                    ? (p.QuantidadeAtual * 100.0 / p.QuantidadeMinima) 
                    : 0
            });
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao buscar produtos com estoque baixo", error = ex.Message });
        }
    }

    /// <summary>
    /// Dashboard consolidado com todos os indicadores
    /// </summary>
    [HttpGet("dashboard")]
    public async Task<ActionResult> GetDashboard()
    {
        try
        {
            var dashboard = await _relatorioService.ObterDashboardAsync();
            return Ok(dashboard);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao gerar dashboard", error = ex.Message });
        }
    }
}
