using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace BLL
{
    public class BackupBLL
    {
        private const string PASS_MAESTRA = "2026";

        private static readonly string CarpetaBackup =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "backups");

        public bool VerificarPassword(string pass) => pass == PASS_MAESTRA;

        public string GenerarBackup()
        {
            if (!Directory.Exists(CarpetaBackup))
                Directory.CreateDirectory(CarpetaBackup);

            string archivo = Path.Combine(CarpetaBackup,
                $"backup_{DateTime.Now:yyyyMMdd_HHmmss}.sql");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("-- BACKUP CAPAS " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            sb.AppendLine("USE [BDCAPAS]");
            sb.AppendLine("GO");
            sb.AppendLine();

            sb.AppendLine("-- TABLAS");

            sb.AppendLine(@"IF OBJECT_ID('dbo.USUARIO', 'U') IS NULL
                CREATE TABLE [dbo].[USUARIO] (
                    [ID]                INT          IDENTITY(1,1) NOT NULL,
                    [USUARIO]           VARCHAR(50)  NULL,
                    [PASS]              VARCHAR(64)  NULL,
                    [INTENTOS_FALLIDOS] INT          NOT NULL DEFAULT 0,
                    [BLOQUEADO]         BIT          NOT NULL DEFAULT 0,
                    [ROL]               VARCHAR(20)  NOT NULL DEFAULT 'usuario',
                    [DVH]               INT          NOT NULL DEFAULT 0,
                    CONSTRAINT PK_USUARIO PRIMARY KEY ([ID])
                )");
                            sb.AppendLine("GO");

                            sb.AppendLine(@"IF OBJECT_ID('dbo.NODO_PERMISO', 'U') IS NULL
                CREATE TABLE [dbo].[NODO_PERMISO] (
                    [ID]        INT          IDENTITY(1,1) NOT NULL,
                    [NOMBRE]    VARCHAR(100) NOT NULL,
                    [TIPO]      VARCHAR(10)  NOT NULL,
                    [PADRE_ID]  INT          NULL,
                    [PROTEGIDO] BIT          NOT NULL DEFAULT 0,
                    CONSTRAINT PK_NODO_PERMISO PRIMARY KEY ([ID])
                )");
                            sb.AppendLine("GO");

                            sb.AppendLine(@"IF OBJECT_ID('dbo.ROL_PERMISO', 'U') IS NULL
                CREATE TABLE [dbo].[ROL_PERMISO] (
                    [ROL_ID]     INT NOT NULL,
                    [PERMISO_ID] INT NOT NULL,
                    CONSTRAINT PK_ROL_PERMISO     PRIMARY KEY ([ROL_ID], [PERMISO_ID]),
                    CONSTRAINT FK_ROLPERM_ROL     FOREIGN KEY ([ROL_ID])     REFERENCES [dbo].[NODO_PERMISO]([ID]),
                    CONSTRAINT FK_ROLPERM_PERMISO FOREIGN KEY ([PERMISO_ID]) REFERENCES [dbo].[NODO_PERMISO]([ID])
                )");
                            sb.AppendLine("GO");

                            sb.AppendLine(@"IF OBJECT_ID('dbo.USUARIO_PERFIL', 'U') IS NULL
                CREATE TABLE [dbo].[USUARIO_PERFIL] (
                    [USUARIO_ID] INT NOT NULL,
                    [PERFIL_ID]  INT NOT NULL,
                    CONSTRAINT PK_USUARIO_PERFIL PRIMARY KEY ([USUARIO_ID], [PERFIL_ID])
                )");
                            sb.AppendLine("GO");

                            sb.AppendLine(@"IF OBJECT_ID('dbo.BITACORA', 'U') IS NULL
                CREATE TABLE [dbo].[BITACORA] (
                    [ID]      INT          IDENTITY(1,1) NOT NULL,
                    [USUARIO] VARCHAR(50)  NULL,
                    [ACCION]  VARCHAR(50)  NULL,
                    [FECHA]   DATETIME     NULL,
                    CONSTRAINT PK_BITACORA PRIMARY KEY ([ID])
                )");
                            sb.AppendLine("GO");

                            sb.AppendLine(@"IF OBJECT_ID('dbo.IDIOMA', 'U') IS NULL
                CREATE TABLE [dbo].[IDIOMA] (
                    [ID]         INT         IDENTITY(1,1) NOT NULL,
                    [NOMBRE]     VARCHAR(50) NOT NULL,
                    [HABILITADO] BIT         NOT NULL DEFAULT 1,
                    CONSTRAINT PK_IDIOMA PRIMARY KEY ([ID])
                )");
                            sb.AppendLine("GO");

                            sb.AppendLine(@"IF OBJECT_ID('dbo.CONTROL_IDIOMA', 'U') IS NULL
                CREATE TABLE [dbo].[CONTROL_IDIOMA] (
                    [ID]            INT          IDENTITY(1,1) NOT NULL,
                    [CLAVE]         VARCHAR(100) NOT NULL UNIQUE,
                    [TEXTO_DEFAULT] VARCHAR(200) NOT NULL,
                    CONSTRAINT PK_CONTROL_IDIOMA PRIMARY KEY ([ID])
                )");
                            sb.AppendLine("GO");

                            sb.AppendLine(@"IF OBJECT_ID('dbo.TRADUCCION', 'U') IS NULL
                CREATE TABLE [dbo].[TRADUCCION] (
                    [IDIOMA_ID]  INT          NOT NULL,
                    [CONTROL_ID] INT          NOT NULL,
                    [TEXTO]      VARCHAR(200) NOT NULL,
                    CONSTRAINT PK_TRADUCCION   PRIMARY KEY ([IDIOMA_ID], [CONTROL_ID]),
                    CONSTRAINT FK_TRAD_IDIOMA  FOREIGN KEY ([IDIOMA_ID])  REFERENCES [dbo].[IDIOMA]([ID]),
                    CONSTRAINT FK_TRAD_CONTROL FOREIGN KEY ([CONTROL_ID]) REFERENCES [dbo].[CONTROL_IDIOMA]([ID])
                )");
                            sb.AppendLine("GO");

                            sb.AppendLine(@"IF OBJECT_ID('dbo.USUARIO_HISTORIAL', 'U') IS NULL
                CREATE TABLE [dbo].[USUARIO_HISTORIAL] (
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
                )");
                            sb.AppendLine("GO");

                            sb.AppendLine(@"IF OBJECT_ID('dbo.DIGITO_VERIFICADOR_VERTICAL', 'U') IS NULL
                CREATE TABLE [dbo].[DIGITO_VERIFICADOR_VERTICAL] (
                    [TABLA]   VARCHAR(50) NOT NULL,
                    [COLUMNA] VARCHAR(50) NOT NULL,
                    [DVV]     INT         NOT NULL DEFAULT 0,
                    CONSTRAINT PK_DVV PRIMARY KEY ([TABLA], [COLUMNA])
                )");
                            sb.AppendLine("GO");
                            sb.AppendLine();

            sb.AppendLine("-- USUARIOS");
            DataTable usuarios = new DAL.IntegridadDAL().ListarUsuariosParaIntegridad();
            foreach (DataRow r in usuarios.Rows)
            {
                sb.AppendLine($"IF NOT EXISTS (SELECT 1 FROM USUARIO WHERE USUARIO = '{r["USUARIO"]}')");
                sb.AppendLine($"    INSERT INTO USUARIO (USUARIO, PASS, ROL, INTENTOS_FALLIDOS, BLOQUEADO, DVH)");
                sb.AppendLine($"    VALUES ('{r["USUARIO"]}', '{r["PASS"]}', '{r["ROL"]}', " +
                              $"{r["INTENTOS_FALLIDOS"]}, {(Convert.ToBoolean(r["BLOQUEADO"]) ? 1 : 0)}, {r["DVH"]})");
            }
            sb.AppendLine("GO");
            sb.AppendLine();

            sb.AppendLine("-- PERFILES Y PERMISOS");
            DataTable nodos = new DAL.PerfilDAL().ListarTodosNodos();
            foreach (DataRow r in nodos.Rows)
            {
                string padre = r["PADRE_ID"] == DBNull.Value ? "NULL" : r["PADRE_ID"].ToString();
                sb.AppendLine($"IF NOT EXISTS (SELECT 1 FROM NODO_PERMISO WHERE NOMBRE = '{r["NOMBRE"]}' AND TIPO = '{r["TIPO"]}')");
                sb.AppendLine($"    INSERT INTO NODO_PERMISO (NOMBRE, TIPO, PADRE_ID, PROTEGIDO)");
                sb.AppendLine($"    VALUES ('{r["NOMBRE"]}', '{r["TIPO"]}', {padre}, {r["PROTEGIDO"]})");
            }
            sb.AppendLine("GO");
            sb.AppendLine();

            sb.AppendLine("-- ROL_PERMISO");
            DataTable rolPermisos = new DAL.PerfilDAL().ListarTodosRolPermiso();
            foreach (DataRow r in rolPermisos.Rows)
            {
                sb.AppendLine($"IF NOT EXISTS (SELECT 1 FROM ROL_PERMISO WHERE ROL_ID = {r["ROL_ID"]} AND PERMISO_ID = {r["PERMISO_ID"]})");
                sb.AppendLine($"    INSERT INTO ROL_PERMISO (ROL_ID, PERMISO_ID) VALUES ({r["ROL_ID"]}, {r["PERMISO_ID"]})");
            }
            sb.AppendLine("GO");
            sb.AppendLine();

            sb.AppendLine("-- USUARIO_PERFIL");
            DataTable perfiles = new DAL.UsuarioPerfilDAL().ListarTodosPerfiles();
            foreach (DataRow r in perfiles.Rows)
            {
                sb.AppendLine($"IF NOT EXISTS (SELECT 1 FROM USUARIO_PERFIL WHERE USUARIO_ID = {r["USUARIO_ID"]} AND PERFIL_ID = {r["PERFIL_ID"]})");
                sb.AppendLine($"    INSERT INTO USUARIO_PERFIL (USUARIO_ID, PERFIL_ID) VALUES ({r["USUARIO_ID"]}, {r["PERFIL_ID"]})");
            }
            sb.AppendLine("GO");

            File.WriteAllText(archivo, sb.ToString(), Encoding.UTF8);

            LimpiarBackupsAntiguos(CarpetaBackup, 10);

            return archivo;
        }

        public string ObtenerUltimoBackup()
        {
            if (!Directory.Exists(CarpetaBackup)) return null;
            string[] archivos = Directory.GetFiles(CarpetaBackup, "backup_*.sql");
            if (archivos.Length == 0) return null;
            Array.Sort(archivos);
            return archivos[archivos.Length - 1];
        }

        public ResultadoSistema VerificarSistema()
        {
            ResultadoSistema resultado = new ResultadoSistema();
            try
            {
                DAL.SistemaDAL dal = new DAL.SistemaDAL();

                List<string> tablasFaltantes = dal.ObtenerTablasFaltantes();
                foreach (string t in tablasFaltantes)
                    resultado.Problemas.Add($"Tabla faltante: {t}");

                if (!dal.ExisteAdmin())
                    resultado.Problemas.Add("No existe ningún usuario administrador.");

                resultado.EstaIntegro = resultado.Problemas.Count == 0;
            }
            catch (Exception ex)
            {
                resultado.EstaIntegro = false;
                resultado.Problemas.Add("Error al verificar: " + ex.Message);
            }
            return resultado;
        }

        public void RestaurarDesdeArchivo(string archivo)
        {
            string sql = System.IO.File.ReadAllText(archivo);
            new DAL.ScriptDAL().EjecutarScript(sql);

            new DAL.ScriptDAL().EjecutarDirectoTexto(@"
                DECLARE @rolAdminId INT = (SELECT ID FROM NODO_PERMISO 
                    WHERE NOMBRE = 'Administrador' AND TIPO = 'PERFIL' AND PADRE_ID IS NULL)

                -- limpiar y reasignar todos los permisos al Administrador
                DELETE FROM ROL_PERMISO WHERE ROL_ID = @rolAdminId

                INSERT INTO ROL_PERMISO (ROL_ID, PERMISO_ID)
                SELECT @rolAdminId, ID FROM NODO_PERMISO
                WHERE TIPO = 'PERMISO' AND PADRE_ID IS NULL

                -- reasignar perfil Administrador a usuarios con ROL = 'admin'
                DELETE FROM USUARIO_PERFIL 
                WHERE USUARIO_ID IN (SELECT ID FROM USUARIO WHERE ROL = 'admin')

                INSERT INTO USUARIO_PERFIL (USUARIO_ID, PERFIL_ID)
                SELECT u.ID, @rolAdminId 
                FROM USUARIO u
                WHERE u.ROL = 'admin'
                    AND NOT EXISTS (
                        SELECT 1 FROM USUARIO_PERFIL 
                        WHERE USUARIO_ID = u.ID AND PERFIL_ID = @rolAdminId)
            ");

   
            new DAL.ScriptDAL().EjecutarDirectoTexto(
                "DELETE FROM DIGITO_VERIFICADOR_VERTICAL WHERE TABLA = 'USUARIO'");

            new IntegridadBLL().RecalcularIntegridad();
        }
        private static readonly string CarpetaFabrica = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "backups_fabrica");

        public string GenerarBackupFabrica()
        {
            if (!Directory.Exists(CarpetaFabrica) ||
                Directory.GetFiles(CarpetaFabrica, "*.sql").Length == 0)
            {
                if (!Directory.Exists(CarpetaFabrica))
                    Directory.CreateDirectory(CarpetaFabrica);

                string archivo = Path.Combine(CarpetaFabrica,
                    $"fabrica_{DateTime.Now:yyyyMMdd_HHmmss}.sql");

                string scriptOrigen = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, "BDCAPAS_setup.sql");

                if (File.Exists(scriptOrigen))
                {
                    File.Copy(scriptOrigen, archivo, true);
                    return archivo;
                }

                return null;
            }
            return null;
        }

        public string ObtenerBackupFabrica()
        {
            if (!Directory.Exists(CarpetaFabrica)) return null;
            string[] archivos = Directory.GetFiles(CarpetaFabrica, "*.sql");
            if (archivos.Length == 0) return null;
            return archivos[0];
        }

        private void LimpiarBackupsAntiguos(string carpeta, int maxGuardar)
        {
            string[] archivos = Directory.GetFiles(carpeta, "backup_*.sql");
            Array.Sort(archivos);
            while (archivos.Length > maxGuardar)
            {
                File.Delete(archivos[0]);
                archivos = Directory.GetFiles(carpeta, "backup_*.sql");
                Array.Sort(archivos);
            }
        }
    }
}