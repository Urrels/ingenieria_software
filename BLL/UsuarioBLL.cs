using BE;
using Seguridad;
using Servicio;

namespace BLL
{
    public class UsuarioBLL
    {
        private readonly DAL.UsuarioDAL _dal = new DAL.UsuarioDAL();
        private readonly BitacoraBLL _bitacora = new BitacoraBLL();

        public bool VerificarContrasena(string usuario, string contrasena)
        {
            string hash = Hasher.Hashear(contrasena);
            BE.USUARIO u = _dal.ObtenerPorCredenciales(usuario, hash);
            return u != null;
        }

        public bool CambiarContrasena(string usuario, string nuevaContrasena)
        {
            string hash = Hasher.Hashear(nuevaContrasena);
            bool resultado = _dal.CambiarContrasena(usuario, hash);

            if (resultado)
                _bitacora.RegistrarAccion(usuario, "CAMBIO_CONTRASENA");

            return resultado;
        }

        public System.Collections.Generic.List<BE.USUARIO> ListarBloqueados()
        {
            return _dal.ListarBloqueados();
        }

        public void Desbloquear(string usuario)
        {
            _dal.Desbloquear(usuario);
            string admin = SessionManager.getInstance().getUsuario().Usuario;
            _bitacora.RegistrarAccion(admin, "DESBLOQUEO_USUARIO:" + usuario);
        }
    }
}
