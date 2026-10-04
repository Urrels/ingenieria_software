using BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class NotificacionDAL
    {
        private readonly Acceso _acceso = new Acceso();

        public int Guardar(BE.Notificacion notificacion)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                new SqlParameter("@grilla_id", SqlDbType.Int) { Value = (object)notificacion.GrillaId ?? DBNull.Value },
                _acceso.CrearParametro("@usuario_id", notificacion.UsuarioId),
                _acceso.CrearParametro("@mensaje", notificacion.Mensaje),
                _acceso.CrearParametro("@estado", notificacion.Estado)
            };
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("NOTIFICACION_INSERTAR", parametros);
                return Convert.ToInt32(tabla.Rows[0]["ID"]);
            }
            finally { _acceso.Cerrar(); }
        }

        public bool ExisteHoy(int usuarioId, string mensaje)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@usuario_id", usuarioId),
                _acceso.CrearParametro("@mensaje", mensaje)
            };
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("NOTIFICACION_EXISTE_HOY", parametros);
                return Convert.ToInt32(tabla.Rows[0]["CANTIDAD"]) > 0;
            }
            finally { _acceso.Cerrar(); }
        }

        public List<BE.Notificacion> ListarPorUsuario(int usuarioId)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
    {
        _acceso.CrearParametro("@usuario_id", usuarioId)
    };
            var lista = new List<BE.Notificacion>();
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("NOTIFICACION_LISTAR_POR_USUARIO", parametros);
                foreach (DataRow fila in tabla.Rows)
                {
                    lista.Add(new BE.Notificacion
                    {
                        Id = Convert.ToInt32(fila["ID"]),
                        GrillaId = fila["GRILLA_ID"] == DBNull.Value ? (int?)null : Convert.ToInt32(fila["GRILLA_ID"]),
                        UsuarioId = Convert.ToInt32(fila["USUARIO_ID"]),
                        Mensaje = fila["MENSAJE"].ToString(),
                        FechaEnvio = Convert.ToDateTime(fila["FECHA_ENVIO"]),
                        Estado = fila["ESTADO"].ToString()
                    });
                }
            }
            finally { _acceso.Cerrar(); }
            return lista;
        }
    }
}