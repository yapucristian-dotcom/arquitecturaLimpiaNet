# Seguridad y logging

## Tres capas

1. Autenticación: JWT Bearer en la API (quién).
2. Autorización: roles, claims, policies en endpoints (qué puede hacer en HTTP).
3. Permisos PostgreSQL: rol de PostgREST solo `EXECUTE` de Functions (qué puede hacer en datos).

No implementar un sistema de usuarios/seguridad completo en la plantilla. JWT configurable + un policy de ejemplo (`Admin`, `User`) basta.

## Configuración

```json
{
  "Database": {
    "Provider": "PostgREST",
    "ConnectionString": "Server=localhost;Database=MiProyecto;User Id=sa;Password=Your_password123;TrustServerCertificate=True;Encrypt=False",
    "CommandTimeoutSeconds": 30
  },
  "PostgREST": { "BaseUrl": "http://localhost:3000", "TimeoutSeconds": 30 },
  "Jwt": { "Authority": "", "Audience": "", "RequireHttpsMetadata": true }
}
```

`Database:Provider` es `PostgREST` o `SqlServer`. La cadena de conexión se lee de `appsettings.json`, no de User Secrets.
Signing key y API keys: User Secrets / env (`Jwt__SigningKey`, `PostgREST__ApiKey`).
Docker con PostgreSQL fija `Database__Provider=PostgREST`.

HTTPS en Production. CORS explícito por orígenes, no `AllowAnyOrigin` con credenciales.

## SQL injection

El cliente solo envía JSON a `/rpc`. Las Functions usan parámetros (`p_`).
Prohibido concatenar SQL dentro de PL/pgSQL. Cuidado con `FORMAT` dinámico.

## Serilog

Enriquecer: CorrelationId, UserId (si existe), RequestPath, StatusCode, ElapsedMs.
Loguear fallos de PostgREST (function, status, code) **sin** body de login ni tokens.
Request logging en API middleware. Nivel Information en Dev, Warning+ en Production para requests.
