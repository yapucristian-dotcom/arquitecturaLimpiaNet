using MediatR;
using MiProyecto.Application.Features.Usuarios;

namespace MiProyecto.Application.Features.Usuarios.ObtenerUsuario;

public sealed class ObtenerUsuarioQuery : IRequest<UsuarioDto>
{
    public long Id { get; init; }
}
