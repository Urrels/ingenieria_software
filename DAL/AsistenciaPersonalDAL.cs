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

        public BE.RegistroAsistenciaPersonal ObtenerAbierta(int usuarioId)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@usuario_id", usuarioId)
            };
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("ASISTENCIA_OBTENER_ABIERTA", parametros);
                if (tabla.Rows.Count == 0) return null;
                return Mapear(tabla.Rows[0]);
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

        public List<BE.RegistroAsistenciaPersonal> ListarRango(DateTime desde, DateTime hasta, int? usuarioId = null)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                new SqlParameter("@desde", desde.Date),
                new SqlParameter("@hasta", hasta.Date),
                new SqlParameter("@usuario_id", SqlDbType.Int) { Value = (object)usuarioId ?? DBNull.Value }
            };
            var lista = new List<BE.RegistroAsistenciaPersonal>();
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("ASISTENCIA_LISTAR_RANGO", parametros);
                foreach (DataRow fila in tabla.Rows)
                    lista.Add(Mapear(fila));
            }
            finally { _acceso.Cerrar(); }
            return lista;
        }

        public BE.RegistroAsistenciaPersonal ObtenerPorId(int id)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@id", id)
            };
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("ASISTENCIA_OBTENER_POR_ID", parametros);
                return tabla.Rows.Count == 0 ? null : Mapear(tabla.Rows[0]);
            }
            finally { _acceso.Cerrar(); }
        }

        public void Corregir(int id, TimeSpan horaIngreso, TimeSpan horaEgreso, string motivo, string corregidoPor)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@id", id),
                new SqlParameter("@hora_ingreso", horaIngreso),
                new SqlParameter("@hora_egreso", horaEgreso),
                _acceso.CrearParametro("@motivo", motivo),
                _acceso.CrearParametro("@corregido_por", corregidoPor)
            };
            try
            {
                _acceso.Abrir();
                _acceso.Escribir("ASISTENCIA_CORREGIR", parametros);
            }
            finally { _acceso.Cerrar(); }
        }

        public int InsertarManual(int usuarioId, DateTime fecha, TimeSpan horaIngreso, TimeSpan horaEgreso,
            string motivo, string corregidoPor)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@usuario_id", usuarioId),
                new SqlParameter("@fecha", fecha.Date),
                new SqlParameter("@hora_ingreso", horaIngreso),
                new SqlParameter("@hora_egreso", horaEgreso),
                _acceso.CrearParametro("@motivo", motivo),
                _acceso.CrearParametro("@corregido_por", corregidoPor)
            };
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("ASISTENCIA_INSERTAR_MANUAL", parametros);
                return Convert.ToInt32(tabla.Rows[0]["ID"]);
            }
            finally { _acceso.Cerrar(); }
        }

        private static BE.RegistroAsistenciaPersonal Mapear(DataRow fila)
        {
            var registro = new BE.RegistroAsistenciaPersonal
            {
                Id = Convert.ToInt32(fila["ID"]),
                UsuarioId = Convert.ToInt32(fila["USUARIO_ID"]),
                Fecha = Convert.ToDateTime(fila["FECHA"]),
                HoraIngreso = (TimeSpan)fila["HORA_INGRESO"],
                HoraEgreso = fila["HORA_EGRESO"] == DBNull.Value ? (TimeSpan?)null : (TimeSpan)fila["HORA_EGRESO"]
            };
            if (fila.Table.Columns.Contains("USUARIO"))
                registro.Empleado = NombreEmpleado(fila);
            if (fila.Table.Columns.Contains("ROL") && fila["ROL"] != DBNull.Value)
                registro.Rol = fila["ROL"].ToString();
            if (fila.Table.Columns.Contains("CORREGIDO"))
            {
                registro.Corregido = Convert.ToBoolean(fila["CORREGIDO"]);
                registro.MotivoCorreccion = fila["MOTIVO_CORRECCION"] == DBNull.Value
                    ? null : fila["MOTIVO_CORRECCION"].ToString();
            }
            return registro;
        }

        internal static string NombreEmpleado(DataRow fila)
        {
            string nombre = fila["NOMBRE"] == DBNull.Value ? "" : fila["NOMBRE"].ToString();
            string apellido = fila["APELLIDO"] == DBNull.Value ? "" : fila["APELLIDO"].ToString();
            string completo = (apellido + ", " + nombre).Trim(' ', ',');
            return completo.Length == 0 ? fila["USUARIO"].ToString() : completo;
        }
    }
}
