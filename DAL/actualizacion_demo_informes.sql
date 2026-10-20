USE [BDCAPAS]
GO
/****** >>> DEMO_E_INFORMES (inicio) — datos de demostración e informe de gestión (CU-40 y CU-41). Idempotente. ******/

CREATE OR ALTER PROCEDURE dbo.EQUIPO_INSERTAR
    @nombre              VARCHAR(100),
    @uso_acumulado       INT,
    @nivel_uso_critico   INT = NULL,
    @tecnico_habitual_id INT = NULL
AS
BEGIN
    INSERT INTO EQUIPO (NOMBRE, USO_ACUMULADO, NIVEL_USO_CRITICO, ESTADO, TECNICO_HABITUAL_ID)
    VALUES (@nombre, @uso_acumulado, @nivel_uso_critico, 'Operativo', @tecnico_habitual_id)
    SELECT SCOPE_IDENTITY() AS ID
END
GO

-- Uso con fecha propia (la demostración carga historia de días anteriores).
CREATE OR ALTER PROCEDURE dbo.EQUIPO_USO_LOG_INSERTAR_FECHA
    @equipo_id  INT,
    @fecha      DATETIME,
    @incremento INT
AS
    INSERT INTO EQUIPO_USO_LOG (EQUIPO_ID, FECHA, INCREMENTO) VALUES (@equipo_id, @fecha, @incremento)
GO

-- Última evaluación de cobertura de cada franja y semana dentro del rango.
CREATE OR ALTER PROCEDURE dbo.EVALUACION_COBERTURA_LISTAR_RANGO
    @desde DATE,
    @hasta DATE
AS
    SELECT e.SEMANA, e.FRANJA_ID, f.DIA, f.HORA_INICIO, f.HORA_FIN, f.ROL_REQUERIDO, e.RESULTADO
    FROM EVALUACION_COBERTURA e
    JOIN FRANJA_HORARIA f ON f.ID = e.FRANJA_ID
    WHERE e.SEMANA BETWEEN @desde AND @hasta
      AND e.ID = (SELECT MAX(x.ID) FROM EVALUACION_COBERTURA x WHERE x.FRANJA_ID = e.FRANJA_ID AND x.SEMANA = e.SEMANA)
    ORDER BY e.SEMANA, f.DIA, f.HORA_INICIO
GO

CREATE OR ALTER PROCEDURE dbo.HISTORIAL_ASISTENCIA_LISTAR_RANGO
    @desde DATE,
    @hasta DATE
AS
    SELECT FRANJA_ID, FECHA, CANTIDAD_SOCIOS
    FROM HISTORIAL_ASISTENCIA_SOCIOS
    WHERE FECHA BETWEEN @desde AND @hasta
    ORDER BY FECHA
GO

-- Cantidad de datos de negocio cargados: la aplicación pregunta por la demostración solo si el sistema está vacío.
CREATE OR ALTER PROCEDURE dbo.DATOS_NEGOCIO_CONTAR
AS
    SELECT (SELECT COUNT(*) FROM FRANJA_HORARIA) AS FRANJAS,
           (SELECT COUNT(*) FROM EQUIPO) AS EQUIPOS,
           (SELECT COUNT(*) FROM GRILLA_TURNOS) AS GRILLAS,
           (SELECT COUNT(*) FROM USUARIO) AS USUARIOS
GO

INSERT INTO PERMISO (NOMBRE)
SELECT v.NOMBRE FROM (VALUES ('Generar informes de gestión'), ('Cargar datos de demostración')) v(NOMBRE)
WHERE NOT EXISTS (SELECT 1 FROM PERMISO p WHERE p.NOMBRE = v.NOMBRE)
GO
INSERT INTO ROL_PERMISO (ROL_ID, PERMISO_ID)
SELECT r.ID, p.ID FROM ROL r, PERMISO p
WHERE r.NOMBRE = 'Administrador' AND p.NOMBRE IN ('Generar informes de gestión', 'Cargar datos de demostración')
  AND NOT EXISTS (SELECT 1 FROM ROL_PERMISO rp WHERE rp.ROL_ID = r.ID AND rp.PERMISO_ID = p.ID)
GO
/****** <<< DEMO_E_INFORMES (fin) ******/
