using BE;

namespace BLL
{
    public class USUARIO
    {
        public bool Login(string usuario, string contrasena)
        {
            DAL.MP_USUARIO mp = new DAL.MP_USUARIO();
            BE.USUARIO u = mp.Login(usuario, contrasena);

            if (u != null)
            {
                SessionManager.getInstane().setUsuario(u);
                return true;
            }
            return false;
        }

        public void Logout()
        {
            SessionManager.getInstane().cerrarSesion();
        }
    }
}