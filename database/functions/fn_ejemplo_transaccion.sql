-- Orquestadora de ejemplo. Vive en negocio y NO se concede a api_user.
-- Toda la operación corre en la transacción que abre PostgREST al llamar
-- una sola Function pública. Un RAISE hace rollback de todos los pasos.
--
-- Patrón para un módulo futuro (ventas, trámites, etc.):
--   api.fn_venta_registrar        ← única Function expuesta
--     negocio.fn_venta_validar_cliente
--     negocio.fn_venta_validar_stock
--     negocio.fn_venta_insertar
--     negocio.fn_venta_insertar_detalle
--     negocio.fn_stock_actualizar
--     negocio.fn_movimiento_registrar

CREATE OR REPLACE FUNCTION negocio.fn_ejemplo_transaccion()
RETURNS VOID
LANGUAGE plpgsql
VOLATILE
SET search_path = negocio, pg_temp
AS $$
BEGIN
    RAISE NOTICE 'Stub fn_ejemplo_transaccion: reemplazar por la orquestación de negocio.';
END;
$$;

COMMENT ON FUNCTION negocio.fn_ejemplo_transaccion() IS
    'Stub de transacción orquestada. No exponer en PostgREST.';
