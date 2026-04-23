-- =============================================
-- 1. CREAR BASE DE DATOS
-- =============================================
CREATE DATABASE [BDCAPAS]
GO

USE [BDCAPAS]
GO

-- =============================================
-- 2. CREAR TABLAS
-- =============================================
CREATE TABLE PERSONAS (
    ID       INT PRIMARY KEY IDENTITY,
    NOMBRE   VARCHAR(50),
    APELLIDO VARCHAR(50)
)
GO

-- ✅ ACÁ ESTÁ — Tabla de usuarios
CREATE TABLE USUARIOS (
    ID      INT PRIMARY KEY IDENTITY,
    USUARIO VARCHAR(50),
    PASS    VARCHAR(50)
)
GO

-- =============================================
-- 3. USUARIO DE PRUEBA
-- =============================================
-- ✅ ACÁ ESTÁ — Insert de prueba
INSERT INTO USUARIOS (USUARIO, PASS) VALUES ('admin', '1234')
GO

-- =============================================
-- 4. STORED PROCEDURES - PERSONA
-- =============================================
CREATE PROCEDURE PERSONA_INSERTAR
    @nom VARCHAR(50),
    @ape VARCHAR(50)
AS
    INSERT INTO PERSONAS (NOMBRE, APELLIDO) 
    VALUES (@nom, @ape)
GO

CREATE PROCEDURE PERSONA_EDITAR
    @ID  INT,
    @nom VARCHAR(50),
    @ape VARCHAR(50)
AS
    UPDATE PERSONAS 
    SET NOMBRE = @nom, APELLIDO = @ape 
    WHERE ID = @ID
GO

CREATE PROCEDURE PERSONA_BORRAR
    @ID INT
AS
    DELETE FROM PERSONAS 
    WHERE ID = @ID
GO

CREATE PROCEDURE PERSONA_LISTAR
AS
    SELECT ID, NOMBRE, APELLIDO 
    FROM PERSONAS
GO

-- =============================================
-- 5. STORED PROCEDURE - LOGIN
-- =============================================
-- ✅ ACÁ ESTÁ — SP de login
CREATE PROCEDURE USUARIO_LOGIN
    @usuario VARCHAR(50),
    @pass    VARCHAR(50)
AS
    SELECT ID, USUARIO 
    FROM USUARIOS
    WHERE USUARIO = @usuario AND PASS = @pass
GO

-- =============================================
-- 6. VERIFICAR QUE TODO SE CREO BIEN
-- =============================================
SELECT 'TABLAS:' AS INFO
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE'

SELECT 'STORED PROCEDURES:' AS INFO
SELECT NAME FROM SYS.PROCEDURES

SELECT 'USUARIOS CARGADOS:' AS INFO
SELECT * FROM USUARIOS
GO