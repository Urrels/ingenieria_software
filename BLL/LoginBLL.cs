using BE;

namespace BLL
{
    public class LoginBLL
    {
        private readonly DAL.UsuarioDAL _dal = new DAL.UsuarioDAL();

        public bool AutenticarUsuario(string usuario, string contrasena)
        {
            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(contrasena))
                return false;

            // ← Hashea la contraseña antes de comparar
            string contrasenHash = HashHelper.HashSHA256(contrasena);

            BE.USUARIO u = _dal.ObtenerPorCredenciales(usuario, contrasenHash);

            if (u != null)
            {
                BE.SessionManager.getInstane().setUsuario(u);
                return true;
            }

            return false;
        }
    }
}