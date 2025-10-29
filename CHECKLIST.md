# Checklist - Etapa 1: Modelagem do Domínio

## ✅ Tarefas Concluídas

### 1. Modelagem das Entidades
- [x] **Produto**
  - [x] Id (PK, auto-incremento)
  - [x] CodigoSku (único, obrigatório)
  - [x] Nome (obrigatório)
  - [x] Categoria (PERECIVEL/NAO_PERECIVEL)
  - [x] PrecoUnitario (decimal, > 0)
  - [x] QuantidadeMinima (>= 0)
  - [x] QuantidadeAtual (default: 0)
  - [x] DataCriacao (auto-gerado)

- [x] **MovimentacaoEstoque**
  - [x] Id (PK, auto-incremento)
  - [x] ProdutoId (FK, obrigatório)
  - [x] Tipo (ENTRADA/SAIDA)
  - [x] Quantidade (> 0)
  - [x] DataMovimentacao (auto-gerado)
  - [x] Lote (para perecíveis)
  - [x] DataValidade (para perecíveis)
  - [x] Observacao (opcional)

### 2. Enums
- [x] CategoriaProduto (PERECIVEL, NAO_PERECIVEL)
- [x] TipoMovimentacao (ENTRADA, SAIDA)

### 3. Regras de Negócio Implementadas
- [x] Produtos perecíveis devem ter lote e data de validade
- [x] Não é permitido entrada/saída de quantidade negativa
- [x] Ao registrar saída, verificar estoque suficiente
- [x] Produtos abaixo da quantidade mínima geram alerta
- [x] Validação de data de validade (não pode ser anterior à data atual)

### 4. Estrutura do Projeto
- [x] Models/Enums criados
- [x] DbContext configurado (Entity Framework InMemory)
- [x] Repositories (Interface + Implementação)
- [x] Program.cs configurado com DI
- [x] README.md completo
- [x] .gitignore configurado

### 5. Funcionalidades Adicionais
- [x] Métodos de negócio no Produto
  - IsEstoqueBaixo()
  - IsPerecivel()
  - AdicionarEstoque()
  - RemoverEstoque()
  - TemEstoqueSuficiente()

- [x] Métodos de negócio na MovimentacaoEstoque
  - Validar()
  - IsProximoVencimento()
  - IsVencido()

### 6. Repositórios com Queries
- [x] ProdutoRepository
  - GetByCodigoSku
  - GetByCategoria
  - GetProdutosComEstoqueBaixo
  - SearchByNome
  - ExistsByCodigoSku

- [x] MovimentacaoEstoqueRepository
  - GetByProdutoId
  - GetByTipo
  - GetByLote
  - GetByPeriodo
  - GetProdutosProximosVencimento
  - GetProdutosVencidos

### 7. Configuração do Banco de Dados
- [x] Entity Framework Core InMemory
- [x] Relacionamento 1:N configurado
- [x] Índices criados (CodigoSku único)
- [x] Conversão de Enums configurada

## 📊 Estatísticas do Projeto

- **Total de Classes**: 8
- **Total de Interfaces**: 2
- **Total de Enums**: 2
- **Linhas de Código**: ~800 (aproximado)
- **Validações**: Data Annotations + Lógica de Negócio
- **Tecnologia**: .NET 7.0 + EF Core InMemory

## 🎯 Próximas Etapas (Sugeridas)

- [ ] Etapa 2: Services (Lógica de Negócio)
- [ ] Etapa 3: Controllers REST (API)
- [ ] Etapa 4: DTOs (Request/Response)
- [ ] Etapa 5: Testes Unitários
- [ ] Etapa 6: Data Loader (Dados de Exemplo)

## 📝 Comprovante
**Commit**: "Etapa 1 - Modelagem do domínio"
**Data**: 29/10/2025
**Autor**: Pedro Scalabrin - RM 98914
