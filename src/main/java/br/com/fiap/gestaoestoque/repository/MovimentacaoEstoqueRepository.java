package br.com.fiap.gestaoestoque.repository;

import br.com.fiap.gestaoestoque.model.MovimentacaoEstoque;
import br.com.fiap.gestaoestoque.model.Produto;
import br.com.fiap.gestaoestoque.model.enums.TipoMovimentacao;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

import java.time.LocalDate;
import java.time.LocalDateTime;
import java.util.List;

/**
 * Repositório para operações de banco de dados da entidade MovimentacaoEstoque.
 */
@Repository
public interface MovimentacaoEstoqueRepository extends JpaRepository<MovimentacaoEstoque, Long> {

    /**
     * Busca movimentações por produto.
     */
    List<MovimentacaoEstoque> findByProdutoOrderByDataMovimentacaoDesc(Produto produto);

    /**
     * Busca movimentações por tipo.
     */
    List<MovimentacaoEstoque> findByTipoOrderByDataMovimentacaoDesc(TipoMovimentacao tipo);

    /**
     * Busca movimentações por lote.
     */
    List<MovimentacaoEstoque> findByLote(String lote);

    /**
     * Busca movimentações em um período.
     */
    List<MovimentacaoEstoque> findByDataMovimentacaoBetweenOrderByDataMovimentacaoDesc(
        LocalDateTime dataInicio, LocalDateTime dataFim);

    /**
     * Busca produtos próximos do vencimento.
     */
    @Query("SELECT m FROM MovimentacaoEstoque m WHERE m.dataValidade IS NOT NULL " +
           "AND m.dataValidade BETWEEN :dataInicio AND :dataFim " +
           "ORDER BY m.dataValidade ASC")
    List<MovimentacaoEstoque> findProdutosProximosVencimento(
        @Param("dataInicio") LocalDate dataInicio, 
        @Param("dataFim") LocalDate dataFim);

    /**
     * Busca produtos vencidos.
     */
    @Query("SELECT m FROM MovimentacaoEstoque m WHERE m.dataValidade IS NOT NULL " +
           "AND m.dataValidade < CURRENT_DATE ORDER BY m.dataValidade ASC")
    List<MovimentacaoEstoque> findProdutosVencidos();

    /**
     * Busca movimentações de um produto em um período.
     */
    @Query("SELECT m FROM MovimentacaoEstoque m WHERE m.produto = :produto " +
           "AND m.dataMovimentacao BETWEEN :dataInicio AND :dataFim " +
           "ORDER BY m.dataMovimentacao DESC")
    List<MovimentacaoEstoque> findByProdutoAndPeriodo(
        @Param("produto") Produto produto,
        @Param("dataInicio") LocalDateTime dataInicio,
        @Param("dataFim") LocalDateTime dataFim);
}
