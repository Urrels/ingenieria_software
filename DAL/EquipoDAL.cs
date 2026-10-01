using BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class EquipoDAL
    {
        private readonly Acceso _acceso = new Acceso();

        public List<BE.Equipo> ListarTodos()
        {
            var lista = new List<BE.Equipo>();
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("EQUIPO_LISTAR_TODOS", null);
                foreach (DataRow fila in tabla.Rows)
                    lista.Add(Mapear(fila));
            }
            finally { _acceso.Cerrar(); }
            return lista;
        }

        public BE.Equipo ObtenerPorId(int id)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@id", id)
            };
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("EQUIPO_OBTENER", parametros);
                return tabla.Rows.Count == 0 ? null : Mapear(tabla.Rows[0]);
            }
            finally { _acceso.Cerrar(); }
        }

        // Devuelve el uso acumulado y el nivel crítico ya actualizados, para que el BLL
        // pueda evaluar sin pegarle una segunda vuelta a la base.
        public (int usoAcumulado, int? nivelUsoCritico) ActualizarUso(int equipoId, int incremento)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@equipo_id", equipoId),
                _acceso.CrearParametro("@incremento", incremento)
            };
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("EQUIPO_ACTUALIZAR_USO", parametros);
                DataRow fila = tabla.Rows[0];
                int uso = Convert.ToInt32(fila["USO_ACUMULADO"]);
                int? nivel = fila["NIVEL_USO_CRITICO"] == DBNull.Value ? (int?)null : Convert.ToInt32(fila["NIVEL_USO_CRITICO"]);
                return (uso, nivel);
            }
            finally { _acceso.Cerrar(); }
        }

        public void ReiniciarContador(int equipoId)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@equipo_id", equipoId)
            };
            try
            {
                _acceso.Abrir();
                _acceso.Escribir("EQUIPO_REINICIAR_CONTADOR", parametros);
            }
            finally { _acceso.Cerrar(); }
        }

        public void MarcarEnMantenimiento(int equipoId)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@equipo_id", equipoId)
            };
            try
            {
                _acceso.Abrir();
                _acceso.Escribir("EQUIPO_MARCAR_EN_MANTENIMIENTO", parametros);
            }
            finally { _acceso.Cerrar(); }
        }

        private BE.Equipo Mapear(DataRow fila) => new BE.Equipo
        {
            Id = Convert.ToInt32(fila["ID"]),
            Nombre = fila["NOMBRE"].ToString(),
            UsoAcumulado = Convert.ToInt32(fila["USO_ACUMULADO"]),
            NivelUsoCritico = fila["NIVEL_USO_CRITICO"] == DBNull.Value ? (int?)null : Convert.ToInt32(fila["NIVEL_USO_CRITICO"]),
            Estado = fila["ESTADO"].ToString(),
            TecnicoHabitualId = fila["TECNICO_HABITUAL_ID"] == DBNull.Value ? (int?)null : Convert.ToInt32(fila["TECNICO_HABITUAL_ID"])
        };

        public BE.Equipo ObtenerPorNombre(string nombre)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
    {
        _acceso.CrearParametro("@nombre", nombre)
    };
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("EQUIPO_OBTENER_POR_NOMBRE", parametros);
                return tabla.Rows.Count == 0 ? null : Mapear(tabla.Rows[0]);
            }
            finally { _acceso.Cerrar(); }
        }
        public void RegistrarUsoLog(int equipoId, int incremento)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
    {
        _acceso.CrearParametro("@equipo_id", equipoId),
        _acceso.CrearParametro("@incremento", incremento)
    };
            try
            {
                _acceso.Abrir();
                _acceso.Escribir("EQUIPO_USO_LOG_INSERTAR", parametros);
            }
            finally { _acceso.Cerrar(); }
        }

        public (int total, DateTime? desde) ObtenerUsoUltimosDias(int equipoId, int dias)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
    {
        _acceso.CrearParametro("@equipo_id", equipoId),
        _acceso.CrearParametro("@dias", dias)
    };
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("EQUIPO_USO_LOG_ULTIMOS_DIAS", parametros);
                int total = Convert.ToInt32(tabla.Rows[0]["TOTAL"]);
                DateTime? desde = tabla.Rows[0]["DESDE"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(tabla.Rows[0]["DESDE"]);
                return (total, desde);
            }
            finally { _acceso.Cerrar(); }
        }
    }
}