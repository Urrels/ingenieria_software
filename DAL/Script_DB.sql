using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    class Script_DB
    {
    }
}
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'BDCAPAS')
BEGIN
    CREATE DATABASE BDCAPAS;
END
GO

USE BDCAPAS;
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Usuarios' AND xtype='U')
BEGIN
    CREATE TABLE Usuarios (
        Id          INT PRIMARY KEY IDENTITY(1,1),
        Nombre      NVARCHAR(100) NOT NULL,
        Apellido    NVARCHAR(100) NOT NULL,
        Email       NVARCHAR(150) NOT NULL UNIQUE,
        Contrasena  NVARCHAR(256) NOT NULL,
        Rol         NVARCHAR(50)  NOT NULL DEFAULT 'Usuario'
    );

    INSERT INTO Usuarios (Nombre, Apellido, Email, Contrasena, Rol)
    VALUES ('Admin', 'Sistema', 'admin@sistema.com', 'admin123', 'Administrador');

    INSERT INTO Usuarios (Nombre, Apellido, Email, Contrasena, Rol)
    VALUES ('Juan', 'Perez', 'juan@sistema.com', 'juan123', 'Usuario');
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SP_Login')
    DROP PROCEDURE SP_Login;
GO

CREATE PROCEDURE SP_Login
    @Nombre     NVARCHAR(100),
    @Contrasena NVARCHAR(256)
AS
BEGIN
    SELECT Id, Nombre, Apellido, Email, Rol
    FROM Usuarios
    WHERE Nombre = @Nombre AND Contrasena = @Contrasena;
END
GO