using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ConfiguradorBD
{
    internal class InstaladorBaseDatos
    {
        internal const string NombreBase = "BDCAPAS";
        private const int VersionMinima = 15;
        private static readonly Regex SeparadorLotes =
            new Regex(@"^\s*GO\s*(--.*)?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private readonly SqlConnectionStringBuilder _cadena;
        private readonly RegistroInstalacion _log;

        internal InstaladorBaseDatos(SqlConnectionStringBuilder cadena, RegistroInstalacion log)
        {
            _cadena = cadena;
            _log = log;
        }

        internal string CadenaParaAplicacion()
        {
            var cadena = new SqlConnectionStringBuilder(_cadena.ConnectionString) { InitialCatalog = NombreBase };
            cadena.Remove("Connect Timeout");
            return cadena.ConnectionString;
        }

        internal ResultadoVerificacion Verificar()
        {
            using (SqlConnection conexion = AbrirMaster())
            {
                var resultado = new ResultadoVerificacion
                {
                    Version = Convert.ToString(Escalar(conexion, "SELECT SERVERPROPERTY('ProductVersion')")),
                    Edicion = Convert.ToString(Escalar(conexion, "SELECT SERVERPROPERTY('Edition')")),
                    ExisteBase = Escalar(conexion, $"SELECT DB_ID('{NombreBase}')") != DBNull.Value,
                    PuedeCrearBases = Convert.ToInt32(Escalar(conexion,
                        "SELECT HAS_PERMS_BY_NAME(NULL, NULL, 'CREATE ANY DATABASE')")) == 1
                };
                int major;
                int.TryParse(resultado.Version.Split('.')[0], out major);
                resultado.VersionCompatible = major >= VersionMinima;
                return resultado;
            }
        }

        internal void CrearBase(string rutaEsquema, string rutaTraducciones)
        {
            _log.Escribir("Creando la base de datos " + NombreBase + "...");
            try
            {
                EjecutarScript(rutaEsquema);
                EjecutarScript(rutaTraducciones);
            }
            catch (Exception ex)
            {
                _log.Escribir("ERROR: " + ex.Message);
                Deshacer();
                throw;
            }
            _log.Escribir("Base de datos creada con sus tablas, procedimientos, datos iniciales y traducciones.");
        }

        internal void ActualizarTraducciones(string rutaTraducciones)
        {
            _log.Escribir("La base ya existe: se actualizan solo las traducciones.");
            EjecutarScript(rutaTraducciones);
        }

        internal static List<string> SepararLotes(string script)
        {
            var lotes = new List<string>();
            var actual = new StringBuilder();
            foreach (string linea in script.Replace("\r\n", "\n").Split('\n'))
            {
                if (SeparadorLotes.IsMatch(linea))
                {
                    AgregarLote(lotes, actual);
                    continue;
                }
                actual.AppendLine(linea);
            }
            AgregarLote(lotes, actual);
            return lotes;
        }

        private static void AgregarLote(List<string> lotes, StringBuilder actual)
        {
            string lote = actual.ToString();
            if (lote.Trim().Length > 0) lotes.Add(lote);
            actual.Clear();
        }

        private void EjecutarScript(string ruta)
        {
            List<string> lotes = SepararLotes(File.ReadAllText(ruta, Encoding.UTF8));
            _log.Escribir($"Ejecutando {Path.GetFileName(ruta)} ({lotes.Count} lotes)...");
            using (SqlConnection conexion = AbrirMaster())
            {
                conexion.InfoMessage += (s, e) => _log.Escribir("SQL: " + e.Message);
                for (int i = 0; i < lotes.Count; i++)
                {
                    try
                    {
                        using (var comando = new SqlCommand(lotes[i], conexion) { CommandTimeout = 600 })
                            comando.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        throw new InvalidOperationException(
                            $"Falló el lote {i + 1} de {Path.GetFileName(ruta)}: {ex.Message}", ex);
                    }
                }
            }
            _log.Escribir(Path.GetFileName(ruta) + " ejecutado correctamente.");
        }

        private void Deshacer()
        {
            try
            {
                SqlConnection.ClearAllPools();
                using (SqlConnection conexion = AbrirMaster())
                {
                    if (Escalar(conexion, $"SELECT DB_ID('{NombreBase}')") == DBNull.Value) return;
                    _log.Escribir("Rollback: se elimina la base creada parcialmente.");
                    using (var comando = new SqlCommand(
                        $"ALTER DATABASE [{NombreBase}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{NombreBase}];",
                        conexion) { CommandTimeout = 120 })
                        comando.ExecuteNonQuery();
                    _log.Escribir("Rollback completado: el servidor quedó como estaba antes de la instalación.");
                }
            }
            catch (SqlException ex)
            {
                _log.Escribir("No se pudo completar el rollback: " + ex.Message +
                              ". Eliminá la base " + NombreBase + " a mano antes de reintentar.");
            }
        }

        private SqlConnection AbrirMaster()
        {
            var cadena = new SqlConnectionStringBuilder(_cadena.ConnectionString) { InitialCatalog = "master" };
            var conexion = new SqlConnection(cadena.ConnectionString);
            conexion.Open();
            return conexion;
        }

        private static object Escalar(SqlConnection conexion, string sql)
        {
            using (var comando = new SqlCommand(sql, conexion))
                return comando.ExecuteScalar() ?? DBNull.Value;
        }
    }

    internal class ResultadoVerificacion
    {
        internal string Version { get; set; }
        internal string Edicion { get; set; }
        internal bool VersionCompatible { get; set; }
        internal bool ExisteBase { get; set; }
        internal bool PuedeCrearBases { get; set; }
    }
}
