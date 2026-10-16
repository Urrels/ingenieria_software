USE [BDCAPAS]
GO
/****** >>> LIQUIDACION_HORAS (inicio) — CU-38 y CU-39: control de horas trabajadas, liquidación y corrección de fichajes. Idempotente. ******/

-- Fichaje: corrección administrativa auditada (quién, cuándo y por qué).
IF COL_LENGTH('dbo.ASISTENCIA_PERSONAL', 'CORREGIDO') IS NULL
    ALTER TABLE dbo.ASISTENCIA_PERSONAL ADD
        CORREGIDO         BIT          NOT NULL CONSTRAINT DF_ASISTPERS_CORREGIDO DEFAULT (0),
        MOTIVO_CORRECCION VARCHAR(200) NULL,
        CORREGIDO_POR     VARCHAR(50)  NULL,
        FECHA_CORRECCION  DATETIME     NULL
GO

-- Parámetros de la liquidación (tolerancia, redondeo, jornada, recargos).
IF OBJECT_ID('dbo.PARAMETRO_LIQUIDACION', 'U') IS NULL
    CREATE TABLE dbo.PARAMETRO_LIQUIDACION (
        CLAVE       VARCHAR(50)   NOT NULL CONSTRAINT PK_PARAMETRO_LIQUIDACION PRIMARY KEY,
        VALOR       DECIMAL(10,2) NOT NULL,
        DESCRIPCION VARCHAR(200)  NULL
    )
GO
INSERT INTO dbo.PARAMETRO_LIQUIDACION (CLAVE, VALOR, DESCRIPCION)
SELECT v.CLAVE, v.VALOR, v.DESCRIPCION
FROM (VALUES
    ('TOLERANCIA_MINUTOS',  10,   'Diferencia (en minutos) entre lo fichado y lo asignado que no se descuenta ni se suma'),
    ('REDONDEO_MINUTOS',    15,   'Las horas de cada día se redondean al múltiplo más cercano de estos minutos (0 = sin redondeo)'),
    ('JORNADA_NORMAL_HORAS', 8,   'Horas por día a valor normal; lo que excede se paga con recargo de hora extra'),
    ('RECARGO_EXTRA_PCT',   50,   'Recargo (%) de las horas extra de días comunes'),
    ('RECARGO_FERIADO_PCT', 100,  'Recargo (%) de las horas trabajadas en domingo o feriado'),
    ('MAX_HORAS_FICHAJE_ABIERTO', 16, 'Un ingreso sin egreso con más horas que estas se considera incompleto y debe corregirlo el administrador')
) v(CLAVE, VALOR, DESCRIPCION)
WHERE NOT EXISTS (SELECT 1 FROM dbo.PARAMETRO_LIQUIDACION p WHERE p.CLAVE = v.CLAVE)
GO

-- Valor hora por empleado, con fecha de vigencia (las liquidaciones pasadas no cambian si el valor sube).
IF OBJECT_ID('dbo.VALOR_HORA_EMPLEADO', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.VALOR_HORA_EMPLEADO (
        ID             INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_VALOR_HORA_EMPLEADO PRIMARY KEY,
        USUARIO_ID     INT           NOT NULL,
        VALOR          DECIMAL(10,2) NOT NULL,
        VIGENTE_DESDE  DATE          NOT NULL,
        CREADO_POR     VARCHAR(50)   NULL,
        CONSTRAINT FK_VALORHORA_USUARIO FOREIGN KEY (USUARIO_ID) REFERENCES dbo.USUARIO (ID),
        CONSTRAINT UQ_VALORHORA_USUARIO_DESDE UNIQUE (USUARIO_ID, VIGENTE_DESDE)
    )
END
GO

IF OBJECT_ID('dbo.FERIADO', 'U') IS NULL
    CREATE TABLE dbo.FERIADO (
        FECHA       DATE         NOT NULL CONSTRAINT PK_FERIADO PRIMARY KEY,
        DESCRIPCION VARCHAR(100) NULL
    )
GO

-- Horas fichadas por encima de lo asignado: se pagan solo si el administrador las aprueba (por empleado y día).
IF OBJECT_ID('dbo.EXCEDENTE_APROBADO', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.EXCEDENTE_APROBADO (
        USUARIO_ID       INT         NOT NULL,
        FECHA            DATE        NOT NULL,
        APROBADO_POR     VARCHAR(50) NULL,
        FECHA_APROBACION DATETIME    NOT NULL CONSTRAINT DF_EXCEDENTE_FECHA DEFAULT (GETDATE()),
        CONSTRAINT PK_EXCEDENTE_APROBADO PRIMARY KEY (USUARIO_ID, FECHA),
        CONSTRAINT FK_EXCEDENTE_USUARIO FOREIGN KEY (USUARIO_ID) REFERENCES dbo.USUARIO (ID)
    )
END
GO

-- Período liquidado: al cerrarlo se guarda una foto de los números (no se recalcula).
IF OBJECT_ID('dbo.LIQUIDACION', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.LIQUIDACION (
        ID           INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_LIQUIDACION PRIMARY KEY,
        DESDE        DATE        NOT NULL,
        HASTA        DATE        NOT NULL,
        ESTADO       VARCHAR(20) NOT NULL,
        CERRADA_POR  VARCHAR(50) NULL,
        FECHA_CIERRE DATETIME    NOT NULL CONSTRAINT DF_LIQUIDACION_CIERRE DEFAULT (GETDATE()),
        PAGADA_POR   VARCHAR(50) NULL,
        FECHA_PAGO   DATETIME    NULL
    )
END
GO

IF OBJECT_ID('dbo.LIQUIDACION_DIA', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.LIQUIDACION_DIA (
        ID                 INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_LIQUIDACION_DIA PRIMARY KEY,
        LIQUIDACION_ID     INT           NOT NULL,
        USUARIO_ID         INT           NOT NULL,
        EMPLEADO           VARCHAR(120)  NULL,
        FECHA              DATE          NOT NULL,
        INGRESO            TIME(0)       NULL,
        EGRESO             TIME(0)       NULL,
        MIN_TRABAJADOS     INT           NOT NULL,
        MIN_ASIGNADOS      INT           NOT NULL,
        MIN_PAGABLES       INT           NOT NULL,
        MIN_NORMALES       INT           NOT NULL,
        MIN_EXTRA          INT           NOT NULL,
        MIN_FERIADO        INT           NOT NULL,
        MIN_EXCEDENTE      INT           NOT NULL,
        EXCEDENTE_APROBADO BIT           NOT NULL,
        AUSENTE            BIT           NOT NULL,
        VALOR_HORA         DECIMAL(10,2) NOT NULL,
        RECARGO_EXTRA_PCT  DECIMAL(10,2) NOT NULL,
        RECARGO_FERIADO_PCT DECIMAL(10,2) NOT NULL,
        IMPORTE            DECIMAL(12,2) NOT NULL,
        OBSERVACION        VARCHAR(200)  NULL,
        CONSTRAINT FK_LIQDIA_LIQUIDACION FOREIGN KEY (LIQUIDACION_ID) REFERENCES dbo.LIQUIDACION (ID),
        CONSTRAINT FK_LIQDIA_USUARIO FOREIGN KEY (USUARIO_ID) REFERENCES dbo.USUARIO (ID)
    )
END
GO

/****** Procedimientos ******/
CREATE OR ALTER PROCEDURE dbo.ASISTENCIA_OBTENER_ABIERTA
    @usuario_id INT
AS
    SELECT TOP 1 ID, USUARIO_ID, FECHA, HORA_INGRESO, HORA_EGRESO
    FROM ASISTENCIA_PERSONAL
    WHERE USUARIO_ID = @usuario_id AND HORA_EGRESO IS NULL
    ORDER BY FECHA DESC, HORA_INGRESO DESC
GO

CREATE OR ALTER PROCEDURE dbo.ASISTENCIA_LISTAR_RANGO
    @desde      DATE,
    @hasta      DATE,
    @usuario_id INT = NULL
AS
    SELECT a.ID, a.USUARIO_ID, u.USUARIO, u.NOMBRE, u.APELLIDO, a.FECHA, a.HORA_INGRESO, a.HORA_EGRESO,
           a.CORREGIDO, a.MOTIVO_CORRECCION
    FROM ASISTENCIA_PERSONAL a
    JOIN USUARIO u ON u.ID = a.USUARIO_ID
    WHERE a.FECHA BETWEEN @desde AND @hasta
      AND (@usuario_id IS NULL OR a.USUARIO_ID = @usuario_id)
    ORDER BY a.FECHA, a.HORA_INGRESO
GO

CREATE OR ALTER PROCEDURE dbo.ASISTENCIA_OBTENER_POR_ID
    @id INT
AS
    SELECT a.ID, a.USUARIO_ID, u.USUARIO, u.NOMBRE, u.APELLIDO, a.FECHA, a.HORA_INGRESO, a.HORA_EGRESO,
           a.CORREGIDO, a.MOTIVO_CORRECCION
    FROM ASISTENCIA_PERSONAL a
    JOIN USUARIO u ON u.ID = a.USUARIO_ID
    WHERE a.ID = @id
GO

CREATE OR ALTER PROCEDURE dbo.ASISTENCIA_CORREGIR
    @id            INT,
    @hora_ingreso  TIME,
    @hora_egreso   TIME,
    @motivo        VARCHAR(200),
    @corregido_por VARCHAR(50)
AS
    UPDATE ASISTENCIA_PERSONAL
    SET HORA_INGRESO = @hora_ingreso, HORA_EGRESO = @hora_egreso, CORREGIDO = 1,
        MOTIVO_CORRECCION = @motivo, CORREGIDO_POR = @corregido_por, FECHA_CORRECCION = GETDATE()
    WHERE ID = @id
GO

CREATE OR ALTER PROCEDURE dbo.ASISTENCIA_INSERTAR_MANUAL
    @usuario_id    INT,
    @fecha         DATE,
    @hora_ingreso  TIME,
    @hora_egreso   TIME,
    @motivo        VARCHAR(200),
    @corregido_por VARCHAR(50)
AS
BEGIN
    INSERT INTO ASISTENCIA_PERSONAL (USUARIO_ID, FECHA, HORA_INGRESO, HORA_EGRESO, CORREGIDO, MOTIVO_CORRECCION, CORREGIDO_POR, FECHA_CORRECCION)
    VALUES (@usuario_id, @fecha, @hora_ingreso, @hora_egreso, 1, @motivo, @corregido_por, GETDATE())
    SELECT SCOPE_IDENTITY() AS ID
END
GO

CREATE OR ALTER PROCEDURE dbo.USUARIO_LISTAR_EMPLEADOS
AS
    SELECT DISTINCT u.ID, u.USUARIO, u.NOMBRE, u.APELLIDO
    FROM USUARIO u
    JOIN USUARIO_PERFIL up ON up.USUARIO_ID = u.ID
    JOIN ROL_PERMISO rp ON rp.ROL_ID = up.PERFIL_ID
    JOIN PERMISO p ON p.ID = rp.PERMISO_ID
    WHERE p.NOMBRE = 'Fichar asistencia'
    ORDER BY u.APELLIDO, u.NOMBRE, u.USUARIO
GO

CREATE OR ALTER PROCEDURE dbo.TURNO_LISTAR_ASIGNADOS_RANGO
    @desde DATE,
    @hasta DATE
AS
    SELECT t.USUARIO_ID, u.USUARIO, u.NOMBRE, u.APELLIDO, g.SEMANA, f.ID AS FRANJA_ID, f.DIA, f.HORA_INICIO, f.HORA_FIN
    FROM TURNO t
    JOIN GRILLA_TURNOS g ON g.ID = t.GRILLA_ID
    JOIN FRANJA_HORARIA f ON f.ID = t.FRANJA_ID
    JOIN USUARIO u ON u.ID = t.USUARIO_ID
    WHERE t.USUARIO_ID IS NOT NULL
      AND t.ESTADO = 'Asignado'
      AND g.ESTADO IN ('Confirmada', 'Comunicada')
      AND g.SEMANA BETWEEN DATEADD(DAY, -6, @desde) AND @hasta
GO

CREATE OR ALTER PROCEDURE dbo.PARAMETRO_LIQUIDACION_LISTAR
AS
    SELECT CLAVE, VALOR, DESCRIPCION FROM PARAMETRO_LIQUIDACION
GO

CREATE OR ALTER PROCEDURE dbo.PARAMETRO_LIQUIDACION_GUARDAR
    @clave VARCHAR(50),
    @valor DECIMAL(10,2)
AS
    UPDATE PARAMETRO_LIQUIDACION SET VALOR = @valor WHERE CLAVE = @clave
GO

CREATE OR ALTER PROCEDURE dbo.VALOR_HORA_LISTAR
AS
    SELECT v.ID, v.USUARIO_ID, u.USUARIO, u.NOMBRE, u.APELLIDO, v.VALOR, v.VIGENTE_DESDE
    FROM VALOR_HORA_EMPLEADO v
    JOIN USUARIO u ON u.ID = v.USUARIO_ID
    ORDER BY u.APELLIDO, u.NOMBRE, v.VIGENTE_DESDE
GO

CREATE OR ALTER PROCEDURE dbo.VALOR_HORA_GUARDAR
    @usuario_id    INT,
    @valor         DECIMAL(10,2),
    @vigente_desde DATE,
    @creado_por    VARCHAR(50)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM VALOR_HORA_EMPLEADO WHERE USUARIO_ID = @usuario_id AND VIGENTE_DESDE = @vigente_desde)
        UPDATE VALOR_HORA_EMPLEADO SET VALOR = @valor, CREADO_POR = @creado_por
        WHERE USUARIO_ID = @usuario_id AND VIGENTE_DESDE = @vigente_desde
    ELSE
        INSERT INTO VALOR_HORA_EMPLEADO (USUARIO_ID, VALOR, VIGENTE_DESDE, CREADO_POR)
        VALUES (@usuario_id, @valor, @vigente_desde, @creado_por)
END
GO

CREATE OR ALTER PROCEDURE dbo.FERIADO_LISTAR
    @desde DATE,
    @hasta DATE
AS
    SELECT FECHA, DESCRIPCION FROM FERIADO WHERE FECHA BETWEEN @desde AND @hasta ORDER BY FECHA
GO

CREATE OR ALTER PROCEDURE dbo.FERIADO_GUARDAR
    @fecha       DATE,
    @descripcion VARCHAR(100)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM FERIADO WHERE FECHA = @fecha)
        UPDATE FERIADO SET DESCRIPCION = @descripcion WHERE FECHA = @fecha
    ELSE
        INSERT INTO FERIADO (FECHA, DESCRIPCION) VALUES (@fecha, @descripcion)
END
GO

CREATE OR ALTER PROCEDURE dbo.FERIADO_ELIMINAR
    @fecha DATE
AS
    DELETE FROM FERIADO WHERE FECHA = @fecha
GO

CREATE OR ALTER PROCEDURE dbo.EXCEDENTE_LISTAR
    @desde DATE,
    @hasta DATE
AS
    SELECT USUARIO_ID, FECHA FROM EXCEDENTE_APROBADO WHERE FECHA BETWEEN @desde AND @hasta
GO

CREATE OR ALTER PROCEDURE dbo.EXCEDENTE_APROBAR
    @usuario_id   INT,
    @fecha        DATE,
    @aprobado_por VARCHAR(50)
AS
    IF NOT EXISTS (SELECT 1 FROM EXCEDENTE_APROBADO WHERE USUARIO_ID = @usuario_id AND FECHA = @fecha)
        INSERT INTO EXCEDENTE_APROBADO (USUARIO_ID, FECHA, APROBADO_POR) VALUES (@usuario_id, @fecha, @aprobado_por)
GO

CREATE OR ALTER PROCEDURE dbo.EXCEDENTE_REVOCAR
    @usuario_id INT,
    @fecha      DATE
AS
    DELETE FROM EXCEDENTE_APROBADO WHERE USUARIO_ID = @usuario_id AND FECHA = @fecha
GO

CREATE OR ALTER PROCEDURE dbo.LIQUIDACION_INSERTAR
    @desde       DATE,
    @hasta       DATE,
    @cerrada_por VARCHAR(50)
AS
BEGIN
    INSERT INTO LIQUIDACION (DESDE, HASTA, ESTADO, CERRADA_POR) VALUES (@desde, @hasta, 'Cerrada', @cerrada_por)
    SELECT SCOPE_IDENTITY() AS ID
END
GO

CREATE OR ALTER PROCEDURE dbo.LIQUIDACION_DIA_INSERTAR
    @liquidacion_id      INT,
    @usuario_id          INT,
    @empleado            VARCHAR(120),
    @fecha               DATE,
    @ingreso             TIME = NULL,
    @egreso              TIME = NULL,
    @min_trabajados      INT,
    @min_asignados       INT,
    @min_pagables        INT,
    @min_normales        INT,
    @min_extra           INT,
    @min_feriado         INT,
    @min_excedente       INT,
    @excedente_aprobado  BIT,
    @ausente             BIT,
    @valor_hora          DECIMAL(10,2),
    @recargo_extra_pct   DECIMAL(10,2),
    @recargo_feriado_pct DECIMAL(10,2),
    @importe             DECIMAL(12,2),
    @observacion         VARCHAR(200) = NULL
AS
    INSERT INTO LIQUIDACION_DIA (LIQUIDACION_ID, USUARIO_ID, EMPLEADO, FECHA, INGRESO, EGRESO, MIN_TRABAJADOS, MIN_ASIGNADOS,
        MIN_PAGABLES, MIN_NORMALES, MIN_EXTRA, MIN_FERIADO, MIN_EXCEDENTE, EXCEDENTE_APROBADO, AUSENTE, VALOR_HORA,
        RECARGO_EXTRA_PCT, RECARGO_FERIADO_PCT, IMPORTE, OBSERVACION)
    VALUES (@liquidacion_id, @usuario_id, @empleado, @fecha, @ingreso, @egreso, @min_trabajados, @min_asignados,
        @min_pagables, @min_normales, @min_extra, @min_feriado, @min_excedente, @excedente_aprobado, @ausente, @valor_hora,
        @recargo_extra_pct, @recargo_feriado_pct, @importe, @observacion)
GO

CREATE OR ALTER PROCEDURE dbo.LIQUIDACION_LISTAR
AS
    SELECT ID, DESDE, HASTA, ESTADO, CERRADA_POR, FECHA_CIERRE, PAGADA_POR, FECHA_PAGO
    FROM LIQUIDACION ORDER BY DESDE DESC
GO

CREATE OR ALTER PROCEDURE dbo.LIQUIDACION_DIA_LISTAR
    @liquidacion_id INT
AS
    SELECT USUARIO_ID, EMPLEADO, FECHA, INGRESO, EGRESO, MIN_TRABAJADOS, MIN_ASIGNADOS, MIN_PAGABLES, MIN_NORMALES,
           MIN_EXTRA, MIN_FERIADO, MIN_EXCEDENTE, EXCEDENTE_APROBADO, AUSENTE, VALOR_HORA, RECARGO_EXTRA_PCT,
           RECARGO_FERIADO_PCT, IMPORTE, OBSERVACION
    FROM LIQUIDACION_DIA
    WHERE LIQUIDACION_ID = @liquidacion_id
    ORDER BY EMPLEADO, FECHA
GO

CREATE OR ALTER PROCEDURE dbo.LIQUIDACION_MARCAR_PAGADA
    @id         INT,
    @pagada_por VARCHAR(50)
AS
    UPDATE LIQUIDACION SET ESTADO = 'Pagada', PAGADA_POR = @pagada_por, FECHA_PAGO = GETDATE()
    WHERE ID = @id AND ESTADO = 'Cerrada'
GO

-- Cuántas liquidaciones cerradas se superponen con el rango (0 = libre).
CREATE OR ALTER PROCEDURE dbo.LIQUIDACION_CONTAR_SUPERPUESTAS
    @desde DATE,
    @hasta DATE
AS
    SELECT COUNT(*) AS CANTIDAD FROM LIQUIDACION WHERE DESDE <= @hasta AND HASTA >= @desde
GO

-- Permiso del caso de uso (altas en bases ya instaladas; el script de instalación lo siembra más abajo).
INSERT INTO PERMISO (NOMBRE)
SELECT 'Liquidar horas' WHERE NOT EXISTS (SELECT 1 FROM PERMISO WHERE NOMBRE = 'Liquidar horas')
GO
INSERT INTO ROL_PERMISO (ROL_ID, PERMISO_ID)
SELECT r.ID, p.ID FROM ROL r, PERMISO p
WHERE r.NOMBRE = 'Administrador' AND p.NOMBRE = 'Liquidar horas'
  AND NOT EXISTS (SELECT 1 FROM ROL_PERMISO rp WHERE rp.ROL_ID = r.ID AND rp.PERMISO_ID = p.ID)
GO
/****** <<< LIQUIDACION_HORAS (fin) ******/
