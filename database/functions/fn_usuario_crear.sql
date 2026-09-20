CREATE OR REPLACE FUNCTION api.fn_usuario_crear(
    p_nombre VARCHAR,
    p_correo VARCHAR
)
RETURNS BIGINT
LANGUAGE plpgsql
VOLATILE
SECURITY DEFINER
SET search_path = negocio, pg_temp
AS $$
DECLARE
    v_id BIGINT;
BEGIN
    INSERT INTO negocio.usuarios (nombre, correo, activo)
    VALUES (btrim(p_nombre), btrim(p_correo), TRUE)
    RETURNING negocio.usuarios.id INTO v_id;

    RETURN v_id;
EXCEPTION
    WHEN unique_violation THEN
        RAISE EXCEPTION 'El correo ya está registrado'
            USING ERRCODE = 'PT409',
                  DETAIL = '{"code":"CONFLICT"}';
END;
$$;

COMMENT ON FUNCTION api.fn_usuario_crear(VARCHAR, VARCHAR) IS
    'Crea un usuario y devuelve el id. POST /rpc/fn_usuario_crear';
