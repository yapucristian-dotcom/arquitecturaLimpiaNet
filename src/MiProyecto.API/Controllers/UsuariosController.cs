using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiProyecto.API.Authentication;
using MiProyecto.API.Extensions;
using MiProyecto.Application.Common.Pagination;
using MiProyecto.Application.Common.Responses;
using MiProyecto.Application.Features.Usuarios;
using MiProyecto.Application.Features.Usuarios.ActualizarUsuario;
using MiProyecto.Application.Features.Usuarios.CrearUsuario;
using MiProyecto.Application.Features.Usuarios.EliminarUsuario;
using MiProyecto.Application.Features.Usuarios.ListarUsuarios;
using MiProyecto.Application.Features.Usuarios.ObtenerUsuario;

namespace MiProyecto.API.Controllers;

[ApiController]
[Route("api/usuarios")]
[Authorize(Policy = AuthorizationPolicies.User)]
[Produces("application/json")]
public sealed class UsuariosController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsuariosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.Admin)]
    [ProducesResponseType(typeof(ApiResponse<CrearUsuarioResult>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<CrearUsuarioResult>>> Crear(
        [FromBody] CrearUsuarioCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse.Ok(result, "Usuario creado", HttpContext.GetCorrelationId()));
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(ApiResponse<UsuarioDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<UsuarioDto>>> Obtener(long id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ObtenerUsuarioQuery { Id = id }, cancellationToken);
        return Ok(ApiResponse.Ok(result, correlationId: HttpContext.GetCorrelationId()));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<UsuarioDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<PagedResult<UsuarioDto>>>> Listar(
        [FromQuery] ListarUsuariosQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(ApiResponse.Ok(result, correlationId: HttpContext.GetCorrelationId()));
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = AuthorizationPolicies.Admin)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<object?>>> Actualizar(
        long id,
        [FromBody] ActualizarUsuarioCommand command,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new ActualizarUsuarioCommand
            {
                Id = id,
                Nombre = command.Nombre,
                Correo = command.Correo,
                Activo = command.Activo
            },
            cancellationToken);

        return Ok(ApiResponse.Ok("Usuario actualizado", HttpContext.GetCorrelationId()));
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = AuthorizationPolicies.Admin)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<object?>>> Eliminar(long id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new EliminarUsuarioCommand { Id = id }, cancellationToken);
        return Ok(ApiResponse.Ok("Usuario eliminado", HttpContext.GetCorrelationId()));
    }
}
