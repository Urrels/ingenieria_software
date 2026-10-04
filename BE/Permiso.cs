using System;
using System.Collections.Generic;

namespace BE
{
    public class Permiso : NodoPermiso
    {
        public override void Agregar(NodoPermiso nodo)
        {
            throw new InvalidOperationException("Un permiso no puede contener hijos.");
        }

        public override void Quitar(NodoPermiso nodo)
        {
            throw new InvalidOperationException("Un permiso no puede contener hijos.");
        }

        public override IList<NodoPermiso> ObtenerHijos()
        {
            return new List<NodoPermiso>();
        }

        public override bool EsHoja()
        {
            return true;
        }

        public override NodoPermiso Clonar()
        {
            return new Permiso { Id = Id, Nombre = Nombre, PadreId = PadreId };
        }
    }
}
