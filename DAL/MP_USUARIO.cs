using BE;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class MP_USUARIO : MAPPER<BE.USUARIO>
    {
        public override int Insertar(USUARIO objeto) { return 0; }
        public override int Editar(USUARIO objeto) { return 0; }
        public override int Borrar(USUARIO objeto) { return 0; }
        public override List<USUARIO> Listar() { return null; }

        public BE.USUARIO Login(string usuario, string contrasena)
        {
            acceso = new Acceso();
            acceso.Abrir();

            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(acceso.CrearParametro("@usuario", usuario));
            parametros.Add(acceso.CrearParametro("@pass", contrasena));

            DataTable tabla = acceso.Leer("USUARIO_LOGIN", parametros);
            acceso.Cerrar();

            if (tabla.Rows.Count > 0)
            {
                BE.USUARIO u = new BE.USUARIO();
                u.Id = int.Parse(tabla.Rows[0]["ID"].ToString());
                u.Usuario = tabla.Rows[0]["USUARIO"].ToString();
                return u;
            }

            return null;
        }
    }
}