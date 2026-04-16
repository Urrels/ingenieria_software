using BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class UsuarioDAL
    {
        private readonly Acceso _acceso = new Acceso();

        public Usuario ObtenerPorCredenciales(string nombre, string contrasena)
        {
            Usuario usuario = null;

            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@Nombre",      nombre),
                _acceso.CrearParametro("@Contrasena",  contrasena)
            };

            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("SP_Login", parametros);

                if (tabla.Rows.Count > 0)
                {
                    DataRow fila = tabla.Rows[0];
                    usuario = new Usuario
                    {
                        Id = Convert.ToInt32(fila["Id"]),
                        Nombre = fila["Nombre"].ToString(),
                        Apellido = fila["Apellido"].ToString(),
                        Email = fila["Email"].ToString(),
                        Rol = fila["Rol"].ToString(),
                        Contrasena = contrasena
                    };
                }
            }
            finally
            {
                _acceso.Cerrar();
            }

            return usuario;
        }
    }
}