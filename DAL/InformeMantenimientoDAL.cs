using BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class InformeMantenimientoDAL
    {
        private readonly Acceso _acceso = new Acceso();

        public int Insertar(int visitaId, string resultado, bool pendienteRepuesto)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@visita_id", visitaId),
                _acceso.CrearParametro("@resultado", resultado),
                new SqlParameter("@pendiente_repuesto", pendienteRepuesto)
            };
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("INFORME_INSERTAR", parametros);
                return Convert.ToInt32(tabla.Rows[0]["ID"]);
            }
            finally { _acceso.Cerrar(); }
        }

        public BE.InformeMantenimiento Obtener(int id)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@id", id)
            };
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("INFORME_OBTENER", parametros);
                return tabla.Rows.Count == 0 ? null : Mapear(tabla.Rows[0]);
            }
            finally { _acceso.Cerrar(); }
        }

        public List<BE.InformeMantenimiento> ListarPendientesCierre()
        {
            var lista = new List<BE.InformeMantenimiento>();
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("INFORME_LISTAR_PENDIENTES_CIERRE", null);
                foreach (DataRow fila in tabla.Rows)
                    lista.Add(Mapear(fila));
            }
            finally { _acceso.Cerrar(); }
            return lista;
        }

        private BE.InformeMantenimiento Mapear(DataRow fila) => new BE.InformeMantenimiento
        {
            Id = Convert.ToInt32(fila["ID"]),
            VisitaId = Convert.ToInt32(fila["VISITA_ID"]),
            Resultado = fila["RESULTADO"].ToString(),
            PendienteRepuesto = Convert.ToBoolean(fila["PENDIENTE_REPUESTO"]),
            FechaEmision = Convert.ToDateTime(fila["FECHA_EMISION"]),
            AlertaId = fila.Table.Columns.Contains("ALERTA_ID") ? Convert.ToInt32(fila["ALERTA_ID"]) : 0,
            EquipoId = fila.Table.Columns.Contains("EQUIPO_ID") ? Convert.ToInt32(fila["EQUIPO_ID"]) : 0
        };

        public void MarcarCerrado(int id)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
    {
        _acceso.CrearParametro("@id", id)
    };
            try
            {
                _acceso.Abrir();
                _acceso.Escribir("INFORME_MARCAR_CERRADO", parametros);
            }
            finally { _acceso.Cerrar(); }
        }
    }
}