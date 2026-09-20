using MediatR;

namespace MiProyecto.Application.Features.Usuarios.CrearUsuario;

public sealed class CrearUsuarioCommand : IRequest<CrearUsuarioResult>
{
    public string Nombre { get; init; } = string.Empty;

    public string Correo { get; init; } = string.Empty;
}
