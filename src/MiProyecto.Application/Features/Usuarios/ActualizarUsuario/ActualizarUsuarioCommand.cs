using MediatR;

namespace MiProyecto.Application.Features.Usuarios.ActualizarUsuario;

public sealed class ActualizarUsuarioCommand : IRequest<Unit>
{
    public long Id { get; init; }

    public string Nombre { get; init; } = string.Empty;

    public string Correo { get; init; } = string.Empty;

    public bool Activo { get; init; } = true;
}
