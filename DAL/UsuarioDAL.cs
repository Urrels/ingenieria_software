using BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class UsuarioDAL
    {
        private readonly Acceso _acceso = new Acceso();

        public BE.USUARIO ObtenerPorCredenciales(string usuario, string contrasena)
        {
            BE.USUARIO u = null;

            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@usuario",   usuario),
                _acceso.CrearParametro("@pass",      contrasena)
            };

            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("USUARIO_LOGIN", parametros);

                if (tabla.Rows.Count > 0)
                {
                    DataRow fila = tabla.Rows[0];
                    u = new BE.USUARIO
                    {
                        Id = Convert.ToInt32(fila["ID"]),
                        Usuario = fila["USUARIO"].ToString()
                    };
                }
            }
            finally
            {
                _acceso.Cerrar();
            }

            return u;
        }
    }
}