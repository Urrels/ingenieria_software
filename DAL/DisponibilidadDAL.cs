using BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class DisponibilidadDAL
    {
        private readonly Acceso _acceso = new Acceso();

        public BE.Disponibilidad ObtenerPorUsuarioYSemana(int usuarioId, DateTime semana)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@usuario_id", usuarioId),
                new SqlParameter("@semana", semana)
            };
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("DISPONIBILIDAD_OBTENER_POR_USUARIO_SEMANA", parametros);
                if (tabla.Rows.Count == 0) return null;

                DataRow fila = tabla.Rows[0];
                var disp = new BE.Disponibilidad
                {
                    Id = Convert.ToInt32(fila["ID"]),
                    UsuarioId = Convert.ToInt32(fila["USUARIO_ID"]),
                    Semana = Convert.ToDateTime(fila["SEMANA"])
                };
                disp.Franjas = ObtenerFranjas(disp.Id);
                return disp;
            }
            finally { _acceso.Cerrar(); }
        }

        private List<BE.FranjaHoraria> ObtenerFranjas(int disponibilidadId)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@disponibilidad_id", disponibilidadId)
            };
            DataTable tabla = _acceso.Leer("DISPONIBILIDAD_FRANJAS_LISTAR", parametros);

            var lista = new List<BE.FranjaHoraria>();
            foreach (DataRow fila in tabla.Rows)
            {
                lista.Add(new BE.FranjaHoraria
                {
                    Id = Convert.ToInt32(fila["ID"]),
                    Dia = fila["DIA"].ToString(),
                    HoraInicio = (TimeSpan)fila["HORA_INICIO"],
                    HoraFin = (TimeSpan)fila["HORA_FIN"]
                });
            }
            return lista;
        }

        public int Guardar(BE.Disponibilidad disponibilidad)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@usuario_id", disponibilidad.UsuarioId),
                new SqlParameter("@semana", disponibilidad.Semana)
            };
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("DISPONIBILIDAD_INSERTAR", parametros);
                int nuevoId = Convert.ToInt32(tabla.Rows[0]["ID"]);

                foreach (var franja in disponibilidad.Franjas)
                {
                    List<SqlParameter> parametrosRel = new List<SqlParameter>
                    {
                        _acceso.CrearParametro("@disponibilidad_id", nuevoId),
                        _acceso.CrearParametro("@franja_id", franja.Id)
                    };
                    _acceso.Escribir("DISPONIBILIDAD_FRANJA_INSERTAR", parametrosRel);
                }
                return nuevoId;
            }
            finally { _acceso.Cerrar(); }
        }

        public void Actualizar(BE.Disponibilidad disponibilidad)
        {
            List<SqlParameter> parametrosBorrado = new List<SqlParameter>
    {
        _acceso.CrearParametro("@disponibilidad_id", disponibilidad.Id)
    };
            try
            {
                _acceso.Abrir();
                _acceso.Escribir("DISPONIBILIDAD_FRANJA_ELIMINAR_POR_DISPONIBILIDAD", parametrosBorrado);

                foreach (var franja in disponibilidad.Franjas)
                {
                    List<SqlParameter> parametrosRel = new List<SqlParameter>
            {
                _acceso.CrearParametro("@disponibilidad_id", disponibilidad.Id),
                _acceso.CrearParametro("@franja_id", franja.Id)
            };
                    _acceso.Escribir("DISPONIBILIDAD_FRANJA_INSERTAR", parametrosRel);
                }
            }
            finally { _acceso.Cerrar(); }
        }
    }
}