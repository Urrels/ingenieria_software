using BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class FranjaHorariaDAL
    {
        private readonly Acceso _acceso = new Acceso();

        public List<BE.FranjaHoraria> ListarTodas()
        {
            var lista = new List<BE.FranjaHoraria>();
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("FRANJA_HORARIA_LISTAR_TODAS", null);
                foreach (DataRow fila in tabla.Rows)
                    lista.Add(Mapear(fila));
            }
            finally { _acceso.Cerrar(); }
            return lista;
        }

        public BE.FranjaHoraria ObtenerPorId(int id)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@id", id)
            };
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("FRANJA_HORARIA_OBTENER", parametros);
                return tabla.Rows.Count == 0 ? null : Mapear(tabla.Rows[0]);
            }
            finally { _acceso.Cerrar(); }
        }

        public int Insertar(string dia, TimeSpan horaInicio, TimeSpan horaFin, string rolRequerido)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@dia", dia),
                new SqlParameter("@hora_inicio", horaInicio),
                new SqlParameter("@hora_fin", horaFin),
                _acceso.CrearParametro("@rol_requerido", rolRequerido)
            };
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("FRANJA_HORARIA_INSERTAR", parametros);
                return Convert.ToInt32(tabla.Rows[0]["ID"]);
            }
            finally { _acceso.Cerrar(); }
        }

        // Devuelve false si el SP rechazó el borrado por tener referencias (disponibilidad, turnos, historial)
        public bool Eliminar(int id)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@id", id)
            };
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("FRANJA_HORARIA_ELIMINAR", parametros);
                return tabla.Rows.Count > 0 && Convert.ToInt32(tabla.Rows[0]["OK"]) == 1;
            }
            finally { _acceso.Cerrar(); }
        }

        private BE.FranjaHoraria Mapear(DataRow fila) => new BE.FranjaHoraria
        {
            Id = Convert.ToInt32(fila["ID"]),
            Dia = fila["DIA"].ToString(),
            HoraInicio = (TimeSpan)fila["HORA_INICIO"],
            HoraFin = (TimeSpan)fila["HORA_FIN"],
            RolRequerido = fila["ROL_REQUERIDO"].ToString()
        };
    }
}