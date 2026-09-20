using MediatR;

namespace MiProyecto.Application.Features.Usuarios.EliminarUsuario;

public sealed class EliminarUsuarioCommand : IRequest<Unit>
{
    public long Id { get; init; }
}
