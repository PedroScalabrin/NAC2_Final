namespace GestaoEstoque.Exceptions;

/// <summary>
/// Exceção customizada para erros de validação de negócio.
/// </summary>
public class ValidationException : Exception
{
    public ValidationException(string message) : base(message) { }
}

/// <summary>
/// Exceção para quando um recurso não é encontrado.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}

/// <summary>
/// Exceção para operações de estoque inválidas.
/// </summary>
public class EstoqueException : Exception
{
    public EstoqueException(string message) : base(message) { }
}
