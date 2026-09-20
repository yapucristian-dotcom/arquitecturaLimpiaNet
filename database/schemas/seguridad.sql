-- Roles técnicos de PostgREST.
-- authenticator: usuario LOGIN que usa PostgREST (NOINHERIT).
-- api_user: rol de ejecución; solo EXECUTE de functions en schema api.
-- La contraseña es solo para Development. En QA/Production use un secreto real.

DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'authenticator') THEN
        CREATE ROLE authenticator LOGIN PASSWORD 'changeme_dev_only' NOINHERIT;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'api_user') THEN
        CREATE ROLE api_user NOLOGIN;
    END IF;
END
$$;

GRANT api_user TO authenticator;
