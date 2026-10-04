using System;
using System.Data.SqlClient;

namespace DAL
{
    public class ConexionDAL
    {
        private readonly Acceso _acceso = new Acceso();

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
