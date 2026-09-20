using MiProyecto.Domain.Exceptions;

namespace MiProyecto.Domain.ValueObjects;

public sealed class Email : IEquatable<Email>
{
    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException("El correo es obligatorio.");
        }

        var normalized = value.Trim();

        if (!EsValido(normalized))
        {
            throw new DomainException("El correo no es válido.");
        }

        Value = normalized;
    }

    public bool Equals(Email? other) =>
        other is not null &&
        string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);

    public override bool Equals(object? obj) => obj is Email other && Equals(other);

    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);

    public override string ToString() => Value;

    public static implicit operator string(Email email) => email.Value;

    private static bool EsValido(string value)
    {
        var at = value.IndexOf('@');
        return at > 0 && at < value.Length - 1 && value.IndexOf('@', at + 1) < 0;
    }
}
