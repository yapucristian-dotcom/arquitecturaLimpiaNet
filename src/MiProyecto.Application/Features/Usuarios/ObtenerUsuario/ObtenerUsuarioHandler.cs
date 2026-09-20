using MediatR;
using MiProyecto.Application.Abstractions;
using MiProyecto.Application.Common.Errors;
using MiProyecto.Application.Common.Exceptions;
using MiProyecto.Application.Features.Usuarios;

namespace MiProyecto.Application.Features.Usuarios.ObtenerUsuario;

public sealed class ObtenerUsuarioHandler : IRequestHandler<ObtenerUsuarioQuery, UsuarioDto>
{
    private readonly IUsuarioRepository _usuarios;

    public ObtenerUsuarioHandler(IUsuarioRepository usuarios)
    {
        _usuarios = usuarios;
    }

    public async Task<UsuarioDto> Handle(ObtenerUsuarioQuery request, CancellationToken cancellationToken)
    {
        var usuario = await _usuarios.ObtenerAsync(request.Id, cancellationToken);

        if (usuario is null)
        {
            throw new NotFoundException("El usuario no existe", ErrorCodes.Usuarios.NotFound);
        }

        return usuario;
    }
}
