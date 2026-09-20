using MiProyecto.Application.Features.Usuarios;

namespace MiProyecto.UnitTests.Features.Usuarios;

internal static class UsuarioTestData
{
    public static UsuarioDto Demo(long id = 10) => new()
    {
        Id = id,
        Nombre = "Usuario Demo",
        Correo = "demo@miproyecto.local",
        Activo = true
    };
}
