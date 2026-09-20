using MediatR;
using MiProyecto.Application.Abstractions;

namespace MiProyecto.Application.Features.Usuarios.ActualizarUsuario;

public sealed class ActualizarUsuarioHandler : IRequestHandler<ActualizarUsuarioCommand, Unit>
{
    private readonly IUsuarioRepository _usuarios;

    public ActualizarUsuarioHandler(IUsuarioRepository usuarios)
    {
        _usuarios = usuarios;
    }

    public async Task<Unit> Handle(ActualizarUsuarioCommand request, CancellationToken cancellationToken)
    {
        await _usuarios.ActualizarAsync(request, cancellationToken);
        return Unit.Value;
    }
}
