using BE;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class LoginBLL
    {
        private readonly UsuarioDAL _dal = new UsuarioDAL();

        public UsuarioComponente AutenticarUsuario(string nombre, string contrasena)
        {
            var usuarioIngresado = new Usuario
            {
                Nombre = nombre,
                Contrasena = contrasena
            };

            if (!usuarioIngresado.Validar())
                return null;

            return _dal.ObtenerPorCredenciales(nombre, contrasena);
        }
    }
}