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
                _acceso.CrearParametro("@grilla_id", notificacion.GrillaId),
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
    }
}