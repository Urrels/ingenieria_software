using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class Usuario : UsuarioComponente
    {
        public string Email { get; set; }
        public string Rol { get; set; }

        public override bool Validar()
        {
            return !string.IsNullOrWhiteSpace(Nombre)
                && !string.IsNullOrWhiteSpace(Contrasena)
                && Contrasena.Length >= 6;
        }

        public override void Agregar(UsuarioComponente componente)
        {
            throw new InvalidOperationException("Un Usuario hoja no tiene hijos.");
        }

        public override void Eliminar(UsuarioComponente componente)
        {
            throw new InvalidOperationException("Un Usuario hoja no tiene hijos.");
        }

        public override UsuarioComponente ObtenerHijo(int indice)
        {
            throw new InvalidOperationException("Un Usuario hoja no tiene hijos.");
        }
    }
}