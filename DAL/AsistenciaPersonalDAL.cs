using BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class AsistenciaPersonalDAL
    {
        private readonly Acceso _acceso = new Acceso();

        public BE.RegistroAsistenciaPersonal ObtenerAbierta(int usuarioId, DateTime fecha)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@usuario_id", usuarioId),
                new SqlParameter("@fecha", fecha)
            };
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("ASISTENCIA_OBTENER_ABIERTA", parametros);
                if (tabla.Rows.Count == 0) return null;

                DataRow fila = tabla.Rows[0];
                return new BE.RegistroAsistenciaPersonal
                {
                    Id = Convert.ToInt32(fila["ID"]),
                    UsuarioId = Convert.ToInt32(fila["USUARIO_ID"]),
                    Fecha = Convert.ToDateTime(fila["FECHA"]),
                    HoraIngreso = (TimeSpan)fila["HORA_INGRESO"]
                };
            }
            finally { _acceso.Cerrar(); }
        }

        public int RegistrarIngreso(int usuarioId, DateTime fecha, TimeSpan horaIngreso)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@usuario_id", usuarioId),
                new SqlParameter("@fecha", fecha),
                new SqlParameter("@hora_ingreso", horaIngreso)
            };
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("ASISTENCIA_REGISTRAR_INGRESO", parametros);
                return Convert.ToInt32(tabla.Rows[0]["ID"]);
            }
            finally { _acceso.Cerrar(); }
        }

        public void RegistrarEgreso(int id, TimeSpan horaEgreso)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@id", id),
                new SqlParameter("@hora_egreso", horaEgreso)
            };
            try
            {
                _acceso.Abrir();
                _acceso.Escribir("ASISTENCIA_REGISTRAR_EGRESO", parametros);
            }
            finally { _acceso.Cerrar(); }
        }
    }
}