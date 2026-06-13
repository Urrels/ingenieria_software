using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class UsuarioHistorialDAL
    {
        private readonly Acceso _acceso = new Acceso();

        public void Insertar(BE.UsuarioHistorial h)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@usuario_id",        h.UsuarioId),
                _acceso.CrearParametro("@usuario_login",     h.UsuarioLogin),
                _acceso.CrearParametro("@rol",               h.Rol),
                _acceso.CrearParametro("@bloqueado",         h.Bloqueado ? 1 : 0),
                _acceso.CrearParametro("@intentos_fallidos", h.IntentosFallidos),
                _acceso.CrearParametro("@perfiles",          h.Perfiles ?? ""),
                _acceso.CrearParametro("@nombre",            h.Nombre ?? ""),
                _acceso.CrearParametro("@apellido",          h.Apellido ?? ""),
                _acceso.CrearParametro("@realizado_por",     h.RealizadoPor),
                _acceso.CrearParametro("@tipo_cambio",       h.TipoCambio)
            };

            SqlParameter pOrigen = new SqlParameter("@version_origen",
                (object)h.VersionOrigen ?? DBNull.Value);
            pOrigen.SqlDbType = SqlDbType.Int;
            parametros.Add(pOrigen);

            try
            {
                _acceso.Abrir();
                _acceso.Escribir("USUARIO_HISTORIAL_INSERTAR", parametros);
            }
            finally { _acceso.Cerrar(); }
        }

        public List<BE.UsuarioHistorial> ListarPorUsuario(int usuarioId)
        {
            List<BE.UsuarioHistorial> lista = new List<BE.UsuarioHistorial>();
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@usuario_id", usuarioId)
            };
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("USUARIO_HISTORIAL_LISTAR", parametros);
                foreach (DataRow fila in tabla.Rows)
                    lista.Add(MapearFila(fila));
            }
            finally { _acceso.Cerrar(); }
            return lista;
        }

        public BE.UsuarioHistorial ObtenerPorId(int id)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@id", id)
            };
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("USUARIO_HISTORIAL_OBTENER", parametros);
                return tabla.Rows.Count > 0 ? MapearFila(tabla.Rows[0]) : null;
            }
            finally { _acceso.Cerrar(); }
        }

        private BE.UsuarioHistorial MapearFila(DataRow fila)
        {
            return new BE.UsuarioHistorial
            {
                Id               = Convert.ToInt32(fila["ID"]),
                UsuarioId        = Convert.ToInt32(fila["USUARIO_ID"]),
                UsuarioLogin     = fila["USUARIO_LOGIN"].ToString(),
                Rol              = fila["ROL"].ToString(),
                Bloqueado        = Convert.ToBoolean(fila["BLOQUEADO"]),
                IntentosFallidos = Convert.ToInt32(fila["INTENTOS_FALLIDOS"]),
                Perfiles         = fila["PERFILES"].ToString(),
                Nombre           = fila["NOMBRE"].ToString(),
                Apellido         = fila["APELLIDO"].ToString(),
                FechaCambio      = Convert.ToDateTime(fila["FECHA_CAMBIO"]),
                RealizadoPor     = fila["REALIZADO_POR"].ToString(),
                TipoCambio       = fila["TIPO_CAMBIO"].ToString(),
                VersionOrigen    = fila["VERSION_ORIGEN"] == DBNull.Value
                                   ? (int?)null
                                   : Convert.ToInt32(fila["VERSION_ORIGEN"])
            };
        }
    }
}
