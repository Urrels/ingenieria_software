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
                        Rol = fila["ROL"].ToString()
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

        public List<BE.USUARIO> ListarBloqueados()
        {
            List<BE.USUARIO> lista = new List<BE.USUARIO>();
            try
            {
                _acceso.Abrir();
                DataTable tabla = _acceso.Leer("USUARIO_LISTAR_BLOQUEADOS");
                foreach (DataRow fila in tabla.Rows)
                {
                    lista.Add(new BE.USUARIO
                    {
                        Id = Convert.ToInt32(fila["ID"]),
                        Usuario = fila["USUARIO"].ToString(),
                        Rol = fila["ROL"].ToString(),
                        IntentosFallidos = Convert.ToInt32(fila["INTENTOS_FALLIDOS"]),
                        Bloqueado = true
                    });
                }
            }
            finally
            {
                _acceso.Cerrar();
            }
            return lista;
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
                        Id       = Convert.ToInt32(fila["ID"]),
                        Usuario  = fila["USUARIO"].ToString(),
                        Rol      = fila["ROL"].ToString(),
                        Bloqueado = Convert.ToBoolean(fila["BLOQUEADO"])
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

        public bool Crear(string usuario, string passHash, string rol)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@usuario", usuario),
                _acceso.CrearParametro("@pass",    passHash),
                _acceso.CrearParametro("@rol",     rol)
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
                    Id                = Convert.ToInt32(fila["ID"]),
                    Usuario           = fila["USUARIO"].ToString(),
                    Rol               = fila["ROL"].ToString(),
                    IntentosFallidos  = Convert.ToInt32(fila["INTENTOS_FALLIDOS"]),
                    Bloqueado         = Convert.ToBoolean(fila["BLOQUEADO"])
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

        public void AplicarEstado(int id, string rol, bool bloqueado, int intentosFallidos)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _acceso.CrearParametro("@id",                id),
                _acceso.CrearParametro("@rol",               rol),
                _acceso.CrearParametro("@bloqueado",         bloqueado ? 1 : 0),
                _acceso.CrearParametro("@intentos_fallidos", intentosFallidos)
            };
            try
            {
                _acceso.Abrir();
                _acceso.Escribir("USUARIO_APLICAR_ESTADO", parametros);
            }
            finally { _acceso.Cerrar(); }
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