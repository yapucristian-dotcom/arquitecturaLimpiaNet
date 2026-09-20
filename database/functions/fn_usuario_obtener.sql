CREATE OR REPLACE FUNCTION api.fn_usuario_obtener(
    p_id BIGINT
)
RETURNS TABLE (
    id BIGINT,
    nombre VARCHAR,
    correo VARCHAR,
    activo BOOLEAN
)
LANGUAGE plpgsql
STABLE
SECURITY DEFINER
SET search_path = negocio, pg_temp
AS $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM negocio.usuarios u
        WHERE u.id = p_id
    ) THEN
        RAISE EXCEPTION 'El usuario no existe'
            USING ERRCODE = 'PT404',
                  DETAIL = '{"code":"USR_NOT_FOUND"}';
    END IF;

    RETURN QUERY
    SELECT
        u.id,
        u.nombre,
        u.correo,
        u.activo
    FROM negocio.usuarios u
    WHERE u.id = p_id;
END;
$$;

COMMENT ON FUNCTION api.fn_usuario_obtener(BIGINT) IS
    'Obtiene un usuario por id. POST /rpc/fn_usuario_obtener';
