-- Ejecutar desde esta carpeta:
--   psql -d miproyecto -v ON_ERROR_STOP=1 -f install.sql
--
-- Orden: roles y schemas → tablas → functions → permisos → seed.

\echo Instalando schemas y roles...
\i schemas/seguridad.sql
\i schemas/negocio.sql
\i schemas/api.sql
\i schemas/reportes.sql

\echo Instalando tablas...
\i tables/usuarios.sql

\echo Instalando functions...
\i functions/fn_usuario_obtener.sql
\i functions/fn_usuario_listar.sql
\i functions/fn_usuario_crear.sql
\i functions/fn_usuario_actualizar.sql
\i functions/fn_usuario_eliminar.sql
\i functions/fn_ejemplo_transaccion.sql

\echo Aplicando permisos PostgREST...
\i schemas/grants.sql

\echo Cargando seed...
\i seed/usuarios.sql

\echo Instalacion completada.
