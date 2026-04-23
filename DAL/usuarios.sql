---- Tabla de usuarios
--CREATE TABLE USUARIOS (
--    ID      INT PRIMARY KEY IDENTITY,
--    USUARIO VARCHAR(50),
--    PASS    VARCHAR(50)
--)

---- Stored Procedure de login
--CREATE PROCEDURE USUARIO_LOGIN
--    @usuario VARCHAR(50),
--    @pass    VARCHAR(50)
--AS
--    SELECT ID, USUARIO
--    FROM USUARIOS
--    WHERE USUARIO = @usuario AND PASS = @pass

---- Insertar un usuario de prueba
--INSERT INTO USUARIOS (USUARIO, PASS) VALUES ('admin', '1234')