-- SQL Server. Ejecutar sobre la base configurada en appsettings.json (Database:ConnectionString).
--   sqlcmd -S localhost -d MiProyecto -E -i database/sqlserver/install.sql
--
-- La API solo llama a los procedimientos api.fn_usuario_*.
-- El login de la cadena necesita EXECUTE sobre el esquema api.

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'negocio')
    EXEC(N'CREATE SCHEMA negocio');
GO

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'api')
    EXEC(N'CREATE SCHEMA api');
GO

IF OBJECT_ID(N'negocio.usuarios', N'U') IS NULL
BEGIN
    CREATE TABLE negocio.usuarios (
        id BIGINT IDENTITY(1, 1) NOT NULL CONSTRAINT pk_usuarios PRIMARY KEY,
        nombre NVARCHAR(150) NOT NULL,
        correo NVARCHAR(255) NOT NULL,
        activo BIT NOT NULL CONSTRAINT df_usuarios_activo DEFAULT (1),
        CONSTRAINT uq_usuarios_correo UNIQUE (correo)
    );
END
GO

CREATE OR ALTER PROCEDURE api.fn_usuario_obtener
    @p_id BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM negocio.usuarios WHERE id = @p_id)
        THROW 50404, N'El usuario no existe', 1;

    SELECT
        u.id,
        u.nombre,
        u.correo,
        u.activo
    FROM negocio.usuarios u
    WHERE u.id = @p_id;
END
GO

CREATE OR ALTER PROCEDURE api.fn_usuario_listar
    @p_page INT = 1,
    @p_page_size INT = 20,
    @p_search NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @page INT = CASE WHEN ISNULL(@p_page, 1) < 1 THEN 1 ELSE @p_page END;
    DECLARE @page_size INT = CASE
        WHEN ISNULL(@p_page_size, 20) < 1 THEN 1
        WHEN @p_page_size > 100 THEN 100
        ELSE @p_page_size
    END;

    SELECT
        u.id,
        u.nombre,
        u.correo,
        u.activo,
        COUNT(*) OVER () AS total_count
    FROM negocio.usuarios u
    WHERE @p_search IS NULL
       OR LTRIM(RTRIM(@p_search)) = N''
       OR u.nombre COLLATE Latin1_General_CI_AI LIKE N'%' + @p_search + N'%' COLLATE Latin1_General_CI_AI
       OR u.correo COLLATE Latin1_General_CI_AI LIKE N'%' + @p_search + N'%' COLLATE Latin1_General_CI_AI
    ORDER BY u.id
    OFFSET (@page - 1) * @page_size ROWS FETCH NEXT @page_size ROWS ONLY;
END
GO

CREATE OR ALTER PROCEDURE api.fn_usuario_crear
    @p_nombre NVARCHAR(150),
    @p_correo NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO negocio.usuarios (nombre, correo, activo)
        VALUES (LTRIM(RTRIM(@p_nombre)), LTRIM(RTRIM(@p_correo)), 1);

        SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
    END TRY
    BEGIN CATCH
        IF ERROR_NUMBER() IN (2601, 2627)
            THROW 50409, N'El correo ya está registrado', 1;

        THROW;
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE api.fn_usuario_actualizar
    @p_id BIGINT,
    @p_nombre NVARCHAR(150),
    @p_correo NVARCHAR(255),
    @p_activo BIT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        UPDATE negocio.usuarios
        SET
            nombre = LTRIM(RTRIM(@p_nombre)),
            correo = LTRIM(RTRIM(@p_correo)),
            activo = @p_activo
        WHERE id = @p_id;

        IF @@ROWCOUNT = 0
            THROW 50404, N'El usuario no existe', 1;
    END TRY
    BEGIN CATCH
        IF ERROR_NUMBER() IN (2601, 2627)
            THROW 50409, N'El correo ya está registrado', 1;

        THROW;
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE api.fn_usuario_eliminar
    @p_id BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE negocio.usuarios
    SET activo = 0
    WHERE id = @p_id
      AND activo = 1;

    IF @@ROWCOUNT = 0
        THROW 50404, N'El usuario no existe', 1;
END
GO

IF NOT EXISTS (SELECT 1 FROM negocio.usuarios WHERE correo = N'demo@miproyecto.local')
    INSERT INTO negocio.usuarios (nombre, correo, activo)
    VALUES (N'Usuario Demo', N'demo@miproyecto.local', 1);
GO
