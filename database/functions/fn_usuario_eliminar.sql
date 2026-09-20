-- Eliminación lógica: desactiva el registro (activo = false).
-- No se borra la fila. Documentar este criterio en cada módulo nuevo.

CREATE OR REPLACE FUNCTION api.fn_usuario_eliminar(
    p_id BIGINT
)
RETURNS VOID
LANGUAGE plpgsql
VOLATILE
SECURITY DEFINER
SET search_path = negocio, pg_temp
AS $$
BEGIN
    UPDATE negocio.usuarios
    SET activo = FALSE
    WHERE id = p_id
      AND activo = TRUE;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'El usuario no existe'
            USING ERRCODE = 'PT404',
                  DETAIL = '{"code":"USR_NOT_FOUND"}';
    END IF;
END;
$$;

COMMENT ON FUNCTION api.fn_usuario_eliminar(BIGINT) IS
    'Eliminación lógica (activo = false). POST /rpc/fn_usuario_eliminar';
