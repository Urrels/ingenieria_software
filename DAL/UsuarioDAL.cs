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