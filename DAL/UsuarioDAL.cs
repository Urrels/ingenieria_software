using BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class UsuarioDAL
    {
        private readonly Acceso _acceso = new Acceso();

        public BE.USUARIO ObtenerPorCredenciales(string usuario, string contrasena)
        {
            BE.USUARIO u = null;
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@usuario", usuario),
                _acceso.CrearParametro("@pass",    contrasena)
            };
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("USUARIO_LOGIN", parametros);
                if (tabla.Rows.Count > 0)
                {
                    DataRow fila = tabla.Rows[0];
                    u = new BE.USUARIO
                    {
                        Id = Convert.ToInt32(fila["ID"]),
                        Usuario = fila["USUARIO"].ToString(),
                        Rol = fila["ROL"].ToString(),
                        RolId = fila["ROL_ID"] == DBNull.Value ? (int?)null : Convert.ToInt32(fila["ROL_ID"]),
                        IdiomaId = fila["IDIOMA_ID"] == DBNull.Value ? (int?)null : Convert.ToInt32(fila["IDIOMA_ID"])
                    };
                }
            }
            finally
            {
                _acceso.Cerrar();
            }
            return u;
        }


        public bool CambiarContrasena(string usuario, string nuevaPassHash)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@usuario", usuario),
                _acceso.CrearParametro("@pass",    nuevaPassHash)
            };
            try
            {
                _acceso.Abrir();
                int filas = _acceso.Escribir("USUARIO_CAMBIAR_PASS", parametros);
                return filas > 0;
            }
            finally
            {
                _acceso.Cerrar();
            }
        }

        public bool EstaBloqueado(string usuario)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@usuario", usuario)
            };
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("USUARIO_VERIFICAR_BLOQUEO", parametros);
                if (tabla.Rows.Count == 0) return false;
                return Convert.ToBoolean(tabla.Rows[0]["BLOQUEADO"]);
            }
            finally
            {
                _acceso.Cerrar();
            }
        }

        public bool IncrementarIntentos(string usuario)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@usuario", usuario)
            };
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("USUARIO_INCREMENTAR_INTENTOS", parametros);
                if (tabla.Rows.Count == 0) return false;
                return Convert.ToBoolean(tabla.Rows[0]["BLOQUEADO"]);
            }
            finally
            {
                _acceso.Cerrar();
            }
        }

        public void ResetearIntentos(string usuario)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@usuario", usuario)
            };
            try
            {
                _acceso.Abrir();
                _acceso.Escribir("USUARIO_RESETEAR_INTENTOS", parametros);
            }
            finally
            {
                _acceso.Cerrar();
            }
        }

        public List<BE.USUARIO> ListarTodos()
        {
            List<BE.USUARIO> lista = new List<BE.USUARIO>();
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("USUARIO_LISTAR_TODOS");
                foreach (DataRow fila in tabla.Rows)
                {
                    lista.Add(new BE.USUARIO
                    {
                        Id = Convert.ToInt32(fila["ID"]),
                        Usuario = fila["USUARIO"].ToString(),
                        Rol = fila["ROL"].ToString(),
                        RolId = fila["ROL_ID"] == DBNull.Value ? (int?)null : Convert.ToInt32(fila["ROL_ID"]),
                        RolNombre = fila["ROL_NOMBRE"] == DBNull.Value ? null : fila["ROL_NOMBRE"].ToString(),
                        Bloqueado = Convert.ToBoolean(fila["BLOQUEADO"]),
                        Nombre = fila["NOMBRE"].ToString(),
                        Apellido = fila["APELLIDO"].ToString(),
                        Telefono = fila["TELEFONO"] == DBNull.Value ? null : fila["TELEFONO"].ToString(),
                        Email = fila["EMAIL"] == DBNull.Value ? null : fila["EMAIL"].ToString()
                    });
                }
            }
            finally
            {
                _acceso.Cerrar();
            }
            return lista;
        }

        public void Eliminar(int id)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@id", id)
            };
            try
            {
                _acceso.Abrir();
                _acceso.Escribir("USUARIO_ELIMINAR", parametros);
            }
            finally
            {
                _acceso.Cerrar();
            }
        }

        public bool Crear(string usuario, string passHash, int rolId, string nombre, string apellido,
                           string telefono = null, string email = null)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@usuario",  usuario),
                _acceso.CrearParametro("@pass",     passHash),
                _acceso.CrearParametro("@rol_id",   rolId),
                _acceso.CrearParametro("@nombre",   nombre ?? ""),
                _acceso.CrearParametro("@apellido", apellido ?? ""),
                _acceso.CrearParametro("@telefono", telefono ?? ""),
                _acceso.CrearParametro("@email",    email ?? "")
            };
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("USUARIO_CREAR", parametros);
                return tabla.Rows.Count > 0 && Convert.ToInt32(tabla.Rows[0]["OK"]) == 1;
            }
            finally
            {
                _acceso.Cerrar();
            }
        }

        public BE.USUARIO ObtenerPorId(int id)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@id", id)
            };
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("USUARIO_OBTENER_POR_ID", parametros);
                if (tabla.Rows.Count == 0) return null;
                DataRow fila = tabla.Rows[0];
                return new BE.USUARIO
                {
                    Id = Convert.ToInt32(fila["ID"]),
                    Usuario = fila["USUARIO"].ToString(),
                    Rol = fila["ROL"].ToString(),
                    RolId = fila["ROL_ID"] == DBNull.Value ? (int?)null : Convert.ToInt32(fila["ROL_ID"]),
                    IntentosFallidos = Convert.ToInt32(fila["INTENTOS_FALLIDOS"]),
                    Bloqueado = Convert.ToBoolean(fila["BLOQUEADO"]),
                    Nombre = fila["NOMBRE"].ToString(),
                    Apellido = fila["APELLIDO"].ToString(),
                    Telefono = fila["TELEFONO"] == DBNull.Value ? null : fila["TELEFONO"].ToString(),
                    Email = fila["EMAIL"] == DBNull.Value ? null : fila["EMAIL"].ToString()
                };
            }
            finally { _acceso.Cerrar(); }
        }

        public int? ObtenerIdPorNombre(string usuario)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@usuario", usuario)
            };
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("USUARIO_OBTENER_ID_POR_NOMBRE", parametros);
                if (tabla.Rows.Count == 0) return null;
                return Convert.ToInt32(tabla.Rows[0]["ID"]);
            }
            finally { _acceso.Cerrar(); }
        }

        public void AplicarEstado(int id, string rol, bool bloqueado, int intentosFallidos, int? rolId = null)
        {
            SqlParameter paramRolId = new SqlParameter("@rol_id", DbType.Int32)
            {
                Value = rolId.HasValue ? (object)rolId.Value : DBNull.Value
            };
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@id",                id),
                _acceso.CrearParametro("@rol",               rol),
                _acceso.CrearParametro("@bloqueado",         bloqueado ? 1 : 0),
                _acceso.CrearParametro("@intentos_fallidos", intentosFallidos),
                paramRolId
            };
            try
            {
                _acceso.Abrir();
                _acceso.Escribir("USUARIO_APLICAR_ESTADO", parametros);
            }
            finally { _acceso.Cerrar(); }
        }

        public void ActualizarDatos(int id, string nombre, string apellido, string telefono = null, string email = null)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@id",       id),
                _acceso.CrearParametro("@nombre",   nombre ?? ""),
                _acceso.CrearParametro("@apellido", apellido ?? ""),
                _acceso.CrearParametro("@telefono", telefono ?? ""),
                _acceso.CrearParametro("@email",    email ?? "")
            };
            try
            {
                _acceso.Abrir();
                _acceso.Escribir("USUARIO_ACTUALIZAR_DATOS", parametros);
            }
            finally
            {
                _acceso.Cerrar();
            }
        }

        public void ActualizarIdioma(int id, int idiomaId)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@id",        id),
                _acceso.CrearParametro("@idioma_id", idiomaId)
            };
            try
            {
                _acceso.Abrir();
                _acceso.Escribir("USUARIO_ACTUALIZAR_IDIOMA", parametros);
            }
            finally
            {
                _acceso.Cerrar();
            }
        }

        public void Desbloquear(string usuario)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@usuario", usuario)
            };
            try
            {
                _acceso.Abrir();
                _acceso.Escribir("USUARIO_DESBLOQUEAR", parametros);
            }
            finally
            {
                _acceso.Cerrar();
            }
        }
    }
}