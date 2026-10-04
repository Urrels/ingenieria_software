using BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class DemandaEstimadaDAL
    {
        private readonly Acceso _acceso = new Acceso();

        public List<int> ObtenerHistorial(int franjaId, int semanas)
        {
            return new DAL.HistorialAsistenciaSociosDAL().ObtenerPorFranja(franjaId, semanas);
        }

        public BE.EvaluacionCobertura ObtenerEvaluacionCobertura(int franjaId, DateTime semana)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@franja_id", franjaId),
                new SqlParameter("@semana", semana)
            };
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("EVALUACION_COBERTURA_OBTENER", parametros);
                if (tabla.Rows.Count == 0) return null;

                DataRow fila = tabla.Rows[0];
                return new BE.EvaluacionCobertura
                {
                    Id = Convert.ToInt32(fila["ID"]),
                    FranjaId = Convert.ToInt32(fila["FRANJA_ID"]),
                    Semana = Convert.ToDateTime(fila["SEMANA"]),
                    Resultado = fila["RESULTADO"].ToString()
                };
            }
            finally { _acceso.Cerrar(); }
        }

        public int Insertar(BE.DemandaEstimada demanda)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@franja_id", demanda.FranjaId),
                new SqlParameter("@semana", demanda.Semana),
                _acceso.CrearParametro("@rol_requerido", demanda.RolRequerido),
                _acceso.CrearParametro("@cantidad_personal_necesario", demanda.CantidadPersonalNecesario),
                _acceso.CrearParametro("@origen_dato", demanda.OrigenDato)
            };
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("DEMANDA_ESTIMADA_INSERTAR", parametros);
                return Convert.ToInt32(tabla.Rows[0]["ID"]);
            }
            finally { _acceso.Cerrar(); }
        }

        public int InsertarEvaluacionCobertura(BE.EvaluacionCobertura evaluacion)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
    {
        _acceso.CrearParametro("@franja_id", evaluacion.FranjaId),
        new SqlParameter("@semana", evaluacion.Semana),
        _acceso.CrearParametro("@resultado", evaluacion.Resultado)
    };
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("EVALUACION_COBERTURA_INSERTAR", parametros);
                return Convert.ToInt32(tabla.Rows[0]["ID"]);
            }
            finally { _acceso.Cerrar(); }
        }

        public Dictionary<string, int> ObtenerResumenCobertura()
        {
            var resultado = new Dictionary<string, int>();
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("EVALUACION_COBERTURA_RESUMEN", null);
                foreach (DataRow fila in tabla.Rows)
                    resultado[fila["RESULTADO"].ToString()] = Convert.ToInt32(fila["CANTIDAD"]);
            }
            finally { _acceso.Cerrar(); }
            return resultado;
        }

    }
}