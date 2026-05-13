using BE;

namespace Servicio
{
    public class SessionManager
    {
        private static SessionManager _instance = null;
        private USUARIO _usuario;

        private SessionManager() { }

        public static SessionManager getInstance()
        {
            if (_instance == null)
            {
                _instance = new SessionManager();
            }
            return _instance;
        }

        public USUARIO getUsuario()
        {
            return _usuario;
        }

        public void setUsuario(USUARIO usuario)
        {
            _usuario = usuario;
        }

        public void cerrarSesion()
        {
            _usuario = null;
            _instance = null;
        }

        public bool EsAdmin()
        {
            return _usuario != null && _usuario.Rol == "admin";
        }
    }
}
