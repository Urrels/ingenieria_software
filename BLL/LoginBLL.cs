using BE;

namespace BLL
{
    public class LoginBLL
    {
        private readonly DAL.UsuarioDAL _dal = new DAL.UsuarioDAL();
        private readonly BitacoraBLL _bitacora = new BitacoraBLL();

        public bool AutenticarUsuario(string usuario, string contrasena)
        {
            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(contrasena))
                return false;

            string contrasenHash = HashHelper.HashSHA256(contrasena);
            BE.USUARIO u = _dal.ObtenerPorCredenciales(usuario, contrasenHash);

            if (u != null)
            {
                BE.SessionManager.getInstane().setUsuario(u);
                _bitacora.RegistrarLogin(usuario); 
                return true;
            }

            return false;
        }
    }
}