CREATE SCHEMA IF NOT EXISTS reportes;

COMMENT ON SCHEMA reportes IS
    'Reservado para reportes. No se expone a PostgREST en la plantilla.';

REVOKE ALL ON SCHEMA reportes FROM PUBLIC;
