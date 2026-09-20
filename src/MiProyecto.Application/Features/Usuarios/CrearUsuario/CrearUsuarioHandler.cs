using MediatR;
using MiProyecto.Application.Abstractions;

namespace MiProyecto.Application.Features.Usuarios.CrearUsuario;

public sealed class CrearUsuarioHandler : IRequestHandler<CrearUsuarioCommand, CrearUsuarioResult>
{
    private readonly IUsuarioRepository _usuarios;

    public CrearUsuarioHandler(IUsuarioRepository usuarios)
    {
        _usuarios = usuarios;
    }

    public async Task<CrearUsuarioResult> Handle(CrearUsuarioCommand request, CancellationToken cancellationToken)
    {
        var id = await _usuarios.CrearAsync(request, cancellationToken);
        return new CrearUsuarioResult(id);
    }
}
