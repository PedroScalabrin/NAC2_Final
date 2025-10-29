package br.com.fiap.gestaoestoque.repository;

import br.com.fiap.gestaoestoque.model.Produto;
import br.com.fiap.gestaoestoque.model.enums.CategoriaProduto;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.stereotype.Repository;

import java.util.List;
import java.util.Optional;

/**
 * Repositório para operações de banco de dados da entidade Produto.
 */
@Repository
public interface ProdutoRepository extends JpaRepository<Produto, Long> {

    /**
     * Busca um produto pelo código SKU.
     */
    Optional<Produto> findByCodigoSku(String codigoSku);

    /**
     * Verifica se existe um produto com o código SKU informado.
     */
    boolean existsByCodigoSku(String codigoSku);

    /**
     * Busca produtos por categoria.
     */
    List<Produto> findByCategoria(CategoriaProduto categoria);

    /**
     * Busca produtos com estoque abaixo da quantidade mínima.
     */
    @Query("SELECT p FROM Produto p WHERE p.quantidadeAtual < p.quantidadeMinima")
    List<Produto> findProdutosComEstoqueBaixo();

    /**
     * Busca produtos por nome (contém a string informada, case insensitive).
     */
    List<Produto> findByNomeContainingIgnoreCase(String nome);
}
