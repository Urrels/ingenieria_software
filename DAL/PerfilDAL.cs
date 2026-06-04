using BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class PerfilDAL
    {
        private readonly Acceso _acceso = new Acceso();

        public List<NodoPermiso> ListarTodos()
        {
            List<NodoPermiso> lista = new List<NodoPermiso>();
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("PERFIL_LISTAR_TODOS");
                foreach (DataRow fila in tabla.Rows)
                {
                    string tipo = fila["TIPO"].ToString();
                    NodoPermiso nodo = tipo == "PERFIL"
                        ? (NodoPermiso)new PerfilPermiso()
                        : new Permiso();

                    nodo.Id = Convert.ToInt32(fila["ID"]);
                    nodo.Nombre = fila["NOMBRE"].ToString();
                    nodo.PadreId = fila["PADRE_ID"] == DBNull.Value
                        ? (int?)null
                        : Convert.ToInt32(fila["PADRE_ID"]);

                    lista.Add(nodo);
                }
            }
            finally
            {
                _acceso.Cerrar();
            }
            return lista;
        }

        public int Insertar(string nombre, string tipo, int? padreId)
        {
            SqlParameter paramPadre = new SqlParameter("@padre_id", DbType.Int32);
            paramPadre.Value = padreId.HasValue ? (object)padreId.Value : DBNull.Value;

            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@nombre", nombre),
                _acceso.CrearParametro("@tipo",   tipo),
                paramPadre
            };
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("PERFIL_INSERTAR", parametros);
                return Convert.ToInt32(tabla.Rows[0]["ID"]);
            }
            finally
            {
                _acceso.Cerrar();
            }
        }

        public void Eliminar(int id)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@id", id)
            };
            try
            {
                _acceso.Abrir();
                _acceso.Escribir("PERFIL_ELIMINAR", parametros);
            }
            finally
            {
                _acceso.Cerrar();
            }
        }
    }
}
