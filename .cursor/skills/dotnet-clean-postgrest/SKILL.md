---
name: dotnet-clean-postgrest
description: Guides building and extending the corporate .NET Clean Architecture template that accesses PostgreSQL only through PostgREST RPC and PostgreSQL Functions. Use when creating or evolving the template, adding modules (Usuarios, Rentas, Inventario, Tramites), writing PL/pgSQL functions, the PostgREST client, JWT, Serilog, FluentValidation, MediatR, tests, Docker, or converting the solution into a dotnet new template.
---

# Plantilla .NET Clean Architecture + PostgREST

Plantilla corporativa reutilizable. Cada sistema nuevo (rentas, inventario, trámites) solo agrega módulos y Functions. No acoplar la plantilla a un negocio específico.

## Reglas no negociables

- Target: `net10.0` (LTS). Language: C#. DI nativo. Config por Options Pattern.
- Sin Entity Framework Core como acceso a datos.
- .NET **nunca** lee/escribe tablas. Solo PostgreSQL Functions vía PostgREST RPC (`POST /rpc/{nombre}`).
- Application **no** referencia PostgREST, HttpClient, SQL ni ASP.NET.
- Domain **no** referencia Application, Infrastructure ni API.
- `IPostgrestClient` vive solo en Infrastructure.
- Repositorios **específicos por módulo**. Prohibido un `IGenericRepository`.
- Secretos fuera del repo (User Secrets, env vars, vault).
- No registrar passwords, JWT, API keys ni PII innecesaria.
- Responder en español al usuario. Código e identificadores en el idioma ya usado por la plantilla (`Usuario`, `fn_usuario_obtener`).

## Flujo obligatorio

```text
Controller → MediatR (Command/Query) → FluentValidation
  → Handler → I{Modulo}Repository → PostgrestClient
  → POST /rpc/fn_{modulo}_{operacion} → PostgreSQL Function → tablas
```

Transacciones de negocio = **una Function orquestadora**. No transacciones repartidas entre varios RPC desde C#.

## Placeholder de plantilla

Usar `MiProyecto` como `sourceName` para `dotnet new`:

- Namespaces: `MiProyecto.Domain`, `MiProyecto.Application`, `MiProyecto.Infrastructure`, `MiProyecto.API`
- Solution: `MiProyecto.sln`
- Tests: `MiProyecto.UnitTests`, `MiProyecto.IntegrationTests`

No hardcodear nombres de sistemas reales (Rentas, Inventario) fuera del módulo de ejemplo `Usuarios`.

## Orden de construcción (no saltar pasos)

1. Solution `.sln` + proyectos `net10.0` + referencias
2. Carpetas base + `Application/Common` (envelope, errores, paginación)
3. Domain delgado (Usuario de ejemplo)
4. Application: interfaces, features Usuarios, DI, MediatR, FluentValidation
5. Infrastructure: Options, `PostgrestClient`, `UsuarioRepository`, Serilog sinks de infra, DI
6. API: Program, middleware, JWT, Swagger, controllers delgados
7. `database/` (schemas, tables, functions, seed) + config PostgREST
8. Unit tests (handlers + validators)
9. Integration tests (WebApplicationFactory; Testcontainers si hay Docker)
10. Docker opcional (`docker-compose`: postgres, postgrest, api)
11. `.template.config/template.json`

## Checklist por cambio

- [ ] Ninguna capa viola las dependencias
- [ ] Nueva operación de datos = Function + RPC + método de repositorio específico
- [ ] Parámetros de Function con prefijo `p_`
- [ ] Validación FluentValidation **antes** del RPC
- [ ] Errores de PostgREST mapeados al envelope estándar
- [ ] Tests del caso de uso (unit) y contrato HTTP si aplica
- [ ] Sin secretos nuevos en `appsettings.json`

## Recursos

- Capas, carpetas, DI y MediatR: [architecture.md](architecture.md)
- Cliente RPC, Functions, errores y transacciones: [postgrest-functions.md](postgrest-functions.md)
- Cómo agregar un módulo clonando Usuarios: [module-playbook.md](module-playbook.md)
- JWT, CORS, secretos, Serilog, permisos DB: [security-logging.md](security-logging.md)
- Convertir a `dotnet new`: [dotnet-new.md](dotnet-new.md)
