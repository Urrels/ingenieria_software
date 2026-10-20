using System;
using System.Data.SqlClient;

namespace DAL
{
    public class ConexionDAL
    {
        private readonly Acceso _acceso = new Acceso();

        public string Identificador()
        {
            var configuracion = System.Configuration.ConfigurationManager.ConnectionStrings["BDCAPAS"];
            if (configuracion == null) return "";
            var cadena = new SqlConnectionStringBuilder(configuracion.ConnectionString);
            return cadena.DataSource + "|" + cadena.InitialCatalog;
        }

        public bool Probar()
        {
            try
            {
                _acceso.Abrir();
                return true;
            }
            catch (SqlException)
            {
                return false;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            finally
            {
                _acceso.Cerrar();
            }
        }
    }
}
