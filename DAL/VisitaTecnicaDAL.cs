using BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class VisitaTecnicaDAL
    {
        private readonly Acceso _acceso = new Acceso();

        public int Insertar(int alertaId, int tecnicoId, DateTime fechaCoordinada)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@alerta_id", alertaId),
                _acceso.CrearParametro("@tecnico_id", tecnicoId),
                new SqlParameter("@fecha_coordinada", fechaCoordinada)
            };
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("VISITA_INSERTAR", parametros);
                return Convert.ToInt32(tabla.Rows[0]["ID"]);
            }
            finally { _acceso.Cerrar(); }
        }

        public List<BE.USUARIO> ListarTecnicosAlternativos()
        {
            var lista = new List<BE.USUARIO>();
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("VISITA_TECNICOS_ALTERNATIVOS", null);
                foreach (DataRow fila in tabla.Rows)
                {
                    lista.Add(new BE.USUARIO
                    {
                        Id = Convert.ToInt32(fila["ID"]),
                        Usuario = fila["USUARIO"].ToString(),
                        Nombre = fila["NOMBRE"].ToString(),
                        Apellido = fila["APELLIDO"].ToString(),
                        Telefono = fila["TELEFONO"] == DBNull.Value ? null : fila["TELEFONO"].ToString(),
                        Email = fila["EMAIL"] == DBNull.Value ? null : fila["EMAIL"].ToString()
                    });
                }
            }
            finally { _acceso.Cerrar(); }
            return lista;
        }

        public List<BE.VisitaTecnica> ListarPendientesPorTecnico(int tecnicoId)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@tecnico_id", tecnicoId)
            };
            var lista = new List<BE.VisitaTecnica>();
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("VISITA_LISTAR_PENDIENTES_TECNICO", parametros);
                foreach (DataRow fila in tabla.Rows)
                {
                    lista.Add(new BE.VisitaTecnica
                    {
                        Id = Convert.ToInt32(fila["ID"]),
                        AlertaId = Convert.ToInt32(fila["ALERTA_ID"]),
                        TecnicoId = Convert.ToInt32(fila["TECNICO_ID"]),
                        FechaCoordinada = Convert.ToDateTime(fila["FECHA_COORDINADA"]),
                        Estado = fila["ESTADO"].ToString()
                    });
                }
            }
            finally { _acceso.Cerrar(); }
            return lista;
        }
    }
}