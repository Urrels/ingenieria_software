using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class GrupoUsuarios : UsuarioComponente
    {
        private readonly List<UsuarioComponente> _hijos = new List<UsuarioComponente>();

        public override bool Validar()
        {
            return _hijos.All(h => h.Validar());
        }

        public override void Agregar(UsuarioComponente componente)
        {
            _hijos.Add(componente);
        }

        public override void Eliminar(UsuarioComponente componente)
        {
            _hijos.Remove(componente);
        }

        public override UsuarioComponente ObtenerHijo(int indice)
        {
            return _hijos[indice];
        }

        public IEnumerable<UsuarioComponente> ObtenerTodos()
        {
            return _hijos;
        }
    }
}