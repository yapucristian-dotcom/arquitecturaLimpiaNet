# Seguridad y logging

## Tres capas

1. Autenticación: JWT Bearer en la API (quién).
2. Autorización: roles, claims, policies en endpoints (qué puede hacer en HTTP).
3. Permisos PostgreSQL: rol de PostgREST solo `EXECUTE` de Functions (qué puede hacer en datos).

No implementar un sistema de usuarios/seguridad completo en la plantilla. JWT configurable + un policy de ejemplo (`Admin`, `User`) basta.

## Configuración

```json
{
  "PostgREST": { "BaseUrl": "http://localhost:3000", "TimeoutSeconds": 30 },
  "Jwt": { "Authority": "", "Audience": "", "RequireHttpsMetadata": true }
}
```

Signing key, API keys y connection strings: User Secrets / env (`Jwt__SigningKey`, `PostgREST__ApiKey`).
`appsettings.Production.json` sin secretos. Docker: env vars.

HTTPS en Production. CORS explícito por orígenes, no `AllowAnyOrigin` con credenciales.

## SQL injection

El cliente solo envía JSON a `/rpc`. Las Functions usan parámetros (`p_`).
Prohibido concatenar SQL dentro de PL/pgSQL. Cuidado con `FORMAT` dinámico.

## Serilog

Enriquecer: CorrelationId, UserId (si existe), RequestPath, StatusCode, ElapsedMs.
Loguear fallos de PostgREST (function, status, code) **sin** body de login ni tokens.
Request logging en API middleware. Nivel Information en Dev, Warning+ en Production para requests.
