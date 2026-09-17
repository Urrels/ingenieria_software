using BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class TurnoDAL
    {
        private readonly Acceso _acceso = new Acceso();

        public int Agregar(BE.Turno turno)
        {
            SqlParameter pUsuario = turno.UsuarioId.HasValue
                ? _acceso.CrearParametro("@usuario_id", turno.UsuarioId.Value)
                : new SqlParameter("@usuario_id", DBNull.Value);

            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@grilla_id", turno.GrillaId),
                _acceso.CrearParametro("@franja_id", turno.FranjaId),
                pUsuario,
                _acceso.CrearParametro("@rol_requerido", turno.RolRequerido),
                _acceso.CrearParametro("@estado", turno.Estado)
            };
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("TURNO_INSERTAR", parametros);
                return Convert.ToInt32(tabla.Rows[0]["ID"]);
            }
            finally { _acceso.Cerrar(); }
        }

        public void ActualizarAsignacion(int turnoId, int usuarioId, string estado)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@turno_id", turnoId),
                _acceso.CrearParametro("@usuario_id", usuarioId),
                _acceso.CrearParametro("@estado", estado)
            };
            try
            {
                _acceso.Abrir();
                _acceso.Escribir("TURNO_ACTUALIZAR_ASIGNACION", parametros);
            }
            finally { _acceso.Cerrar(); }
        }

        public List<BE.USUARIO> BuscarCompatibles(string rolRequerido, int franjaId)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@rol_requerido", rolRequerido),
                _acceso.CrearParametro("@franja_id", franjaId)
            };
            var lista = new List<BE.USUARIO>();
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("TURNO_BUSCAR_COMPATIBLES", parametros);
                foreach (DataRow fila in tabla.Rows)
                {
                    lista.Add(new BE.USUARIO
                    {
                        Id = Convert.ToInt32(fila["ID"]),
                        Usuario = fila["USUARIO"].ToString(),
                        Nombre = fila["NOMBRE"].ToString(),
                        Apellido = fila["APELLIDO"].ToString(),
                        LimiteHorasSemanales = fila["LIMITE_HORAS_SEMANALES"] == DBNull.Value
                            ? (int?)null
                            : Convert.ToInt32(fila["LIMITE_HORAS_SEMANALES"])
                    });
                }
            }
            finally { _acceso.Cerrar(); }
            return lista;
        }

        public decimal HorasAsignadasEnSemana(int usuarioId, int grillaId)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@usuario_id", usuarioId),
                _acceso.CrearParametro("@grilla_id", grillaId)
            };
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("TURNO_HORAS_ASIGNADAS_SEMANA", parametros);
                return tabla.Rows.Count == 0 ? 0 : Convert.ToDecimal(tabla.Rows[0]["HORAS"]);
            }
            finally { _acceso.Cerrar(); }
        }
    }
}