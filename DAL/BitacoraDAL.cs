using BE;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class BitacoraDAL
    {
        private readonly Acceso _acceso = new Acceso();

        public void Registrar(string usuario, string accion)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@usuario", usuario),
                _acceso.CrearParametro("@accion",  accion)
            };
            try
            {
                _acceso.Abrir();
                _acceso.Escribir("BITACORA_INSERTAR", parametros);
            }
            finally
            {
                _acceso.Cerrar();
            }
        }

        public List<BE.BITACORA> Listar()
        {
            List<BE.BITACORA> lista = new List<BE.BITACORA>();
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("BITACORA_LISTAR");
                foreach (DataRow fila in tabla.Rows)
                {
                    BE.BITACORA b = new BE.BITACORA();
                    b.Id = int.Parse(fila["ID"].ToString());
                    b.Usuario = fila["USUARIO"].ToString();
                    b.Accion = fila["ACCION"].ToString();
                    b.Fecha = System.DateTime.Parse(fila["FECHA"].ToString());
                    lista.Add(b);
                }
            }
            finally
            {
                _acceso.Cerrar();
            }
            return lista;
        }
    }
}