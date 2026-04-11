using BE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class PERSONA
    {

        public void Grabar(BE.PERSONA persona)
        {
            DAL.MP_PERSONA mp = new DAL.MP_PERSONA();
            if (persona.Id == 0)
            {
                mp.Insertar(persona);
            }
            else
            { 
                mp.Editar(persona);
            }        
        }

        public void Borrar(BE.PERSONA persona)
        {
            DAL.MP_PERSONA mp = new DAL.MP_PERSONA();            
            mp.Borrar(persona);
            
        }

        public List<BE.PERSONA> Listar()
        {
            DAL.MP_PERSONA mp = new DAL.MP_PERSONA();
            return mp.Listar();
        }

    }
}
