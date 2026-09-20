INSERT INTO negocio.usuarios (nombre, correo, activo)
VALUES ('Usuario Demo', 'demo@miproyecto.local', TRUE)
ON CONFLICT (correo) DO NOTHING;
