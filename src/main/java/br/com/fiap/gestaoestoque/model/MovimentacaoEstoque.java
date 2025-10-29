package br.com.fiap.gestaoestoque.model;

import br.com.fiap.gestaoestoque.model.enums.TipoMovimentacao;
import jakarta.persistence.*;
import jakarta.validation.constraints.*;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.time.LocalDate;
import java.time.LocalDateTime;

/**
 * Entidade representando uma Movimentação de Estoque.
 * 
 * Regras de negócio:
 * - Produtos perecíveis devem ter lote e data de validade
 * - Não é permitido quantidade negativa
 * - Ao registrar saída, deve verificar estoque suficiente
 */
@Entity
@Table(name = "movimentacoes_estoque")
@Data
@NoArgsConstructor
@AllArgsConstructor
public class MovimentacaoEstoque {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "produto_id", nullable = false)
    @NotNull(message = "Produto é obrigatório")
    private Produto produto;

    @Enumerated(EnumType.STRING)
    @Column(nullable = false)
    @NotNull(message = "Tipo de movimentação é obrigatório")
    private TipoMovimentacao tipo;

    @Column(nullable = false)
    @NotNull(message = "Quantidade é obrigatória")
    @Min(value = 1, message = "Quantidade deve ser maior que zero")
    private Integer quantidade;

    @Column(nullable = false)
    private LocalDateTime dataMovimentacao;

    @Column(length = 100)
    @Size(max = 100, message = "Lote deve ter no máximo 100 caracteres")
    private String lote;

    @Column
    private LocalDate dataValidade;

    @Column(length = 500)
    @Size(max = 500, message = "Observação deve ter no máximo 500 caracteres")
    private String observacao;

    @PrePersist
    protected void onCreate() {
        dataMovimentacao = LocalDateTime.now();
    }

    /**
     * Valida se a movimentação está conforme as regras de negócio.
     * 
     * @throws IllegalArgumentException se a movimentação for inválida
     */
    public void validar() {
        // Valida quantidade
        if (quantidade == null || quantidade <= 0) {
            throw new IllegalArgumentException("Quantidade deve ser maior que zero");
        }

        // Valida lote e data de validade para produtos perecíveis
        if (produto.isPerecivel()) {
            if (lote == null || lote.trim().isEmpty()) {
                throw new IllegalArgumentException("Lote é obrigatório para produtos perecíveis");
            }
            if (dataValidade == null) {
                throw new IllegalArgumentException("Data de validade é obrigatória para produtos perecíveis");
            }
            if (dataValidade.isBefore(LocalDate.now())) {
                throw new IllegalArgumentException("Data de validade não pode ser anterior à data atual");
            }
        }

        // Valida estoque suficiente para saída
        if (tipo == TipoMovimentacao.SAIDA) {
            if (!produto.temEstoqueSuficiente(quantidade)) {
                throw new IllegalArgumentException(
                    String.format("Estoque insuficiente. Disponível: %d, Solicitado: %d", 
                                  produto.getQuantidadeAtual(), quantidade)
                );
            }
        }
    }

    /**
     * Verifica se o produto está próximo do vencimento (dentro de 7 dias).
     */
    public boolean isProximoVencimento() {
        if (dataValidade == null) {
            return false;
        }
        LocalDate dataLimite = LocalDate.now().plusDays(7);
        return dataValidade.isBefore(dataLimite) || dataValidade.isEqual(dataLimite);
    }

    /**
     * Verifica se o produto está vencido.
     */
    public boolean isVencido() {
        if (dataValidade == null) {
            return false;
        }
        return dataValidade.isBefore(LocalDate.now());
    }
}
