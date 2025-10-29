# Sistema de Gestão de Estoque - NAC 2 🏭

## 📋 Descrição

Sistema completo de gestão de estoque desenvolvido em **C# / .NET 7.0** com ASP.NET Core Web API para controlar produtos perecíveis e não-perecíveis, com regras específicas de validação, cálculos automáticos, controle de lotes e sistema de alertas.

---

## 🎯 Etapas Implementadas

### ✅ Etapa 1 - Modelagem do Domínio (2.5 pontos)
- Modelagem completa das entidades (Produto e MovimentacaoEstoque)
- Configuração do banco de dados (Entity Framework InMemory)
- Relacionamentos e validações
- Repositórios com queries personalizadas

### ✅ Etapa 2 - Implementação das Regras de Negócio (5.0 pontos)
- **Serviço de Produto (2.0 pontos)**
  - Validação de categoria vs dados obrigatórios
  - Método para verificar produtos abaixo do estoque mínimo
  - Cadastro completo de produtos
  
- **Serviço de Movimentação (2.5 pontos)**
  - Validação de quantidade positiva
  - Verificação de estoque suficiente para saídas
  - Atualização automática do saldo do produto
  - Validação de data de validade para perecíveis
  - Entrada e saída de estoque
  
- **Serviço de Relatórios (0.5 pontos)**
  - Cálculo do valor total do estoque (quantidade × preço)
  - Listagem de produtos que vencerão em até 7 dias
  - Identificação de produtos com estoque abaixo do mínimo

### ✅ Etapa 3 - Validações e Tratamento de Erros (2.0 pontos)
- **Validações de Integridade (1.0 ponto)**
  - Produto perecível sem data de validade
  - Saída com quantidade maior que estoque disponível
  - Movimentação com quantidade negativa
  
- **Validações de Regras Complexas (1.0 ponto)**
  - Produto perecível não pode ter movimentação após validade
  - Alertas de estoque mínimo
  - Cálculo correto do saldo após movimentações

### ✅ Etapa 4 - Documentação e Entrega (0.5 ponto)
- README.md completo com exemplos
- Diagrama de entidades
- Documentação da API (Swagger)
- Instruções de execução

---

## 🏗️ Arquitetura do Sistema

```
┌─────────────┐
│ Controllers │ ← Camada de API (DTOs, Validações)
└──────┬──────┘
       │
┌──────▼──────┐
│  Services   │ ← Camada de Negócio (Regras, Validações)
└──────┬──────┘
       │
┌──────▼───────┐
│ Repositories │ ← Camada de Dados (Queries)
└──────┬───────┘
       │
┌──────▼──────┐
│   DbContext │ ← Entity Framework (InMemory)
└─────────────┘
```

---

## 📊 Diagrama de Entidades

```
┌────────────────────────────────┐
│          PRODUTO               │
├────────────────────────────────┤
│ + Id: long (PK)                │
│ + CodigoSku: string (UNIQUE)   │
│ + Nome: string                 │
│ + Categoria: Enum              │
│ + PrecoUnitario: decimal       │
│ + QuantidadeMinima: int        │
│ + QuantidadeAtual: int         │
│ + DataCriacao: DateTime        │
├────────────────────────────────┤
│ + IsEstoqueBaixo(): bool       │
│ + IsPerecivel(): bool          │
│ + AdicionarEstoque(int)        │
│ + RemoverEstoque(int)          │
│ + TemEstoqueSuficiente(int)    │
└────────────┬───────────────────┘
             │ 1:N
             │
┌────────────▼───────────────────┐
│   MOVIMENTACAO_ESTOQUE         │
├────────────────────────────────┤
│ + Id: long (PK)                │
│ + ProdutoId: long (FK)         │
│ + Tipo: Enum (ENTRADA/SAIDA)   │
│ + Quantidade: int              │
│ + DataMovimentacao: DateTime   │
│ + Lote: string?                │
│ + DataValidade: DateTime?      │
│ + Observacao: string?          │
├────────────────────────────────┤
│ + Validar()                    │
│ + IsProximoVencimento(): bool  │
│ + IsVencido(): bool            │
└────────────────────────────────┘

ENUMS:
- CategoriaProduto: PERECIVEL | NAO_PERECIVEL
- TipoMovimentacao: ENTRADA | SAIDA
```

---

## 🚀 Como Executar

### Pré-requisitos
- .NET 7.0 SDK ou superior

### Passos

1. **Clone o repositório**
```bash
git clone https://github.com/PedroScalabrin/NAC2_Final.git
cd NAC2_Final/GestaoEstoque
```

2. **Restaure as dependências**
```bash
dotnet restore
```

3. **Execute a aplicação**
```bash
dotnet run
```

4. **Acesse o Swagger**
   - URL: https://localhost:7xxx/swagger
   - (A porta será exibida no console)

---

## 📡 Exemplos de Requisições API

### 1. Cadastrar Produto Perecível

**POST** `/api/produtos`

```json
{
  "codigoSku": "FRUTA-001",
  "nome": "Maçã Fuji",
  "categoria": "PERECIVEL",
  "precoUnitario": 5.50,
  "quantidadeMinima": 50
}
```

### 2. Registrar Entrada de Estoque

**POST** `/api/movimentacoes/entrada`

```json
{
  "produtoId": 1,
  "quantidade": 100,
  "lote": "LOTE-2025-001",
  "dataValidade": "2025-11-15",
  "observacao": "Primeira remessa"
}
```

### 3. Registrar Saída de Estoque

**POST** `/api/movimentacoes/saida`

```json
{
  "produtoId": 1,
  "quantidade": 30,
  "observacao": "Venda"
}
```

### 4. Listar Produtos com Estoque Baixo

**GET** `/api/relatorios/estoque-baixo`

### 5. Dashboard Completo

**GET** `/api/relatorios/dashboard`

---

## ⚠️ Validações Implementadas

| Validação | Descrição | Resposta |
|-----------|-----------|----------|
| Produto perecível sem lote | Entrada sem lote em produto PERECIVEL | 400 Bad Request |
| Estoque insuficiente | Saída maior que disponível | 400 Bad Request |
| Quantidade negativa | Movimentação com qtd <= 0 | 400 Bad Request |
| Data de validade vencida | Data anterior à atual | 400 Bad Request |

---

## 📊 Pontuação Total: **10.0 pontos**

| Etapa | Pontos |
|-------|--------|
| Modelagem do Domínio | 2.5 |
| Regras de Negócio | 5.0 |
| Validações e Erros | 2.0 |
| Documentação | 0.5 |

---

## 👨‍💻 Autor

**Pedro Scalabrin** - RM 98914  
FIAP - Segundo Semestre - 2025

**Repositório:** https://github.com/PedroScalabrin/NAC2_Final
