using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class UsuarioPerfilDAL
    {
        private readonly Acceso _acceso = new Acceso();

        public List<string> ListarPermisosDeUsuario(int usuarioId)
        {
            List<string> permisos = new List<string>();
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@usuario_id", usuarioId)
            };
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("USUARIO_PERMISOS_LISTAR", parametros);
                foreach (DataRow fila in tabla.Rows)
                    permisos.Add(fila["NOMBRE"].ToString());
            }
            finally
            {
                _acceso.Cerrar();
            }
            return permisos;
        }

        public List<int> ListarPerfilesPorUsuario(int usuarioId)
        {
            List<int> ids = new List<int>();
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@usuario_id", usuarioId)
            };
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("USUARIO_PERFIL_LISTAR", parametros);
                foreach (DataRow fila in tabla.Rows)
                    ids.Add(Convert.ToInt32(fila["PERFIL_ID"]));
            }
            finally
            {
                _acceso.Cerrar();
            }
            return ids;
        }

        public Dictionary<int, List<int>> ListarRolesDeUsuariosActivos()
        {
            Dictionary<int, List<int>> rolesPorUsuario = new Dictionary<int, List<int>>();
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("USUARIO_PERFIL_LISTAR_ACTIVOS");
                foreach (DataRow fila in tabla.Rows)
                {
                    int usuarioId = Convert.ToInt32(fila["USUARIO_ID"]);
                    if (!rolesPorUsuario.ContainsKey(usuarioId))
                        rolesPorUsuario[usuarioId] = new List<int>();
                    if (fila["PERFIL_ID"] != DBNull.Value)
                        rolesPorUsuario[usuarioId].Add(Convert.ToInt32(fila["PERFIL_ID"]));
                }
            }
            finally
            {
                _acceso.Cerrar();
            }
            return rolesPorUsuario;
        }

        public void ReemplazarAsignaciones(int usuarioId, List<int> perfilIds)
        {
            try
            {
                _acceso.Abrir();
                _acceso.IniciarTx();

                _acceso.Escribir("USUARIO_PERFIL_BORRAR_TODOS",
                    new List<SqlParameter> { _acceso.CrearParametro("@usuario_id", usuarioId) });

                foreach (int perfilId in perfilIds)
                {
                    _acceso.Escribir("USUARIO_PERFIL_ASIGNAR", new List<SqlParameter>
                    {
                        _acceso.CrearParametro("@usuario_id", usuarioId),
                        _acceso.CrearParametro("@perfil_id",  perfilId)
                    });
                }

                _acceso.ConfirmarTX();
            }
            catch
            {
                _acceso.DeshacerTX();
                throw;
            }
            finally
            {
                _acceso.Cerrar();
            }
        }
    }
}
