CREATE OR REPLACE FUNCTION api.fn_usuario_listar(
    p_page INT DEFAULT 1,
    p_page_size INT DEFAULT 20,
    p_search VARCHAR DEFAULT NULL
)
RETURNS TABLE (
    id BIGINT,
    nombre VARCHAR,
    correo VARCHAR,
    activo BOOLEAN,
    total_count INT
)
LANGUAGE plpgsql
STABLE
SECURITY DEFINER
SET search_path = negocio, pg_temp
AS $$
DECLARE
    v_page INT := GREATEST(COALESCE(p_page, 1), 1);
    v_page_size INT := LEAST(GREATEST(COALESCE(p_page_size, 20), 1), 100);
BEGIN
    RETURN QUERY
    SELECT
        u.id,
        u.nombre,
        u.correo,
        u.activo,
        COUNT(*) OVER ()::INT AS total_count
    FROM negocio.usuarios u
    WHERE p_search IS NULL
       OR btrim(p_search) = ''
       OR u.nombre ILIKE '%' || p_search || '%'
       OR u.correo ILIKE '%' || p_search || '%'
    ORDER BY u.id
    OFFSET (v_page - 1) * v_page_size
    LIMIT v_page_size;
END;
$$;

COMMENT ON FUNCTION api.fn_usuario_listar(INT, INT, VARCHAR) IS
    'Lista usuarios paginados. POST /rpc/fn_usuario_listar';
