using BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class GrillaDAL
    {
        private readonly Acceso _acceso = new Acceso();

        public int Crear(BE.GrillaDeTurnos grilla)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@administrador_id", grilla.AdministradorId),
                new SqlParameter("@semana", grilla.Semana),
                _acceso.CrearParametro("@estado", grilla.Estado)
            };
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("GRILLA_INSERTAR", parametros);
                return Convert.ToInt32(tabla.Rows[0]["ID"]);
            }
            finally { _acceso.Cerrar(); }
        }

        public void Confirmar(int grillaId)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@grilla_id", grillaId)
            };
            try
            {
                _acceso.Abrir();
                _acceso.Escribir("GRILLA_CONFIRMAR", parametros);
            }
            finally { _acceso.Cerrar(); }
        }

        public void MarcarComunicada(int grillaId)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@grilla_id", grillaId)
            };
            try
            {
                _acceso.Abrir();
                _acceso.Escribir("GRILLA_MARCAR_COMUNICADA", parametros);
            }
            finally { _acceso.Cerrar(); }
        }

        // Agrupa los turnos asignados de la grilla por empleado (para el envío de notificaciones, UC4)
        public Dictionary<int, List<BE.Turno>> ObtenerTurnosPorEmpleado(int grillaId)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@grilla_id", grillaId)
            };
            var resultado = new Dictionary<int, List<BE.Turno>>();
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("GRILLA_TURNOS_POR_EMPLEADO", parametros);
                foreach (DataRow fila in tabla.Rows)
                {
                    int usuarioId = Convert.ToInt32(fila["USUARIO_ID"]);
                    var turno = new BE.Turno
                    {
                        Id = Convert.ToInt32(fila["ID"]),
                        GrillaId = grillaId,
                        FranjaId = Convert.ToInt32(fila["FRANJA_ID"]),
                        UsuarioId = usuarioId,
                        RolRequerido = fila["ROL_REQUERIDO"].ToString(),
                        Estado = fila["ESTADO"].ToString()
                    };
                    if (!resultado.ContainsKey(usuarioId))
                        resultado[usuarioId] = new List<BE.Turno>();
                    resultado[usuarioId].Add(turno);
                }
            }
            finally { _acceso.Cerrar(); }
            return resultado;
        }


        public BE.GrillaDeTurnos ObtenerPorSemana(DateTime semana)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                new SqlParameter("@semana", semana)
            };
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("GRILLA_OBTENER_POR_SEMANA", parametros);
                if (tabla.Rows.Count == 0) return null;

                DataRow fila = tabla.Rows[0];
                return new BE.GrillaDeTurnos
                {
                    Id = Convert.ToInt32(fila["ID"]),
                    AdministradorId = Convert.ToInt32(fila["ADMINISTRADOR_ID"]),
                    Semana = Convert.ToDateTime(fila["SEMANA"]),
                    Estado = fila["ESTADO"].ToString()
                };
            }
            finally { _acceso.Cerrar(); }
        }
    }
}