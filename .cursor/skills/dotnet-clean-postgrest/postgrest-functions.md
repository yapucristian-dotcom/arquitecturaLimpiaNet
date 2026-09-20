# PostgREST y Functions

## Cliente

```csharp
Task<T?> RpcAsync<T>(string functionName, object? args, CancellationToken ct = default);
Task<IReadOnlyList<T>> RpcListAsync<T>(string functionName, object? args, CancellationToken ct = default);
Task RpcExecuteAsync(string functionName, object? args, CancellationToken ct = default);
```

Responsabilidades: `HttpClient` tipado, BaseUrl, timeout, JSON, header JWT/API key, `X-Correlation-ID`, CancellationToken, mapear status codes, log sin payload sensible.

Nunca usar `HttpClient` a mano en un repository.

Llamada:

```csharp
await _client.RpcAsync<UsuarioDto>("fn_usuario_obtener", new { p_id = id }, ct);
```

El nombre de Function es el único acoplamiento permitido en el repository.

## Convención de nombres

```text
{schema}.fn_{modulo}_{operacion}
```

Ejemplos: `fn_usuario_obtener`, `fn_usuario_listar`, `fn_usuario_crear`,
`fn_usuario_actualizar`, `fn_usuario_eliminar`, `fn_venta_registrar`.

- Parámetros: `p_id`, `p_nombre`, `p_page`, `p_page_size`
- Listados: `RETURNS TABLE` + `p_page` / `p_page_size` / `p_search` (y total)
- CREATE: `RETURNS BIGINT` (id) o `RETURNS TABLE` de la fila
- UPDATE/DELETE: `RETURNS VOID` o fila afectada; `RAISE` si no existe
- Lectura: `LANGUAGE sql` o `plpgsql` + `STABLE`
- Escritura: `plpgsql` + `VOLATILE`

Scripts en `database/functions/fn_{modulo}_{operacion}.sql`.

## Exposición PostgREST

- `POST /rpc/fn_usuario_obtener` body `{ "p_id": 10 }`
- No exponer tablas ni views de escritura en el schema de API.
- `GRANT EXECUTE` solo de Functions públicas al rol de PostgREST.
- Tablas: privilegios al owner de las Functions (`SECURITY DEFINER`).

## Errores (canal único)

En PL/pgSQL:

```sql
RAISE EXCEPTION 'El usuario no existe'
  USING ERRCODE = 'PT404',
        DETAIL = '{"code":"USR_NOT_FOUND"}';
```

PostgREST body típico: `{ "message", "details", "hint", "code" }`.

`PostgrestClient`:

- 2xx → deserializa
- 4xx/5xx → `PostgrestException` (Message, Code, HttpStatus, Details)
- Timeout / red → excepción de infra (HTTP 503 en API)

No devolver el JSON crudo de PostgREST al cliente final.

Códigos de negocio: `USR_NOT_FOUND`, `VAL_*`, `CONFLICT`, `INTERNAL`. Prefijo por módulo.

## Transacciones

Una operación de negocio = una Function. PostgREST = una transacción por request.
`RAISE` ⇒ rollback. Functions internas no expuestas para pasos (stock, movimiento).
No abrir transacción en C# ni encadenar varios RPC para una sola unidad de trabajo.

## Ejemplos mínimos que la plantilla debe incluir

- SELECT por id
- INSERT RETURNING id
- UPDATE + not found
- DELETE lógico o físico (documentar cuál)
- Listado paginado
- Orquestadora (comentario + stub `fn_ejemplo_transaccion`) para ventas futuras
