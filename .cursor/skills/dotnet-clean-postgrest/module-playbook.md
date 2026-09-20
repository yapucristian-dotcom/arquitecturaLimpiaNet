# Playbook: agregar un módulo

Clonar el módulo `Usuarios`. No inventar otra forma de CRUD.

## Pasos

1. Domain: entidad (si hay identidad de negocio).
2. SQL: `database/tables/{tabla}.sql` + `database/functions/fn_{modulo}_*.sql`.
3. Application: `Features/{Modulo}/{Accion}/` (command/query, validator, handler).
4. `Abstractions/I{Modulo}Repository` con métodos explícitos.
5. Infrastructure: `{Modulo}Repository` → solo `IPostgrestClient`.
6. Registrar repositorio en `AddInfrastructure`.
7. API: controller delgado + atributos Swagger + `[Authorize]` / policies.
8. Unit tests del handler y validator.
9. Integration test del endpoint o del RPC, según exista Testcontainers.

## Contrato del puerto (ejemplo)

```csharp
public interface IUsuarioRepository
{
    Task<UsuarioDto?> ObtenerAsync(long id, CancellationToken ct);
    Task<PagedResult<UsuarioDto>> ListarAsync(ListarUsuariosQuery query, CancellationToken ct);
    Task<long> CrearAsync(CrearUsuarioCommand command, CancellationToken ct);
    Task ActualizarAsync(ActualizarUsuarioCommand command, CancellationToken ct);
    Task EliminarAsync(long id, CancellationToken ct);
}
```

Prohibido: `IRepository<T>`, `Add/Update/GetById` genéricos, o pasar `HttpRequest`.

## Controller

```text
POST   /api/usuarios
GET    /api/usuarios/{id}
GET    /api/usuarios?page=&pageSize=
PUT    /api/usuarios/{id}
DELETE /api/usuarios/{id}
```

Cada acción: `await mediator.Send(...)`.
