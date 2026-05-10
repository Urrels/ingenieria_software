using System.Security.Cryptography;
using System.Text;

namespace BE
{
    public class SessionManager
    {
        private static SessionManager _instance = null;
        private USUARIO _usuario;

        private SessionManager() { }

        public static string Hashear(string texto)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(texto));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in bytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

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