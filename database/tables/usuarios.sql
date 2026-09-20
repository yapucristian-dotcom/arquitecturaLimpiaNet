CREATE TABLE IF NOT EXISTS negocio.usuarios (
    id BIGSERIAL PRIMARY KEY,
    nombre VARCHAR(150) NOT NULL,
    correo VARCHAR(255) NOT NULL,
    activo BOOLEAN NOT NULL DEFAULT TRUE,
    CONSTRAINT uq_usuarios_correo UNIQUE (correo)
);

COMMENT ON TABLE negocio.usuarios IS
    'Usuarios de ejemplo de la plantilla. No es un módulo de seguridad corporativo.';

REVOKE ALL ON TABLE negocio.usuarios FROM PUBLIC;
REVOKE ALL ON SEQUENCE negocio.usuarios_id_seq FROM PUBLIC;
