using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class SistemaDAL
    {
        private static readonly string[] TablasCriticas = {
            "USUARIO", "NODO_PERMISO", "ROL_PERMISO", "USUARIO_PERFIL",
            "BITACORA", "IDIOMA", "CONTROL_IDIOMA", "TRADUCCION",
            "USUARIO_HISTORIAL", "DIGITO_VERIFICADOR_VERTICAL"
        };

        public List<string> ObtenerTablasFaltantes()
        {
            List<string> faltantes = new List<string>();
            Acceso db = new Acceso();
            try
            {
                db.Abrir();
                foreach (string tabla in TablasCriticas)
                {
                    DataTable dt = db.LeerTexto(
                        $"SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES " +
                        $"WHERE TABLE_NAME = '{tabla}'");
                    if (dt.Rows.Count == 0)
                        faltantes.Add(tabla);
                }
            }
            finally { db.Cerrar(); }
            return faltantes;
        }

        public bool ExisteAdmin()
        {
            Acceso db = new Acceso();
            try
            {
                db.Abrir();
                DataTable dt = db.LeerTexto(
                    "SELECT COUNT(*) AS C FROM USUARIO WHERE ROL = 'admin'");
                return System.Convert.ToInt32(dt.Rows[0]["C"]) > 0;
            }
            catch { return false; }
            finally { db.Cerrar(); }
        }
    }
}