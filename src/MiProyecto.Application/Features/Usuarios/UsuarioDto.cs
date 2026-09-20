namespace MiProyecto.Application.Features.Usuarios;

public sealed class UsuarioDto
{
    public long Id { get; init; }

    public string Nombre { get; init; } = string.Empty;

    public string Correo { get; init; } = string.Empty;

    public bool Activo { get; init; }
}
