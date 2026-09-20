CREATE OR REPLACE FUNCTION api.fn_usuario_actualizar(
    p_id BIGINT,
    p_nombre VARCHAR,
    p_correo VARCHAR,
    p_activo BOOLEAN
)
RETURNS VOID
LANGUAGE plpgsql
VOLATILE
SECURITY DEFINER
SET search_path = negocio, pg_temp
AS $$
BEGIN
    UPDATE negocio.usuarios
    SET
        nombre = btrim(p_nombre),
        correo = btrim(p_correo),
        activo = p_activo
    WHERE id = p_id;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'El usuario no existe'
            USING ERRCODE = 'PT404',
                  DETAIL = '{"code":"USR_NOT_FOUND"}';
    END IF;
EXCEPTION
    WHEN unique_violation THEN
        RAISE EXCEPTION 'El correo ya está registrado'
            USING ERRCODE = 'PT409',
                  DETAIL = '{"code":"CONFLICT"}';
END;
$$;

COMMENT ON FUNCTION api.fn_usuario_actualizar(BIGINT, VARCHAR, VARCHAR, BOOLEAN) IS
    'Actualiza un usuario. POST /rpc/fn_usuario_actualizar';
