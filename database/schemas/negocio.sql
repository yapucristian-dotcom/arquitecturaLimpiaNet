CREATE SCHEMA IF NOT EXISTS negocio;

COMMENT ON SCHEMA negocio IS
    'Tablas y functions internas. No se expone a PostgREST.';

REVOKE ALL ON SCHEMA negocio FROM PUBLIC;
