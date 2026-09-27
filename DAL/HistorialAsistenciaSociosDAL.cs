using BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class HistorialAsistenciaSociosDAL
    {
        private readonly Acceso _acceso = new Acceso();

        public int Insertar(int franjaId, DateTime fecha, int cantidadSocios)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@franja_id", franjaId),
                new SqlParameter("@fecha", fecha),
                _acceso.CrearParametro("@cantidad_socios", cantidadSocios)
            };
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("HISTORIAL_ASISTENCIA_INSERTAR", parametros);
                return Convert.ToInt32(tabla.Rows[0]["ID"]);
            }
            finally { _acceso.Cerrar(); }
        }

        public List<int> ObtenerPorFranja(int franjaId, int semanas)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@franja_id", franjaId),
                _acceso.CrearParametro("@semanas", semanas)
            };
            var lista = new List<int>();
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("HISTORIAL_ASISTENCIA_OBTENER_POR_FRANJA", parametros);
                foreach (DataRow fila in tabla.Rows)
                    lista.Add(Convert.ToInt32(fila["CANTIDAD_SOCIOS"]));
            }
            finally { _acceso.Cerrar(); }
            return lista;
        }

        public List<int> ObtenerPorFranjaYSemana(int franjaId, DateTime lunes)
        {
            DateTime domingo = lunes.AddDays(6);

            List<SqlParameter> parametros = new List<SqlParameter>
    {
        _acceso.CrearParametro("@franja_id", franjaId),
        new SqlParameter("@fecha_inicio", lunes),
        new SqlParameter("@fecha_fin", domingo)
    };
            var lista = new List<int>();
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("HISTORIAL_ASISTENCIA_OBTENER_POR_FRANJA_Y_SEMANA", parametros);
                foreach (DataRow fila in tabla.Rows)
                    lista.Add(Convert.ToInt32(fila["CANTIDAD_SOCIOS"]));
            }
            finally { _acceso.Cerrar(); }
            return lista;
        }
    }
}