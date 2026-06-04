USE [BDCAPAS]
GO
/****** Object:  Table [dbo].[PERSONA]    Script Date: 27/04/2026 1:27:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PERSONA](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[NOMBRE] [varchar](50) NULL,
	[APELLIDO] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[USUARIO]    Script Date: 27/04/2026 1:27:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[USUARIO](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[USUARIO] [varchar](50) NULL,
	[PASS] [varchar](64) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[PERSONA_BORRAR]    Script Date: 27/04/2026 1:27:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PERSONA_BORRAR]
    @ID INT
AS
    DELETE FROM PERSONA WHERE ID = @ID
GO
/****** Object:  StoredProcedure [dbo].[PERSONA_EDITAR]    Script Date: 27/04/2026 1:27:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PERSONA_EDITAR]
    @ID  INT,
    @nom VARCHAR(50),
    @ape VARCHAR(50)
AS
    UPDATE PERSONA SET NOMBRE = @nom, APELLIDO = @ape WHERE ID = @ID
GO
/****** Object:  StoredProcedure [dbo].[PERSONA_INSERTAR]    Script Date: 27/04/2026 1:27:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PERSONA_INSERTAR]
    @nom VARCHAR(50),
    @ape VARCHAR(50)
AS
    INSERT INTO PERSONA (NOMBRE, APELLIDO) VALUES (@nom, @ape)
GO
/****** Object:  StoredProcedure [dbo].[PERSONA_LISTAR]    Script Date: 27/04/2026 1:27:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PERSONA_LISTAR]
AS
    SELECT ID, NOMBRE, APELLIDO FROM PERSONA
GO
/****** Object:  StoredProcedure [dbo].[USUARIO_LOGIN]    Script Date: 27/04/2026 1:27:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[USUARIO_LOGIN]
    @usuario VARCHAR(64),
    @pass    VARCHAR(64)
AS
    SELECT ID, USUARIO 
    FROM USUARIO
    WHERE USUARIO = @usuario AND PASS = @pass
GO
/****** Datos iniciales ******/
IF NOT EXISTS (SELECT 1 FROM USUARIO WHERE USUARIO = 'admin')
    INSERT INTO USUARIO (USUARIO, PASS)
    VALUES ('admin', '03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4')
GO

-- Gestión de perfiles y permisos (patrón Composite)
IF OBJECT_ID('dbo.NODO_PERMISO', 'U') IS NULL
CREATE TABLE [dbo].[NODO_PERMISO](
    [ID]       INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [NOMBRE]   VARCHAR(100)      NOT NULL,
    [TIPO]     VARCHAR(10)       NOT NULL,  -- 'PERFIL' | 'PERMISO'
    [PADRE_ID] INT               NULL
)
GO

IF OBJECT_ID('dbo.USUARIO_PERFIL', 'U') IS NULL
CREATE TABLE [dbo].[USUARIO_PERFIL](
    [USUARIO_ID] INT NOT NULL,
    [PERFIL_ID]  INT NOT NULL,
    PRIMARY KEY ([USUARIO_ID], [PERFIL_ID])
)
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'PERFIL_LISTAR_TODOS')
    DROP PROCEDURE [dbo].[PERFIL_LISTAR_TODOS]
GO
CREATE PROCEDURE [dbo].[PERFIL_LISTAR_TODOS]
AS
    SELECT ID, NOMBRE, TIPO, PADRE_ID FROM NODO_PERMISO ORDER BY ID
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'PERFIL_INSERTAR')
    DROP PROCEDURE [dbo].[PERFIL_INSERTAR]
GO
CREATE PROCEDURE [dbo].[PERFIL_INSERTAR]
    @nombre   VARCHAR(100),
    @tipo     VARCHAR(10),
    @padre_id INT = NULL
AS
BEGIN
    INSERT INTO NODO_PERMISO (NOMBRE, TIPO, PADRE_ID) VALUES (@nombre, @tipo, @padre_id)
    SELECT SCOPE_IDENTITY() AS ID
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'PERFIL_ELIMINAR')
    DROP PROCEDURE [dbo].[PERFIL_ELIMINAR]
GO
CREATE PROCEDURE [dbo].[PERFIL_ELIMINAR]
    @id INT
AS
BEGIN
    -- Recolecta el nodo raíz y todos sus descendientes con una CTE recursiva
    DECLARE @ids TABLE (ID INT)
    ;WITH Descendientes AS (
        SELECT ID FROM NODO_PERMISO WHERE ID = @id
        UNION ALL
        SELECT np.ID FROM NODO_PERMISO np
            INNER JOIN Descendientes d ON np.PADRE_ID = d.ID
    )
    INSERT INTO @ids SELECT ID FROM Descendientes

    DELETE FROM USUARIO_PERFIL WHERE PERFIL_ID IN (SELECT ID FROM @ids)
    DELETE FROM NODO_PERMISO    WHERE ID        IN (SELECT ID FROM @ids)
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'USUARIO_LISTAR_TODOS')
    DROP PROCEDURE [dbo].[USUARIO_LISTAR_TODOS]
GO
CREATE PROCEDURE [dbo].[USUARIO_LISTAR_TODOS]
AS
    SELECT ID, USUARIO, ROL, BLOQUEADO FROM USUARIO ORDER BY USUARIO
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'USUARIO_ELIMINAR')
    DROP PROCEDURE [dbo].[USUARIO_ELIMINAR]
GO
CREATE PROCEDURE [dbo].[USUARIO_ELIMINAR]
    @id INT
AS
BEGIN
    DELETE FROM USUARIO_PERFIL WHERE USUARIO_ID = @id
    DELETE FROM USUARIO         WHERE ID         = @id
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'USUARIO_CREAR')
    DROP PROCEDURE [dbo].[USUARIO_CREAR]
GO
CREATE PROCEDURE [dbo].[USUARIO_CREAR]
    @usuario VARCHAR(50),
    @pass    VARCHAR(64),
    @rol     VARCHAR(20)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM USUARIO WHERE USUARIO = @usuario)
    BEGIN
        SELECT 0 AS OK
        RETURN
    END
    INSERT INTO USUARIO (USUARIO, PASS, ROL) VALUES (@usuario, @pass, @rol)
    SELECT 1 AS OK
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'USUARIO_PERMISOS_LISTAR')
    DROP PROCEDURE [dbo].[USUARIO_PERMISOS_LISTAR]
GO
CREATE PROCEDURE [dbo].[USUARIO_PERMISOS_LISTAR]
    @usuario_id INT
AS
BEGIN
    -- Recorre recursivamente todos los nodos asignados al usuario y devuelve solo los permisos (hojas)
    ;WITH Arbol AS (
        SELECT np.ID, np.NOMBRE, np.TIPO
        FROM USUARIO_PERFIL up
        JOIN NODO_PERMISO np ON np.ID = up.PERFIL_ID
        WHERE up.USUARIO_ID = @usuario_id

        UNION ALL

        SELECT np.ID, np.NOMBRE, np.TIPO
        FROM NODO_PERMISO np
        JOIN Arbol a ON np.PADRE_ID = a.ID
    )
    SELECT DISTINCT NOMBRE FROM Arbol WHERE TIPO = 'PERMISO'
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'USUARIO_PERFIL_LISTAR')
    DROP PROCEDURE [dbo].[USUARIO_PERFIL_LISTAR]
GO
CREATE PROCEDURE [dbo].[USUARIO_PERFIL_LISTAR]
    @usuario_id INT
AS
    SELECT PERFIL_ID FROM USUARIO_PERFIL WHERE USUARIO_ID = @usuario_id
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'USUARIO_PERFIL_BORRAR_TODOS')
    DROP PROCEDURE [dbo].[USUARIO_PERFIL_BORRAR_TODOS]
GO
CREATE PROCEDURE [dbo].[USUARIO_PERFIL_BORRAR_TODOS]
    @usuario_id INT
AS
    DELETE FROM USUARIO_PERFIL WHERE USUARIO_ID = @usuario_id
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'USUARIO_PERFIL_ASIGNAR')
    DROP PROCEDURE [dbo].[USUARIO_PERFIL_ASIGNAR]
GO
CREATE PROCEDURE [dbo].[USUARIO_PERFIL_ASIGNAR]
    @usuario_id INT,
    @perfil_id  INT
AS
    INSERT INTO USUARIO_PERFIL (USUARIO_ID, PERFIL_ID) VALUES (@usuario_id, @perfil_id)
GO

-- crear sp para cambiar contrase;a

USE [BDCAPAS]
GO

CREATE PROCEDURE [dbo].[USUARIO_CAMBIAR_PASS]
    @usuario VARCHAR(50),
    @pass    VARCHAR(64)
AS
    UPDATE USUARIO 
    SET PASS = @pass 
    WHERE USUARIO = @usuario
GO

CREATE PROCEDURE [dbo].[BITACORA_INSERTAR]
    @usuario VARCHAR(50),
    @accion  VARCHAR(50)
AS
    INSERT INTO BITACORA (USUARIO, ACCION, FECHA)
    VALUES (@usuario, @accion, GETDATE())
GO

CREATE PROCEDURE [dbo].[BITACORA_LISTAR]
AS
    SELECT ID, USUARIO, ACCION, FECHA 
    FROM BITACORA
    ORDER BY FECHA DESC
GO 
CREATE TABLE [dbo].[BITACORA](
    [ID]      [int] IDENTITY(1,1) NOT NULL,
    [USUARIO] [varchar](50) NULL,
    [ACCION]  [varchar](50) NULL,
    [FECHA]   [datetime] NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
)
GO

-- Bloqueo por intentos fallidos (idempotente: se puede correr varias veces)
IF COL_LENGTH('dbo.USUARIO', 'INTENTOS_FALLIDOS') IS NULL
    ALTER TABLE [dbo].[USUARIO] ADD [INTENTOS_FALLIDOS] INT NOT NULL DEFAULT 0
GO
IF COL_LENGTH('dbo.USUARIO', 'BLOQUEADO') IS NULL
    ALTER TABLE [dbo].[USUARIO] ADD [BLOQUEADO] BIT NOT NULL DEFAULT 0
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'USUARIO_VERIFICAR_BLOQUEO')
    DROP PROCEDURE [dbo].[USUARIO_VERIFICAR_BLOQUEO]
GO
CREATE PROCEDURE [dbo].[USUARIO_VERIFICAR_BLOQUEO]
    @usuario VARCHAR(50)
AS
    SELECT BLOQUEADO FROM USUARIO WHERE USUARIO = @usuario
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'USUARIO_INCREMENTAR_INTENTOS')
    DROP PROCEDURE [dbo].[USUARIO_INCREMENTAR_INTENTOS]
GO
CREATE PROCEDURE [dbo].[USUARIO_INCREMENTAR_INTENTOS]
    @usuario VARCHAR(50)
AS
BEGIN
    UPDATE USUARIO
    SET INTENTOS_FALLIDOS = INTENTOS_FALLIDOS + 1,
        BLOQUEADO = CASE WHEN INTENTOS_FALLIDOS + 1 >= 3 THEN 1 ELSE BLOQUEADO END
    WHERE USUARIO = @usuario

    SELECT BLOQUEADO FROM USUARIO WHERE USUARIO = @usuario
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'USUARIO_RESETEAR_INTENTOS')
    DROP PROCEDURE [dbo].[USUARIO_RESETEAR_INTENTOS]
GO
CREATE PROCEDURE [dbo].[USUARIO_RESETEAR_INTENTOS]
    @usuario VARCHAR(50)
AS
    UPDATE USUARIO
    SET INTENTOS_FALLIDOS = 0
    WHERE USUARIO = @usuario
GO

-- Roles y administración de usuarios (idempotente)
IF COL_LENGTH('dbo.USUARIO', 'ROL') IS NULL
    ALTER TABLE [dbo].[USUARIO] ADD [ROL] VARCHAR(20) NOT NULL DEFAULT 'usuario'
GO

UPDATE [dbo].[USUARIO] SET ROL = 'admin' WHERE USUARIO = 'admin'
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'USUARIO_LOGIN')
    DROP PROCEDURE [dbo].[USUARIO_LOGIN]
GO
CREATE PROCEDURE [dbo].[USUARIO_LOGIN]
    @usuario VARCHAR(64),
    @pass    VARCHAR(64)
AS
    SELECT ID, USUARIO, ROL
    FROM USUARIO
    WHERE USUARIO = @usuario AND PASS = @pass
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'USUARIO_LISTAR_BLOQUEADOS')
    DROP PROCEDURE [dbo].[USUARIO_LISTAR_BLOQUEADOS]
GO
CREATE PROCEDURE [dbo].[USUARIO_LISTAR_BLOQUEADOS]
AS
    SELECT ID, USUARIO, ROL, INTENTOS_FALLIDOS
    FROM USUARIO
    WHERE BLOQUEADO = 1
    ORDER BY USUARIO
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'USUARIO_DESBLOQUEAR')
    DROP PROCEDURE [dbo].[USUARIO_DESBLOQUEAR]
GO
CREATE PROCEDURE [dbo].[USUARIO_DESBLOQUEAR]
    @usuario VARCHAR(50)
AS
    UPDATE USUARIO
    SET BLOQUEADO = 0,
        INTENTOS_FALLIDOS = 0
    WHERE USUARIO = @usuario
GO

-- Datos iniciales: perfiles y permisos (idempotente)
-- Perfiles raíz
IF NOT EXISTS (SELECT 1 FROM NODO_PERMISO WHERE NOMBRE = 'Administrador' AND TIPO = 'PERFIL' AND PADRE_ID IS NULL)
    INSERT INTO NODO_PERMISO (NOMBRE, TIPO, PADRE_ID) VALUES ('Administrador', 'PERFIL', NULL)
GO
IF NOT EXISTS (SELECT 1 FROM NODO_PERMISO WHERE NOMBRE = 'Usuario' AND TIPO = 'PERFIL' AND PADRE_ID IS NULL)
    INSERT INTO NODO_PERMISO (NOMBRE, TIPO, PADRE_ID) VALUES ('Usuario', 'PERFIL', NULL)
GO

-- Permisos del perfil Administrador (se busca el ID por nombre para no depender del orden de inserción)
DECLARE @idAdmin INT = (SELECT ID FROM NODO_PERMISO WHERE NOMBRE = 'Administrador' AND TIPO = 'PERFIL' AND PADRE_ID IS NULL)

IF NOT EXISTS (SELECT 1 FROM NODO_PERMISO WHERE NOMBRE = 'Cambiar contraseña'            AND PADRE_ID = @idAdmin)
    INSERT INTO NODO_PERMISO (NOMBRE, TIPO, PADRE_ID) VALUES ('Cambiar contraseña',            'PERMISO', @idAdmin)
IF NOT EXISTS (SELECT 1 FROM NODO_PERMISO WHERE NOMBRE = 'Ver bitácora'                  AND PADRE_ID = @idAdmin)
    INSERT INTO NODO_PERMISO (NOMBRE, TIPO, PADRE_ID) VALUES ('Ver bitácora',                  'PERMISO', @idAdmin)
IF NOT EXISTS (SELECT 1 FROM NODO_PERMISO WHERE NOMBRE = 'Gestionar usuarios bloqueados' AND PADRE_ID = @idAdmin)
    INSERT INTO NODO_PERMISO (NOMBRE, TIPO, PADRE_ID) VALUES ('Gestionar usuarios bloqueados', 'PERMISO', @idAdmin)
IF NOT EXISTS (SELECT 1 FROM NODO_PERMISO WHERE NOMBRE = 'Gestionar perfiles y permisos' AND PADRE_ID = @idAdmin)
    INSERT INTO NODO_PERMISO (NOMBRE, TIPO, PADRE_ID) VALUES ('Gestionar perfiles y permisos', 'PERMISO', @idAdmin)
IF NOT EXISTS (SELECT 1 FROM NODO_PERMISO WHERE NOMBRE = 'Asignar perfiles a usuarios'   AND PADRE_ID = @idAdmin)
    INSERT INTO NODO_PERMISO (NOMBRE, TIPO, PADRE_ID) VALUES ('Asignar perfiles a usuarios',   'PERMISO', @idAdmin)
GO

-- Permisos del perfil Usuario
DECLARE @idUsuario INT = (SELECT TOP 1 ID FROM NODO_PERMISO WHERE NOMBRE = 'Usuario' AND TIPO = 'PERFIL' AND PADRE_ID IS NULL)

IF NOT EXISTS (SELECT 1 FROM NODO_PERMISO WHERE NOMBRE = 'Cambiar contraseña' AND PADRE_ID = @idUsuario)
    INSERT INTO NODO_PERMISO (NOMBRE, TIPO, PADRE_ID) VALUES ('Cambiar contraseña', 'PERMISO', @idUsuario)
GO

-- Asignar usuario 'admin' al perfil Administrador
DECLARE @idAdminUser   INT = (SELECT TOP 1 ID FROM USUARIO      WHERE USUARIO = 'admin')
DECLARE @idAdminPerfil INT = (SELECT TOP 1 ID FROM NODO_PERMISO WHERE NOMBRE  = 'Administrador' AND TIPO = 'PERFIL' AND PADRE_ID IS NULL)

IF @idAdminUser IS NOT NULL AND @idAdminPerfil IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM USUARIO_PERFIL WHERE USUARIO_ID = @idAdminUser AND PERFIL_ID = @idAdminPerfil)
        INSERT INTO USUARIO_PERFIL (USUARIO_ID, PERFIL_ID) VALUES (@idAdminUser, @idAdminPerfil)
END
GO

-- ============================================================
-- MÓDULO DE IDIOMAS (patrón Observer)
-- ============================================================

IF OBJECT_ID('dbo.IDIOMA', 'U') IS NULL
CREATE TABLE [dbo].[IDIOMA] (
    [ID]         INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [NOMBRE]     VARCHAR(50)       NOT NULL,
    [HABILITADO] BIT               NOT NULL DEFAULT 1
)
GO

IF OBJECT_ID('dbo.CONTROL_IDIOMA', 'U') IS NULL
CREATE TABLE [dbo].[CONTROL_IDIOMA] (
    [ID]            INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [CLAVE]         VARCHAR(100)      NOT NULL UNIQUE,
    [TEXTO_DEFAULT] VARCHAR(200)      NOT NULL
)
GO

IF OBJECT_ID('dbo.TRADUCCION', 'U') IS NULL
CREATE TABLE [dbo].[TRADUCCION] (
    [IDIOMA_ID]  INT          NOT NULL,
    [CONTROL_ID] INT          NOT NULL,
    [TEXTO]      VARCHAR(200) NOT NULL,
    PRIMARY KEY ([IDIOMA_ID], [CONTROL_ID]),
    FOREIGN KEY ([IDIOMA_ID])  REFERENCES IDIOMA(ID),
    FOREIGN KEY ([CONTROL_ID]) REFERENCES CONTROL_IDIOMA(ID)
)
GO

-- Datos iniciales: idiomas base
IF NOT EXISTS (SELECT 1 FROM IDIOMA WHERE NOMBRE = 'Español')
    INSERT INTO IDIOMA (NOMBRE, HABILITADO) VALUES ('Español', 1)
GO
IF NOT EXISTS (SELECT 1 FROM IDIOMA WHERE NOMBRE = 'Inglés')
    INSERT INTO IDIOMA (NOMBRE, HABILITADO) VALUES ('Inglés', 1)
GO

-- Stored procedures IDIOMA
IF EXISTS (SELECT * FROM sys.objects WHERE type='P' AND name='IDIOMA_LISTAR_TODOS')
    DROP PROCEDURE [dbo].[IDIOMA_LISTAR_TODOS]
GO
CREATE PROCEDURE [dbo].[IDIOMA_LISTAR_TODOS]
AS
    SELECT ID, NOMBRE, HABILITADO FROM IDIOMA ORDER BY NOMBRE
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type='P' AND name='IDIOMA_LISTAR_HABILITADOS')
    DROP PROCEDURE [dbo].[IDIOMA_LISTAR_HABILITADOS]
GO
CREATE PROCEDURE [dbo].[IDIOMA_LISTAR_HABILITADOS]
AS
    SELECT ID, NOMBRE, HABILITADO FROM IDIOMA WHERE HABILITADO = 1 ORDER BY NOMBRE
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type='P' AND name='IDIOMA_INSERTAR')
    DROP PROCEDURE [dbo].[IDIOMA_INSERTAR]
GO
CREATE PROCEDURE [dbo].[IDIOMA_INSERTAR]
    @nombre     VARCHAR(50),
    @habilitado BIT
AS
BEGIN
    INSERT INTO IDIOMA (NOMBRE, HABILITADO) VALUES (@nombre, @habilitado)
    SELECT SCOPE_IDENTITY() AS ID
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type='P' AND name='IDIOMA_RENOMBRAR')
    DROP PROCEDURE [dbo].[IDIOMA_RENOMBRAR]
GO
CREATE PROCEDURE [dbo].[IDIOMA_RENOMBRAR]
    @id     INT,
    @nombre NVARCHAR(100)
AS
    UPDATE IDIOMA SET NOMBRE = @nombre WHERE ID = @id
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type='P' AND name='IDIOMA_ACTUALIZAR_ESTADO')
    DROP PROCEDURE [dbo].[IDIOMA_ACTUALIZAR_ESTADO]
GO
CREATE PROCEDURE [dbo].[IDIOMA_ACTUALIZAR_ESTADO]
    @id         INT,
    @habilitado BIT
AS
    UPDATE IDIOMA SET HABILITADO = @habilitado WHERE ID = @id
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type='P' AND name='IDIOMA_ELIMINAR')
    DROP PROCEDURE [dbo].[IDIOMA_ELIMINAR]
GO
CREATE PROCEDURE [dbo].[IDIOMA_ELIMINAR]
    @id INT
AS
BEGIN
    DELETE FROM TRADUCCION WHERE IDIOMA_ID  = @id
    DELETE FROM IDIOMA      WHERE ID        = @id
END
GO

-- Stored procedures CONTROL_IDIOMA
IF EXISTS (SELECT * FROM sys.objects WHERE type='P' AND name='CONTROL_LISTAR_TODOS')
    DROP PROCEDURE [dbo].[CONTROL_LISTAR_TODOS]
GO
CREATE PROCEDURE [dbo].[CONTROL_LISTAR_TODOS]
AS
    SELECT ID, CLAVE, TEXTO_DEFAULT FROM CONTROL_IDIOMA ORDER BY CLAVE
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type='P' AND name='CONTROL_REGISTRAR')
    DROP PROCEDURE [dbo].[CONTROL_REGISTRAR]
GO
CREATE PROCEDURE [dbo].[CONTROL_REGISTRAR]
    @clave         VARCHAR(100),
    @texto_default VARCHAR(200)
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM CONTROL_IDIOMA WHERE CLAVE = @clave)
        INSERT INTO CONTROL_IDIOMA (CLAVE, TEXTO_DEFAULT) VALUES (@clave, @texto_default)
END
GO

-- Stored procedures TRADUCCION
IF EXISTS (SELECT * FROM sys.objects WHERE type='P' AND name='TRADUCCION_LISTAR_POR_IDIOMA')
    DROP PROCEDURE [dbo].[TRADUCCION_LISTAR_POR_IDIOMA]
GO
CREATE PROCEDURE [dbo].[TRADUCCION_LISTAR_POR_IDIOMA]
    @idioma_id INT
AS
    SELECT ci.CLAVE, t.TEXTO
    FROM CONTROL_IDIOMA ci
    LEFT JOIN TRADUCCION t ON t.CONTROL_ID = ci.ID AND t.IDIOMA_ID = @idioma_id
    WHERE t.TEXTO IS NOT NULL
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type='P' AND name='TRADUCCION_GUARDAR')
    DROP PROCEDURE [dbo].[TRADUCCION_GUARDAR]
GO
CREATE PROCEDURE [dbo].[TRADUCCION_GUARDAR]
    @idioma_id  INT,
    @control_id INT,
    @texto      VARCHAR(200)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM TRADUCCION WHERE IDIOMA_ID = @idioma_id AND CONTROL_ID = @control_id)
        UPDATE TRADUCCION SET TEXTO = @texto
        WHERE IDIOMA_ID = @idioma_id AND CONTROL_ID = @control_id
    ELSE
        INSERT INTO TRADUCCION (IDIOMA_ID, CONTROL_ID, TEXTO)
        VALUES (@idioma_id, @control_id, @texto)
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type='P' AND name='TRADUCCION_LISTAR_CONTROLES')
    DROP PROCEDURE [dbo].[TRADUCCION_LISTAR_CONTROLES]
GO
CREATE PROCEDURE [dbo].[TRADUCCION_LISTAR_CONTROLES]
    @idioma_id INT
AS
    SELECT ci.ID, ci.CLAVE, ci.TEXTO_DEFAULT,
           ISNULL(t.TEXTO, '') AS TEXTO_TRADUCCION
    FROM CONTROL_IDIOMA ci
    LEFT JOIN TRADUCCION t ON t.CONTROL_ID = ci.ID AND t.IDIOMA_ID = @idioma_id
    ORDER BY ci.CLAVE
GO

-- ============================================================
-- Registro de controles de todos los formularios (idempotente)
-- ============================================================

-- LogIn
EXEC CONTROL_REGISTRAR 'lblUsuario',    '[Usuario]'
EXEC CONTROL_REGISTRAR 'lblContrasena', '[Contraseña]'
EXEC CONTROL_REGISTRAR 'btnIngresar',   '[Ingresar]'
GO

-- frmMenu (ítems del menú)
EXEC CONTROL_REGISTRAR 'usuarioToolStripMenuItem',            '[Usuario]'
EXEC CONTROL_REGISTRAR 'cerrarSesionToolStripMenuItem',       '[Cerrar Sesion]'
EXEC CONTROL_REGISTRAR 'configuraciónToolStripMenuItem',      '[Configuración]'
EXEC CONTROL_REGISTRAR 'cambiarContraseñaToolStripMenuItem',  '[Cambiar Contraseña]'
EXEC CONTROL_REGISTRAR 'bitacoraToolStripMenuItem',           '[Bitácora]'
EXEC CONTROL_REGISTRAR 'administracionToolStripMenuItem',     '[Administración]'
EXEC CONTROL_REGISTRAR 'usuariosBloqueadosToolStripMenuItem', '[Gestión de usuarios]'
EXEC CONTROL_REGISTRAR 'perfilesToolStripMenuItem',           '[Perfiles y Permisos]'
EXEC CONTROL_REGISTRAR 'idiomasToolStripMenuItem',            '[Gestión de idiomas]'
GO

-- frmAdminUsuarios
EXEC CONTROL_REGISTRAR 'btnNuevo',            '[Nuevo usuario]'
EXEC CONTROL_REGISTRAR 'btnModificarPerfiles', '[Modificar perfiles]'
EXEC CONTROL_REGISTRAR 'btnEliminar',         '[Eliminar]'
EXEC CONTROL_REGISTRAR 'btnDesbloquear',      '[Desbloquear seleccionado]'
EXEC CONTROL_REGISTRAR 'btnRefrescar',        '[Refrescar]'
EXEC CONTROL_REGISTRAR 'btnCerrar',           '[Cerrar]'
GO

-- frmNuevoUsuario
EXEC CONTROL_REGISTRAR 'lblNombre',   '[Usuario:]'
EXEC CONTROL_REGISTRAR 'lblRol',      '[Rol:]'
EXEC CONTROL_REGISTRAR 'btnAceptar',  '[Crear]'
EXEC CONTROL_REGISTRAR 'btnCancelar', '[Cancelar]'
GO

-- frmAsignarPerfiles
EXEC CONTROL_REGISTRAR 'btnGuardar', '[Guardar]'
GO

-- frmPerfiles
EXEC CONTROL_REGISTRAR 'lblSeleccionado',   '[Seleccioná un nodo]'
EXEC CONTROL_REGISTRAR 'btnAgregarPerfil',  '[Agregar Perfil]'
EXEC CONTROL_REGISTRAR 'btnAgregarPermiso', '[Agregar Permiso]'
GO

-- frmBitacora
EXEC CONTROL_REGISTRAR 'lblAccion', '[Acción:]'
EXEC CONTROL_REGISTRAR 'lblDesde',  '[Desde:]'
EXEC CONTROL_REGISTRAR 'lblHasta',  '[Hasta:]'
EXEC CONTROL_REGISTRAR 'btnFiltrar','[Filtrar]'
EXEC CONTROL_REGISTRAR 'btnLimpiar','[Limpiar]'
GO

-- frmContraseña
EXEC CONTROL_REGISTRAR 'label1',       '[Contraseña Actual:]'
EXEC CONTROL_REGISTRAR 'label2',       '[Nueva Contraseña:]'
EXEC CONTROL_REGISTRAR 'label3',       '[Confirmar Contraseña:]'
EXEC CONTROL_REGISTRAR 'label5',       '[Cambiar Contraseña]'
EXEC CONTROL_REGISTRAR 'btnContinuar', '[Continuar]'
EXEC CONTROL_REGISTRAR 'button1',      '[Cancelar]'
GO

-- frmIdiomas
EXEC CONTROL_REGISTRAR 'lblIdiomas',             '[Idiomas]'
EXEC CONTROL_REGISTRAR 'btnAgregarIdioma',       '[Agregar idioma]'
EXEC CONTROL_REGISTRAR 'btnRenombrar',           '[Renombrar idioma]'
EXEC CONTROL_REGISTRAR 'btnToggleHabilitado',    '[Habilitar/Deshabilitar]'
EXEC CONTROL_REGISTRAR 'lblTraducciones',        '[Traducciones del idioma seleccionado]'
EXEC CONTROL_REGISTRAR 'btnGuardarTraducciones', '[Guardar traducciones]'
GO

-- ============================================================
-- Traducciones al inglés
-- ============================================================
DECLARE @inglesId INT = (SELECT ID FROM IDIOMA WHERE NOMBRE = 'Inglés')
DECLARE @cid INT

-- LogIn
SET @cid = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'lblUsuario')
EXEC TRADUCCION_GUARDAR @inglesId, @cid, 'Username'

SET @cid = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'lblContrasena')
EXEC TRADUCCION_GUARDAR @inglesId, @cid, 'Password'

SET @cid = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'btnIngresar')
EXEC TRADUCCION_GUARDAR @inglesId, @cid, 'Login'

-- frmMenu
SET @cid = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'usuarioToolStripMenuItem')
EXEC TRADUCCION_GUARDAR @inglesId, @cid, 'User'

SET @cid = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'cerrarSesionToolStripMenuItem')
EXEC TRADUCCION_GUARDAR @inglesId, @cid, 'Log Out'

SET @cid = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'configuraciónToolStripMenuItem')
EXEC TRADUCCION_GUARDAR @inglesId, @cid, 'Settings'

SET @cid = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'cambiarContraseñaToolStripMenuItem')
EXEC TRADUCCION_GUARDAR @inglesId, @cid, 'Change Password'

SET @cid = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'bitacoraToolStripMenuItem')
EXEC TRADUCCION_GUARDAR @inglesId, @cid, 'Audit Log'

SET @cid = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'administracionToolStripMenuItem')
EXEC TRADUCCION_GUARDAR @inglesId, @cid, 'Administration'

SET @cid = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'usuariosBloqueadosToolStripMenuItem')
EXEC TRADUCCION_GUARDAR @inglesId, @cid, 'User Management'

SET @cid = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'perfilesToolStripMenuItem')
EXEC TRADUCCION_GUARDAR @inglesId, @cid, 'Profiles & Permissions'

SET @cid = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'idiomasToolStripMenuItem')
EXEC TRADUCCION_GUARDAR @inglesId, @cid, 'Language Management'

-- Compartidos
SET @cid = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'btnCerrar')
EXEC TRADUCCION_GUARDAR @inglesId, @cid, 'Close'

SET @cid = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'btnEliminar')
EXEC TRADUCCION_GUARDAR @inglesId, @cid, 'Delete'

SET @cid = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'btnGuardar')
EXEC TRADUCCION_GUARDAR @inglesId, @cid, 'Save'

SET @cid = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'btnRefrescar')
EXEC TRADUCCION_GUARDAR @inglesId, @cid, 'Refresh'

-- frmAdminUsuarios
SET @cid = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'btnNuevo')
EXEC TRADUCCION_GUARDAR @inglesId, @cid, 'New User'

SET @cid = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'btnModificarPerfiles')
EXEC TRADUCCION_GUARDAR @inglesId, @cid, 'Modify Profiles'

SET @cid = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'btnDesbloquear')
EXEC TRADUCCION_GUARDAR @inglesId, @cid, 'Unlock Selected'

-- frmNuevoUsuario
SET @cid = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'lblNombre')
EXEC TRADUCCION_GUARDAR @inglesId, @cid, 'Username:'

SET @cid = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'lblRol')
EXEC TRADUCCION_GUARDAR @inglesId, @cid, 'Role:'

SET @cid = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'btnAceptar')
EXEC TRADUCCION_GUARDAR @inglesId, @cid, 'Create'

SET @cid = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'btnCancelar')
EXEC TRADUCCION_GUARDAR @inglesId, @cid, 'Cancel'

-- frmPerfiles
SET @cid = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'lblSeleccionado')
EXEC TRADUCCION_GUARDAR @inglesId, @cid, 'Select a node'

SET @cid = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'btnAgregarPerfil')
EXEC TRADUCCION_GUARDAR @inglesId, @cid, 'Add Profile'

SET @cid = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'btnAgregarPermiso')
EXEC TRADUCCION_GUARDAR @inglesId, @cid, 'Add Permission'

-- frmBitacora
SET @cid = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'lblAccion')
EXEC TRADUCCION_GUARDAR @inglesId, @cid, 'Action:'

SET @cid = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'lblDesde')
EXEC TRADUCCION_GUARDAR @inglesId, @cid, 'From:'

SET @cid = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'lblHasta')
EXEC TRADUCCION_GUARDAR @inglesId, @cid, 'To:'

SET @cid = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'btnFiltrar')
EXEC TRADUCCION_GUARDAR @inglesId, @cid, 'Filter'

SET @cid = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'btnLimpiar')
EXEC TRADUCCION_GUARDAR @inglesId, @cid, 'Clear'

-- frmContraseña
SET @cid = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'label1')
EXEC TRADUCCION_GUARDAR @inglesId, @cid, 'Current Password:'

SET @cid = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'label2')
EXEC TRADUCCION_GUARDAR @inglesId, @cid, 'New Password:'

SET @cid = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'label3')
EXEC TRADUCCION_GUARDAR @inglesId, @cid, 'Confirm Password:'

SET @cid = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'label5')
EXEC TRADUCCION_GUARDAR @inglesId, @cid, 'Change Password'

SET @cid = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'btnContinuar')
EXEC TRADUCCION_GUARDAR @inglesId, @cid, 'Continue'

SET @cid = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'button1')
EXEC TRADUCCION_GUARDAR @inglesId, @cid, 'Cancel'

-- frmIdiomas
SET @cid = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'lblIdiomas')
EXEC TRADUCCION_GUARDAR @inglesId, @cid, 'Languages'

SET @cid = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'btnAgregarIdioma')
EXEC TRADUCCION_GUARDAR @inglesId, @cid, 'Add Language'

SET @cid = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'btnRenombrar')
EXEC TRADUCCION_GUARDAR @inglesId, @cid, 'Rename Language'

-- ============================================================
-- CONTROL DE CAMBIOS — HISTORIAL DE USUARIO
-- ============================================================

IF OBJECT_ID('USUARIO_HISTORIAL', 'U') IS NULL
    CREATE TABLE USUARIO_HISTORIAL (
        [ID]                INT          IDENTITY(1,1) NOT NULL,
        [USUARIO_ID]        INT          NOT NULL,
        [USUARIO_LOGIN]     VARCHAR(50)  NOT NULL,
        [ROL]               VARCHAR(20)  NOT NULL,
        [BLOQUEADO]         BIT          NOT NULL,
        [INTENTOS_FALLIDOS] INT          NOT NULL,
        [PERFILES]          VARCHAR(500) NOT NULL,
        [FECHA_CAMBIO]      DATETIME     NOT NULL DEFAULT GETDATE(),
        [REALIZADO_POR]     VARCHAR(50)  NOT NULL,
        [TIPO_CAMBIO]       VARCHAR(25)  NOT NULL,
        [VERSION_ORIGEN]    INT          NULL,
        CONSTRAINT PK_USUARIO_HISTORIAL PRIMARY KEY ([ID])
        -- Sin FK hacia USUARIO: el historial sobrevive a la baja del usuario
    );
GO

-- Obtener un usuario por ID (para snapshot de historial)
IF OBJECT_ID('USUARIO_OBTENER_POR_ID', 'P') IS NOT NULL
    DROP PROCEDURE USUARIO_OBTENER_POR_ID;
GO
CREATE PROCEDURE USUARIO_OBTENER_POR_ID
    @id INT
AS
BEGIN
    SELECT ID, USUARIO, ROL, INTENTOS_FALLIDOS, BLOQUEADO
    FROM USUARIO WHERE ID = @id;
END
GO

-- Obtener el ID de un usuario por nombre de login
IF OBJECT_ID('USUARIO_OBTENER_ID_POR_NOMBRE', 'P') IS NOT NULL
    DROP PROCEDURE USUARIO_OBTENER_ID_POR_NOMBRE;
GO
CREATE PROCEDURE USUARIO_OBTENER_ID_POR_NOMBRE
    @usuario VARCHAR(50)
AS
BEGIN
    SELECT ID FROM USUARIO WHERE USUARIO = @usuario;
END
GO

-- Aplicar estado histórico a un usuario (rollback; no toca PASS)
IF OBJECT_ID('USUARIO_APLICAR_ESTADO', 'P') IS NOT NULL
    DROP PROCEDURE USUARIO_APLICAR_ESTADO;
GO
CREATE PROCEDURE USUARIO_APLICAR_ESTADO
    @id                INT,
    @rol               VARCHAR(20),
    @bloqueado         BIT,
    @intentos_fallidos INT
AS
BEGIN
    UPDATE USUARIO
    SET ROL = @rol, BLOQUEADO = @bloqueado, INTENTOS_FALLIDOS = @intentos_fallidos
    WHERE ID = @id;
END
GO

-- Insertar registro en historial (inmutable: solo INSERT)
IF OBJECT_ID('USUARIO_HISTORIAL_INSERTAR', 'P') IS NOT NULL
    DROP PROCEDURE USUARIO_HISTORIAL_INSERTAR;
GO
CREATE PROCEDURE USUARIO_HISTORIAL_INSERTAR
    @usuario_id        INT,
    @usuario_login     VARCHAR(50),
    @rol               VARCHAR(20),
    @bloqueado         BIT,
    @intentos_fallidos INT,
    @perfiles          VARCHAR(500),
    @realizado_por     VARCHAR(50),
    @tipo_cambio       VARCHAR(25),
    @version_origen    INT
AS
BEGIN
    INSERT INTO USUARIO_HISTORIAL
        (USUARIO_ID, USUARIO_LOGIN, ROL, BLOQUEADO, INTENTOS_FALLIDOS,
         PERFILES, REALIZADO_POR, TIPO_CAMBIO, VERSION_ORIGEN)
    VALUES
        (@usuario_id, @usuario_login, @rol, @bloqueado, @intentos_fallidos,
         @perfiles, @realizado_por, @tipo_cambio, @version_origen);
END
GO

-- Listar historial de un usuario ordenado por más reciente primero
IF OBJECT_ID('USUARIO_HISTORIAL_LISTAR', 'P') IS NOT NULL
    DROP PROCEDURE USUARIO_HISTORIAL_LISTAR;
GO
CREATE PROCEDURE USUARIO_HISTORIAL_LISTAR
    @usuario_id INT
AS
BEGIN
    SELECT ID, USUARIO_ID, USUARIO_LOGIN, ROL, BLOQUEADO, INTENTOS_FALLIDOS,
           PERFILES, FECHA_CAMBIO, REALIZADO_POR, TIPO_CAMBIO, VERSION_ORIGEN
    FROM USUARIO_HISTORIAL
    WHERE USUARIO_ID = @usuario_id
    ORDER BY ID DESC;
END
GO

-- Obtener un registro de historial por su ID (para rollback)
IF OBJECT_ID('USUARIO_HISTORIAL_OBTENER', 'P') IS NOT NULL
    DROP PROCEDURE USUARIO_HISTORIAL_OBTENER;
GO
CREATE PROCEDURE USUARIO_HISTORIAL_OBTENER
    @id INT
AS
BEGIN
    SELECT ID, USUARIO_ID, USUARIO_LOGIN, ROL, BLOQUEADO, INTENTOS_FALLIDOS,
           PERFILES, FECHA_CAMBIO, REALIZADO_POR, TIPO_CAMBIO, VERSION_ORIGEN
    FROM USUARIO_HISTORIAL WHERE ID = @id;
END
GO

-- ============================================================
-- DÍGITOS VERIFICADORES DE INTEGRIDAD
-- ============================================================

-- Agregar columna DVH a USUARIO (idempotente)
IF COL_LENGTH('USUARIO', 'DVH') IS NULL
    ALTER TABLE USUARIO ADD DVH INT NOT NULL DEFAULT 0;
GO

-- Tabla para dígitos verificadores verticales
IF OBJECT_ID('DIGITO_VERIFICADOR_VERTICAL', 'U') IS NULL
    CREATE TABLE DIGITO_VERIFICADOR_VERTICAL (
        [TABLA]   VARCHAR(50) NOT NULL,
        [COLUMNA] VARCHAR(50) NOT NULL,
        [DVV]     INT         NOT NULL DEFAULT 0,
        CONSTRAINT PK_DVV PRIMARY KEY ([TABLA], [COLUMNA])
    );
GO

-- SP: Listar todos los usuarios con sus datos completos para cálculo de integridad
IF OBJECT_ID('USUARIO_LISTAR_PARA_INTEGRIDAD', 'P') IS NOT NULL
    DROP PROCEDURE USUARIO_LISTAR_PARA_INTEGRIDAD;
GO
CREATE PROCEDURE USUARIO_LISTAR_PARA_INTEGRIDAD
AS
BEGIN
    SELECT ID, USUARIO, PASS, INTENTOS_FALLIDOS, BLOQUEADO, ROL, DVH
    FROM USUARIO
    ORDER BY ID;
END
GO

-- SP: Actualizar DVH de un usuario específico
IF OBJECT_ID('USUARIO_ACTUALIZAR_DVH', 'P') IS NOT NULL
    DROP PROCEDURE USUARIO_ACTUALIZAR_DVH;
GO
CREATE PROCEDURE USUARIO_ACTUALIZAR_DVH
    @id  INT,
    @dvh INT
AS
BEGIN
    UPDATE USUARIO SET DVH = @dvh WHERE ID = @id;
END
GO

-- SP: Listar dígitos verificadores verticales de una tabla
IF OBJECT_ID('DVV_LISTAR', 'P') IS NOT NULL
    DROP PROCEDURE DVV_LISTAR;
GO
CREATE PROCEDURE DVV_LISTAR
    @tabla VARCHAR(50)
AS
BEGIN
    SELECT COLUMNA, DVV
    FROM DIGITO_VERIFICADOR_VERTICAL
    WHERE TABLA = @tabla;
END
GO

-- SP: Upsert de dígito verificador vertical
IF OBJECT_ID('DVV_ACTUALIZAR', 'P') IS NOT NULL
    DROP PROCEDURE DVV_ACTUALIZAR;
GO
CREATE PROCEDURE DVV_ACTUALIZAR
    @tabla   VARCHAR(50),
    @columna VARCHAR(50),
    @dvv     INT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM DIGITO_VERIFICADOR_VERTICAL WHERE TABLA = @tabla AND COLUMNA = @columna)
        UPDATE DIGITO_VERIFICADOR_VERTICAL SET DVV = @dvv WHERE TABLA = @tabla AND COLUMNA = @columna;
    ELSE
        INSERT INTO DIGITO_VERIFICADOR_VERTICAL (TABLA, COLUMNA, DVV) VALUES (@tabla, @columna, @dvv);
END
GO

-- ---- BITACORA ----

IF COL_LENGTH('BITACORA', 'DVH') IS NULL
    ALTER TABLE BITACORA ADD DVH INT NOT NULL DEFAULT 0;
GO

IF OBJECT_ID('BITACORA_LISTAR_PARA_INTEGRIDAD', 'P') IS NOT NULL
    DROP PROCEDURE BITACORA_LISTAR_PARA_INTEGRIDAD;
GO
CREATE PROCEDURE BITACORA_LISTAR_PARA_INTEGRIDAD
AS
BEGIN
    SELECT ID, USUARIO, ACCION,
           CONVERT(VARCHAR(19), FECHA, 120) AS FECHA,
           DVH
    FROM BITACORA
    ORDER BY ID;
END
GO

IF OBJECT_ID('BITACORA_ACTUALIZAR_DVH', 'P') IS NOT NULL
    DROP PROCEDURE BITACORA_ACTUALIZAR_DVH;
GO
CREATE PROCEDURE BITACORA_ACTUALIZAR_DVH
    @id  INT,
    @dvh INT
AS
BEGIN
    UPDATE BITACORA SET DVH = @dvh WHERE ID = @id;
END
GO

-- ---- NODO_PERMISO ----

IF COL_LENGTH('NODO_PERMISO', 'DVH') IS NULL
    ALTER TABLE NODO_PERMISO ADD DVH INT NOT NULL DEFAULT 0;
GO

IF OBJECT_ID('NODO_PERMISO_LISTAR_PARA_INTEGRIDAD', 'P') IS NOT NULL
    DROP PROCEDURE NODO_PERMISO_LISTAR_PARA_INTEGRIDAD;
GO
CREATE PROCEDURE NODO_PERMISO_LISTAR_PARA_INTEGRIDAD
AS
BEGIN
    SELECT ID, NOMBRE, TIPO,
           ISNULL(CAST(PADRE_ID AS VARCHAR(10)), '0') AS PADRE_ID,
           DVH
    FROM NODO_PERMISO
    ORDER BY ID;
END
GO

IF OBJECT_ID('NODO_PERMISO_ACTUALIZAR_DVH', 'P') IS NOT NULL
    DROP PROCEDURE NODO_PERMISO_ACTUALIZAR_DVH;
GO
CREATE PROCEDURE NODO_PERMISO_ACTUALIZAR_DVH
    @id  INT,
    @dvh INT
AS
BEGIN
    UPDATE NODO_PERMISO SET DVH = @dvh WHERE ID = @id;
END
GO

SET @cid = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'btnToggleHabilitado')
EXEC TRADUCCION_GUARDAR @inglesId, @cid, 'Enable/Disable'

SET @cid = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'lblTraducciones')
EXEC TRADUCCION_GUARDAR @inglesId, @cid, 'Translations for selected language'

SET @cid = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'btnGuardarTraducciones')
EXEC TRADUCCION_GUARDAR @inglesId, @cid, 'Save Translations'
GO

-- ============================================================
-- Títulos de formularios (barra de título) y cabeceras de grillas
-- ============================================================

-- Títulos de formularios
EXEC CONTROL_REGISTRAR 'LogIn',            'Iniciar Sesión'
EXEC CONTROL_REGISTRAR 'frmAdminUsuarios', 'Administración de usuarios'
EXEC CONTROL_REGISTRAR 'frmBitacora',      'Bitácora'
EXEC CONTROL_REGISTRAR 'frmPerfiles',      'Gestión de Perfiles y Permisos'
EXEC CONTROL_REGISTRAR 'frmContraseña',    'Cambiar Contraseña'
EXEC CONTROL_REGISTRAR 'frmNuevoUsuario',  'Nuevo usuario'
EXEC CONTROL_REGISTRAR 'frmAsignarPerfiles', 'Modificar perfiles de usuario'
EXEC CONTROL_REGISTRAR 'frmIdiomas',       'Gestión de idiomas'
GO

-- Cabeceras de columnas de DataGridView
EXEC CONTROL_REGISTRAR 'colhdr_Id',              'ID'
EXEC CONTROL_REGISTRAR 'colhdr_Usuario',         'Usuario'
EXEC CONTROL_REGISTRAR 'colhdr_Rol',             'Rol'
EXEC CONTROL_REGISTRAR 'colhdr_Bloqueado',       'Bloqueado'
EXEC CONTROL_REGISTRAR 'colhdr_Accion',          'Acción'
EXEC CONTROL_REGISTRAR 'colhdr_Fecha',           'Fecha y Hora'
EXEC CONTROL_REGISTRAR 'colhdr_Nombre',          'Idioma'
EXEC CONTROL_REGISTRAR 'colhdr_Habilitado',      'Habilitado'
EXEC CONTROL_REGISTRAR 'colhdr_Clave',           'Clave'
EXEC CONTROL_REGISTRAR 'colhdr_TextoDefault',    'Texto por defecto'
EXEC CONTROL_REGISTRAR 'colhdr_TextoTraduccion', 'Traducción'
GO

-- Botón eliminar idioma (frmIdiomas)
EXEC CONTROL_REGISTRAR 'btnEliminarIdioma', 'Eliminar idioma'
GO

-- Labels con key único por formulario (lblTitulo existe en dos forms distintos)
EXEC CONTROL_REGISTRAR 'lblTitulo_Bitacora',      'Bitácora del Sistema'
EXEC CONTROL_REGISTRAR 'lblTitulo_AdminUsuarios', 'Gestión de usuarios'
GO

-- Condiciones de contraseña (frmContraseña, label4)
EXEC CONTROL_REGISTRAR 'label4', '- 6 o mas caracteres / 1 MAYÚSCULA / 1 número'
GO

-- Prefijos de nodos del TreeView de perfiles
EXEC CONTROL_REGISTRAR 'prefijo_Perfil',  '[Perfil] '
EXEC CONTROL_REGISTRAR 'prefijo_Permiso', '[Permiso] '
GO

-- ============================================================
-- Traducciones al inglés — títulos y cabeceras
-- ============================================================
DECLARE @inglesId2 INT = (SELECT ID FROM IDIOMA WHERE NOMBRE = 'Inglés')
DECLARE @cid2 INT
DECLARE @txtCondiciones NVARCHAR(500)

-- Títulos de formularios
SET @cid2 = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'LogIn')
EXEC TRADUCCION_GUARDAR @inglesId2, @cid2, 'Log In'

SET @cid2 = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'frmAdminUsuarios')
EXEC TRADUCCION_GUARDAR @inglesId2, @cid2, 'User Administration'

SET @cid2 = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'frmBitacora')
EXEC TRADUCCION_GUARDAR @inglesId2, @cid2, 'Audit Log'

SET @cid2 = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'frmPerfiles')
EXEC TRADUCCION_GUARDAR @inglesId2, @cid2, 'Profiles & Permissions'

SET @cid2 = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'frmContraseña')
EXEC TRADUCCION_GUARDAR @inglesId2, @cid2, 'Change Password'

SET @cid2 = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'frmNuevoUsuario')
EXEC TRADUCCION_GUARDAR @inglesId2, @cid2, 'New User'

SET @cid2 = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'frmAsignarPerfiles')
EXEC TRADUCCION_GUARDAR @inglesId2, @cid2, 'Edit User Profiles'

SET @cid2 = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'frmIdiomas')
EXEC TRADUCCION_GUARDAR @inglesId2, @cid2, 'Language Management'

-- Cabeceras de columnas
SET @cid2 = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'colhdr_Id')
EXEC TRADUCCION_GUARDAR @inglesId2, @cid2, 'ID'

SET @cid2 = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'colhdr_Usuario')
EXEC TRADUCCION_GUARDAR @inglesId2, @cid2, 'Username'

SET @cid2 = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'colhdr_Rol')
EXEC TRADUCCION_GUARDAR @inglesId2, @cid2, 'Role'

SET @cid2 = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'colhdr_Bloqueado')
EXEC TRADUCCION_GUARDAR @inglesId2, @cid2, 'Blocked'

SET @cid2 = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'colhdr_Accion')
EXEC TRADUCCION_GUARDAR @inglesId2, @cid2, 'Action'

SET @cid2 = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'colhdr_Fecha')
EXEC TRADUCCION_GUARDAR @inglesId2, @cid2, 'Date & Time'

SET @cid2 = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'colhdr_Nombre')
EXEC TRADUCCION_GUARDAR @inglesId2, @cid2, 'Language'

SET @cid2 = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'colhdr_Habilitado')
EXEC TRADUCCION_GUARDAR @inglesId2, @cid2, 'Enabled'

SET @cid2 = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'colhdr_Clave')
EXEC TRADUCCION_GUARDAR @inglesId2, @cid2, 'Key'

SET @cid2 = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'colhdr_TextoDefault')
EXEC TRADUCCION_GUARDAR @inglesId2, @cid2, 'Default Text'

SET @cid2 = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'colhdr_TextoTraduccion')
EXEC TRADUCCION_GUARDAR @inglesId2, @cid2, 'Translation'

-- Botón eliminar idioma
SET @cid2 = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'btnEliminarIdioma')
EXEC TRADUCCION_GUARDAR @inglesId2, @cid2, 'Delete Language'

-- Labels con key único por formulario
SET @cid2 = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'lblTitulo_Bitacora')
EXEC TRADUCCION_GUARDAR @inglesId2, @cid2, 'Audit Log'

SET @cid2 = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'lblTitulo_AdminUsuarios')
EXEC TRADUCCION_GUARDAR @inglesId2, @cid2, 'User Management'

-- Condiciones de contraseña
SET @txtCondiciones = ' - 6 or more characters' + CHAR(13)+CHAR(10) + '- 1 or more UPPERCASE letters' + CHAR(13)+CHAR(10) + '- 1 or more numbers'
SET @cid2 = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'label4')
EXEC TRADUCCION_GUARDAR @inglesId2, @cid2, @txtCondiciones

-- Prefijos TreeView perfiles
SET @cid2 = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'prefijo_Perfil')
EXEC TRADUCCION_GUARDAR @inglesId2, @cid2, '[Profile] '

SET @cid2 = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'prefijo_Permiso')
EXEC TRADUCCION_GUARDAR @inglesId2, @cid2, '[Permission] '

-- ============================================================
-- Control de Cambios — frmHistorialUsuario
-- ============================================================

-- Título del formulario y botones
EXEC CONTROL_REGISTRAR 'frmHistorialUsuario',         'Historial de usuario'
EXEC CONTROL_REGISTRAR 'lblTitulo_HistorialUsuarios', 'Historial de cambios'
EXEC CONTROL_REGISTRAR 'btnRollback',                 'Restaurar versión'
EXEC CONTROL_REGISTRAR 'btnHistorial',                'Ver historial'

-- Cabeceras de columnas específicas del historial
EXEC CONTROL_REGISTRAR 'colhdr_FechaCambio',      'Fecha'
EXEC CONTROL_REGISTRAR 'colhdr_TipoCambio',       'Tipo'
EXEC CONTROL_REGISTRAR 'colhdr_IntentosFallidos', 'Intentos'
EXEC CONTROL_REGISTRAR 'colhdr_Perfiles',         'Perfiles'
EXEC CONTROL_REGISTRAR 'colhdr_RealizadoPor',     'Realizado por'
EXEC CONTROL_REGISTRAR 'colhdr_VersionOrigen',    'Versión origen'
GO

-- Traducciones al inglés
DECLARE @inglesId3 INT = (SELECT ID FROM IDIOMA WHERE NOMBRE = 'Inglés')
DECLARE @cid3 INT

SET @cid3 = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'frmHistorialUsuario')
EXEC TRADUCCION_GUARDAR @inglesId3, @cid3, 'User History'

SET @cid3 = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'lblTitulo_HistorialUsuarios')
EXEC TRADUCCION_GUARDAR @inglesId3, @cid3, 'Change History'

SET @cid3 = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'btnRollback')
EXEC TRADUCCION_GUARDAR @inglesId3, @cid3, 'Restore Version'

SET @cid3 = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'btnHistorial')
EXEC TRADUCCION_GUARDAR @inglesId3, @cid3, 'View History'

SET @cid3 = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'colhdr_FechaCambio')
EXEC TRADUCCION_GUARDAR @inglesId3, @cid3, 'Date'

SET @cid3 = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'colhdr_TipoCambio')
EXEC TRADUCCION_GUARDAR @inglesId3, @cid3, 'Type'

SET @cid3 = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'colhdr_IntentosFallidos')
EXEC TRADUCCION_GUARDAR @inglesId3, @cid3, 'Failed Attempts'

SET @cid3 = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'colhdr_Perfiles')
EXEC TRADUCCION_GUARDAR @inglesId3, @cid3, 'Profiles'

SET @cid3 = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'colhdr_RealizadoPor')
EXEC TRADUCCION_GUARDAR @inglesId3, @cid3, 'Done By'

SET @cid3 = (SELECT ID FROM CONTROL_IDIOMA WHERE CLAVE = 'colhdr_VersionOrigen')
EXEC TRADUCCION_GUARDAR @inglesId3, @cid3, 'Source Version'
GO
GO