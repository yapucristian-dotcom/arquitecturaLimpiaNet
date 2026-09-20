CREATE SCHEMA IF NOT EXISTS api;

COMMENT ON SCHEMA api IS
    'Functions públicas para PostgREST RPC. Sin tablas.';

REVOKE ALL ON SCHEMA api FROM PUBLIC;
GRANT USAGE ON SCHEMA api TO api_user;
