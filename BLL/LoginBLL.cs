using BE;
using Seguridad;
using Servicio;

namespace BLL
{
    public class LoginBLL
    {
        private readonly DAL.UsuarioDAL _dal = new DAL.UsuarioDAL();
        private readonly BitacoraBLL _bitacora = new BitacoraBLL();

        public LoginResultado AutenticarUsuario(string usuario, string contrasena)
        {
            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(contrasena))
                return LoginResultado.CredencialesInvalidas;

            if (_dal.EstaBloqueado(usuario))
                return LoginResultado.UsuarioBloqueado;

            string contrasenHash = Hasher.Hashear(contrasena);
            BE.USUARIO u = _dal.ObtenerPorCredenciales(usuario, contrasenHash);

            if (u != null)
            {
                _dal.ResetearIntentos(usuario);
                SessionManager.getInstance().setUsuario(u);
                _bitacora.RegistrarLogin(usuario);
                return LoginResultado.Exito;
            }

            bool quedoBloqueado = _dal.IncrementarIntentos(usuario);
            _bitacora.RegistrarAccion(usuario, "LOGIN_FALLIDO");

            if (quedoBloqueado)
            {
                _bitacora.RegistrarAccion(usuario, "USUARIO_BLOQUEADO");
                return LoginResultado.UsuarioBloqueado;
            }

            return LoginResultado.CredencialesInvalidas;
        }
    }
}
