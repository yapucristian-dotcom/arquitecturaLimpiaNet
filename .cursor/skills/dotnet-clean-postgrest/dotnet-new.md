# Convertir a dotnet new

Hacerlo **al final**, cuando la solución compile y el módulo Usuarios esté completo.

## Ubicación

```text
.template.config/template.json
```

## `template.json` (mínimo)

```json
{
  "$schema": "http://json.schemastore.org/template",
  "author": "MiEmpresa",
  "name": "MiEmpresa Clean Architecture PostgREST",
  "identity": "MiEmpresa.DotNet.CleanArchitecture",
  "shortName": "cleanarch",
  "sourceName": "MiProyecto",
  "preferNameDirectory": true,
  "tags": { "language": "C#", "type": "solution" },
  "symbols": {
    "skipRestore": {
      "type": "parameter",
      "datatype": "bool",
      "defaultValue": "false"
    },
    "UseJwt": {
      "type": "parameter",
      "datatype": "bool",
      "defaultValue": "true",
      "description": "Incluir autenticación JWT"
    }
  }
}
```

`sourceName: MiProyecto` reemplaza namespaces, folders y `.csproj`.

Parámetros extra (`--database`, `--auth`) solo si hay `#if` o fuentes alternativas reales. No agregar flags vacíos.

Instalación local:

```bash
dotnet new install .
dotnet new cleanarch -n SistemaRentas
```

Incluir esta skill en el output de la plantilla (`.cursor/skills/dotnet-clean-postgrest/`) para que cada sistema generado la herede.
