namespace WexChallenge.Domain.Exceptions;

/// <summary>
/// Base exception for domain-specific errors.
/// </summary>
public abstract class DomainException : Exception
{
    public string Code { get; }

    protected DomainException(string code, string message) : base(message)
    {
        Code = code;
    }
}
