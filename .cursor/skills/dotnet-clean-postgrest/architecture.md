# Arquitectura

## Proyectos y referencias

```text
MiProyecto.API            → Application, Infrastructure
MiProyecto.Infrastructure → Application
MiProyecto.Application    → Domain
MiProyecto.Domain         → (ninguna)
MiProyecto.UnitTests      → Application, Domain
MiProyecto.IntegrationTests → API
```

No crear proyecto Shared salvo un segundo host (worker/jobs).

## Estructura

```text
src/
  MiProyecto.Domain/
    Entities/
    ValueObjects/
    Enums/
    Exceptions/
  MiProyecto.Application/
    Common/                 # ApiResponse, Result, ErrorCodes, Pagination
    Abstractions/           # IUsuarioRepository y demás puertos
    Features/{Modulo}/{Accion}/
    DependencyInjection.cs
  MiProyecto.Infrastructure/
    PostgREST/              # IPostgrestClient, PostgrestClient, PostgrestOptions
    Persistence/            # UsuarioRepository
    Authentication/         # solo infra técnica si hace falta
    DependencyInjection.cs
  MiProyecto.API/
    Controllers/
    Middleware/
    Extensions/
    Program.cs
    appsettings.json
    appsettings.Development.json
tests/
  MiProyecto.UnitTests/
  MiProyecto.IntegrationTests/
database/
  schemas/
  tables/
  views/
  functions/
  seed/
  postgrest/
```

## Domain

Permitido: entidades de identidad, value objects, enums, excepciones de dominio.
Prohibido: repositorios, DTOs HTTP, atributos ASP.NET, SQL, PostgREST.

`Usuario` es entidad de ejemplo, no un módulo de seguridad corporativo completo.

## Application

- Un folder por caso de uso (vertical slice).
- MediatR: `IRequest` / `IRequestHandler`.
- Pipeline: `ValidationBehavior` (FluentValidation), opcional logging de handling.
- Puertos en `Abstractions` (`IUsuarioRepository`).
- Contratos del puerto = DTOs/commands de Application, no request models de API.

## Infrastructure

- Implementa puertos.
- Único lugar que conoce URL, headers, JWT técnico, API key y serialización RPC.
- `PostgrestOptions`: `BaseUrl`, `TimeoutSeconds`, `ApiKey` (opcional). Binding: sección `PostgREST`.

## API

- Controllers: reciben HTTP, envían MediatR, devuelven `ApiResponse<T>`.
- Middleware: Correlation ID, excepciones globales, request logging.
- JWT Bearer, policies, CORS, Swagger con esquema Bearer.
- `AddApplication()` + `AddInfrastructure()`.

## Envelope HTTP

Éxito:

```json
{
  "success": true,
  "message": "Operación exitosa",
  "code": "OK",
  "data": {},
  "errors": [],
  "correlationId": "..."
}
```

Error (validación o negocio):

```json
{
  "success": false,
  "message": "Se encontraron errores de validación",
  "code": "VALIDATION_ERROR",
  "data": null,
  "errors": [{ "field": "correo", "message": "El correo no es válido" }],
  "correlationId": "..."
}
```

Códigos HTTP: 200/201 éxito, 400 validación, 401/403 authz, 404 no encontrado, 409 conflicto, 502/503 PostgREST/infra.

## MediatR vs servicios de aplicación

Usar MediatR. No crear `UsuarioService` que orqueste CRUD completo: cada acción es un handler.
