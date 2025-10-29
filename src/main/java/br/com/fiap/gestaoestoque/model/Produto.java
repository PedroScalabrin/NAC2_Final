package br.com.fiap.gestaoestoque.model;

import br.com.fiap.gestaoestoque.model.enums.CategoriaProduto;
import jakarta.persistence.*;
import jakarta.validation.constraints.*;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.math.BigDecimal;
import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;

/**
 * Entidade representando um Produto no sistema de gestão de estoque.
 * 
 * Regras de negócio:
 * - Código SKU deve ser único
 * - Preço unitário deve ser positivo
 * - Quantidade mínima em estoque não pode ser negativa
 */
@Entity
@Table(name = "produtos")
@Data
@NoArgsConstructor
@AllArgsConstructor
public class Produto {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @Column(unique = true, nullable = false, length = 50)
    @NotBlank(message = "Código SKU é obrigatório")
    @Size(max = 50, message = "Código SKU deve ter no máximo 50 caracteres")
    private String codigoSku;

    @Column(nullable = false, length = 200)
    @NotBlank(message = "Nome do produto é obrigatório")
    @Size(max = 200, message = "Nome deve ter no máximo 200 caracteres")
    private String nome;

    @Enumerated(EnumType.STRING)
    @Column(nullable = false)
    @NotNull(message = "Categoria é obrigatória")
    private CategoriaProduto categoria;

    @Column(nullable = false, precision = 10, scale = 2)
    @NotNull(message = "Preço unitário é obrigatório")
    @DecimalMin(value = "0.01", message = "Preço unitário deve ser maior que zero")
    private BigDecimal precoUnitario;

    @Column(nullable = false)
    @NotNull(message = "Quantidade mínima em estoque é obrigatória")
    @Min(value = 0, message = "Quantidade mínima não pode ser negativa")
    private Integer quantidadeMinima;

    @Column(nullable = false)
    private Integer quantidadeAtual = 0;

    @Column(nullable = false, updatable = false)
    private LocalDateTime dataCriacao;

    @OneToMany(mappedBy = "produto", cascade = CascadeType.ALL, orphanRemoval = true)
    private List<MovimentacaoEstoque> movimentacoes = new ArrayList<>();

    @PrePersist
    protected void onCreate() {
        dataCriacao = LocalDateTime.now();
        if (quantidadeAtual == null) {
            quantidadeAtual = 0;
        }
    }

    /**
     * Verifica se o produto está abaixo da quantidade mínima (alerta de estoque baixo).
     */
    public boolean isEstoqueBaixo() {
        return quantidadeAtual < quantidadeMinima;
    }

    /**
     * Verifica se é um produto perecível.
     */
    public boolean isPerecivel() {
        return categoria == CategoriaProduto.PERECIVEL;
    }

    /**
     * Adiciona quantidade ao estoque atual.
     */
    public void adicionarEstoque(Integer quantidade) {
        this.quantidadeAtual += quantidade;
    }

    /**
     * Remove quantidade do estoque atual.
     */
    public void removerEstoque(Integer quantidade) {
        this.quantidadeAtual -= quantidade;
    }

    /**
     * Verifica se há estoque suficiente para retirada.
     */
    public boolean temEstoqueSuficiente(Integer quantidade) {
        return this.quantidadeAtual >= quantidade;
    }
}
