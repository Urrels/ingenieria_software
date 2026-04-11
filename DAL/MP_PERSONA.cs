using BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class MP_PERSONA : MAPPER<BE.PERSONA>
    {
        public override int Borrar(PERSONA objeto)
        {
            acceso = new Acceso();
            acceso.Abrir();
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(acceso.CrearParametro("@ID", objeto.Id));
            int res = acceso.Escribir("PERSONA_BORRAR",parametros);
            acceso.Cerrar();
            return res;
        }

        public override int Editar(PERSONA objeto)
        {
            acceso = new Acceso();
            acceso.Abrir();
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(acceso.CrearParametro("@ID", objeto.Id));
            parametros.Add(acceso.CrearParametro("@nom", objeto.Nombre));
            parametros.Add(acceso.CrearParametro("@ape", objeto.Apellido));
            int res = acceso.Escribir("PERSONA_EDITAR", parametros);
            acceso.Cerrar();
            return res;
        }

        public override int Insertar(PERSONA objeto)
        {
            acceso = new Acceso();
            acceso.Abrir();
            List<SqlParameter> parametros = new List<SqlParameter>();         
            parametros.Add(acceso.CrearParametro("@nom", objeto.Nombre));
            parametros.Add(acceso.CrearParametro("@ape", objeto.Apellido));
            int res = acceso.Escribir("PERSONA_INSERTAR", parametros);
            acceso.Cerrar();
            return res;
        }

        public override List<PERSONA> Listar()
        {
            List<PERSONA> personas = new List<PERSONA>();
            acceso = new Acceso();
            acceso.Abrir();
            DataTable tabla = acceso.Leer("PERSONA_LISTAR");
            acceso.Cerrar();

            foreach (DataRow registro in tabla.Rows)
            {
                PERSONA p = new PERSONA();
                p.Id = int.Parse(registro["ID"].ToString());
                p.Nombre = registro["NOMBRE"].ToString();
                p.Apellido = registro["APELLIDO"].ToString();
                personas.Add(p);
            }

            return personas;
        }
    }
}
