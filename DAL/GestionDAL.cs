using BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class GestionDAL
    {
        private readonly Acceso _acceso = new Acceso();

        public DatosNegocio ContarDatosNegocio()
        {
            try
            {
                _acceso.Abrir();
                DataRow fila = _acceso.Leer("DATOS_NEGOCIO_CONTAR").Rows[0];
                return new DatosNegocio
                {
                    Franjas = Convert.ToInt32(fila["FRANJAS"]),
                    Equipos = Convert.ToInt32(fila["EQUIPOS"]),
                    Grillas = Convert.ToInt32(fila["GRILLAS"]),
                    Usuarios = Convert.ToInt32(fila["USUARIOS"])
                };
            }
            finally { _acceso.Cerrar(); }
        }

        public int InsertarEquipo(string nombre, int usoAcumulado, int? nivelCritico)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@nombre", nombre),
                _acceso.CrearParametro("@uso_acumulado", usoAcumulado),
                new SqlParameter("@nivel_uso_critico", SqlDbType.Int) { Value = (object)nivelCritico ?? DBNull.Value }
            };
            try
            {
                _acceso.Abrir();
                return Convert.ToInt32(_acceso.Leer("EQUIPO_INSERTAR", parametros).Rows[0]["ID"]);
            }
            finally { _acceso.Cerrar(); }
        }

        public void InsertarUsoConFecha(int equipoId, DateTime fecha, int incremento)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@equipo_id", equipoId),
                new SqlParameter("@fecha", SqlDbType.DateTime) { Value = fecha },
                _acceso.CrearParametro("@incremento", incremento)
            };
            try
            {
                _acceso.Abrir();
                _acceso.Escribir("EQUIPO_USO_LOG_INSERTAR_FECHA", parametros);
            }
            finally { _acceso.Cerrar(); }
        }

        public List<EvaluacionFranjaSemana> ListarEvaluaciones(DateTime desde, DateTime hasta)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                new SqlParameter("@desde", desde.Date),
                new SqlParameter("@hasta", hasta.Date)
            };
            var lista = new List<EvaluacionFranjaSemana>();
            try
            {
                _acceso.Abrir();
                foreach (DataRow fila in _acceso.Leer("EVALUACION_COBERTURA_LISTAR_RANGO", parametros).Rows)
                {
                    lista.Add(new EvaluacionFranjaSemana
                    {
                        Semana = Convert.ToDateTime(fila["SEMANA"]),
                        FranjaId = Convert.ToInt32(fila["FRANJA_ID"]),
                        Dia = fila["DIA"].ToString(),
                        HoraInicio = (TimeSpan)fila["HORA_INICIO"],
                        HoraFin = (TimeSpan)fila["HORA_FIN"],
                        Rol = fila["ROL_REQUERIDO"].ToString(),
                        Resultado = fila["RESULTADO"].ToString()
                    });
                }
            }
            finally { _acceso.Cerrar(); }
            return lista;
        }

        public List<SociosEnFecha> ListarSocios(DateTime desde, DateTime hasta)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                new SqlParameter("@desde", desde.Date),
                new SqlParameter("@hasta", hasta.Date)
            };
            var lista = new List<SociosEnFecha>();
            try
            {
                _acceso.Abrir();
                foreach (DataRow fila in _acceso.Leer("HISTORIAL_ASISTENCIA_LISTAR_RANGO", parametros).Rows)
                {
                    lista.Add(new SociosEnFecha
                    {
                        FranjaId = Convert.ToInt32(fila["FRANJA_ID"]),
                        Fecha = Convert.ToDateTime(fila["FECHA"]),
                        Cantidad = Convert.ToInt32(fila["CANTIDAD_SOCIOS"])
                    });
                }
            }
            finally { _acceso.Cerrar(); }
            return lista;
        }
    }
}
