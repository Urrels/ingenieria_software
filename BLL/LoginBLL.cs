using BE;
using SeguridadYServicios;
using System.Collections.Generic;

namespace BLL
{
    public class LoginBLL
    {
        // Un administrador bloqueado se libera solo: si fuera el único, nadie podría desbloquearlo.
        private const int MinutosBloqueoAdministrador = 15;

        private readonly DAL.UsuarioDAL      _dal       = new DAL.UsuarioDAL();
        private readonly BitacoraBLL         _bitacora  = new BitacoraBLL();
        private readonly UsuarioPerfilBLL    _perfilBll = new UsuarioPerfilBLL();
        private readonly UsuarioHistorialBLL _historial = new UsuarioHistorialBLL();

        public LoginResultado AutenticarUsuario(string usuario, string contrasena)
        {
            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(contrasena))
                return LoginResultado.CredencialesInvalidas;

            if (_dal.DesbloquearSiExpiro(usuario, MinutosBloqueoAdministrador))
            {
                _bitacora.RegistrarAccion(usuario, "DESBLOQUEO_AUTOMATICO");
                int? idLiberado = _dal.ObtenerIdPorNombre(usuario);
                if (idLiberado.HasValue) _historial.RegistrarCambio(idLiberado.Value, "DESBLOQUEO", "sistema");
            }

            if (_dal.EstaBloqueado(usuario))
                return LoginResultado.UsuarioBloqueado;

            string contrasenHash = Hasher.Hashear(contrasena);
            BE.USUARIO u = _dal.ObtenerPorCredenciales(usuario, contrasenHash);

            if (u != null)
            {
                _dal.ResetearIntentos(usuario);
                SessionManager.getInstance().setUsuario(u);
                List<string> permisos = _perfilBll.ObtenerPermisos(u.Id);
                SessionManager.getInstance().setPermisos(permisos);
                _bitacora.RegistrarLogin(usuario);

                new FachadaIdioma().AplicarPreferencia(u);

                return LoginResultado.Exito;
            }

            bool quedoBloqueado = _dal.IncrementarIntentos(usuario);
            _bitacora.RegistrarAccion(usuario, "LOGIN_FALLIDO");

            if (quedoBloqueado)
            {
                _bitacora.RegistrarAccion(usuario, "USUARIO_BLOQUEADO");
                int? id = _dal.ObtenerIdPorNombre(usuario);
                if (id.HasValue) _historial.RegistrarCambio(id.Value, "BLOQUEO", "sistema");
                return LoginResultado.UsuarioBloqueado;
            }

            return LoginResultado.CredencialesInvalidas;
        }
    }
}
