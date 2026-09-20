using MiProyecto.Domain.Exceptions;
using MiProyecto.Domain.ValueObjects;

namespace MiProyecto.Domain.Entities;

public sealed class Usuario
{
    public long Id { get; }

    public string Nombre { get; }

    public Email Correo { get; }

    public bool Activo { get; }

    public Usuario(long id, string nombre, Email correo, bool activo = true)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new DomainException("El nombre es obligatorio.");
        }

        Id = id;
        Nombre = nombre.Trim();
        Correo = correo ?? throw new ArgumentNullException(nameof(correo));
        Activo = activo;
    }
}
