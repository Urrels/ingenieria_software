using BE;

namespace BLL
{
    public class UsuarioBLL
    {
        private readonly DAL.UsuarioDAL _dal = new DAL.UsuarioDAL();
        private readonly BitacoraBLL _bitacora = new BitacoraBLL();

        public bool VerificarContrasena(string usuario, string contrasena)
        {
            string hash = HashHelper.HashSHA256(contrasena);
            BE.USUARIO u = _dal.ObtenerPorCredenciales(usuario, hash);
            return u != null;
        }

        public bool CambiarContrasena(string usuario, string nuevaContrasena)
        {
            string hash = HashHelper.HashSHA256(nuevaContrasena);
            bool resultado = _dal.CambiarContrasena(usuario, hash);

            if (resultado)
                _bitacora.RegistrarAccion(usuario, "CAMBIO_CONTRASENA");

            return resultado;
        }
    }
}