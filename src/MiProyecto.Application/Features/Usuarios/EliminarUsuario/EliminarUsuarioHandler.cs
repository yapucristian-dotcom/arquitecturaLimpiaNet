using MediatR;
using MiProyecto.Application.Abstractions;

namespace MiProyecto.Application.Features.Usuarios.EliminarUsuario;

public sealed class EliminarUsuarioHandler : IRequestHandler<EliminarUsuarioCommand, Unit>
{
    private readonly IUsuarioRepository _usuarios;

    public EliminarUsuarioHandler(IUsuarioRepository usuarios)
    {
        _usuarios = usuarios;
    }

    public async Task<Unit> Handle(EliminarUsuarioCommand request, CancellationToken cancellationToken)
    {
        await _usuarios.EliminarAsync(request.Id, cancellationToken);
        return Unit.Value;
    }
}
