using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public abstract class UsuarioComponente : PERSONA
    {
        public string Contrasena { get; set; }

        public abstract bool Validar();
        public abstract void Agregar(UsuarioComponente componente);
        public abstract void Eliminar(UsuarioComponente componente);
        public abstract UsuarioComponente ObtenerHijo(int indice);
    }
}