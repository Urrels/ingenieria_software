using BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class AlertaRevisionDAL
    {
        private readonly Acceso _acceso = new Acceso();

        public int Insertar(int equipoId)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@equipo_id", equipoId)
            };
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("ALERTA_INSERTAR", parametros);
                return Convert.ToInt32(tabla.Rows[0]["ID"]);
            }
            finally { _acceso.Cerrar(); }
        }

        public BE.AlertaRevision Obtener(int id)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@id", id)
            };
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("ALERTA_OBTENER", parametros);
                if (tabla.Rows.Count == 0) return null;
                return MapearBasico(tabla.Rows[0]);
            }
            finally { _acceso.Cerrar(); }
        }

        public List<BE.AlertaRevision> ListarPendientes()
        {
            var lista = new List<BE.AlertaRevision>();
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("ALERTA_LISTAR_PENDIENTES", null);
                foreach (DataRow fila in tabla.Rows)
                {
                    var alerta = MapearBasico(fila);
                    alerta.EquipoNombre = fila["EQUIPO_NOMBRE"].ToString();
                    lista.Add(alerta);
                }
            }
            finally { _acceso.Cerrar(); }
            return lista;
        }

        public void ActualizarEstado(int alertaId, string estado)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@alerta_id", alertaId),
                _acceso.CrearParametro("@estado", estado)
            };
            try
            {
                _acceso.Abrir();
                _acceso.Escribir("ALERTA_ACTUALIZAR_ESTADO", parametros);
            }
            finally { _acceso.Cerrar(); }
        }

        private BE.AlertaRevision MapearBasico(DataRow fila) => new BE.AlertaRevision
        {
            Id = Convert.ToInt32(fila["ID"]),
            EquipoId = Convert.ToInt32(fila["EQUIPO_ID"]),
            FechaGeneracion = Convert.ToDateTime(fila["FECHA_GENERACION"]),
            Estado = fila["ESTADO"].ToString()
        };

        public List<BE.AlertaRevision> ListarAutorizadas()
        {
            var lista = new List<BE.AlertaRevision>();
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("ALERTA_LISTAR_AUTORIZADAS", null);
                foreach (DataRow fila in tabla.Rows)
                {
                    var alerta = MapearBasico(fila);
                    alerta.EquipoNombre = fila["EQUIPO_NOMBRE"].ToString();
                    lista.Add(alerta);
                }
            }
            finally { _acceso.Cerrar(); }
            return lista;
        }
    }

}