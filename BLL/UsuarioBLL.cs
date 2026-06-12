using BE;
using SeguridadYServicios;

namespace BLL
{
    public class UsuarioBLL
    {
        private readonly DAL.UsuarioDAL      _dal        = new DAL.UsuarioDAL();
        private readonly BitacoraBLL         _bitacora   = new BitacoraBLL();
        private readonly IntegridadBLL       _integridad = new IntegridadBLL();
        private readonly UsuarioHistorialBLL _historial  = new UsuarioHistorialBLL();

        public BE.USUARIO ObtenerPorId(int id)
        {
            return _dal.ObtenerPorId(id);
        }

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
            {
                _bitacora.RegistrarAccion(usuario, "CAMBIO_CONTRASENA");
                _integridad.RecalcularIntegridadUsuarios();
                int? id = _dal.ObtenerIdPorNombre(usuario);
                if (id.HasValue) _historial.RegistrarCambio(id.Value, "CAMBIO_CLAVE", usuario);
            }

            return resultado;
        }

        public System.Collections.Generic.List<BE.USUARIO> ListarTodos()
        {
            return _dal.ListarTodos();
        }

        public void Eliminar(int id)
        {
            string admin = SessionManager.getInstance().getUsuario().Usuario;
            _historial.RegistrarCambio(id, "BAJA", admin);
            _dal.Eliminar(id);
            _integridad.RecalcularIntegridadUsuarios();
        }

        public bool Crear(string usuario, string contrasena, string rol)
        {
            string hash = Hasher.Hashear(contrasena);
            bool resultado = _dal.Crear(usuario, hash, rol);
            if (resultado)
            {
                _integridad.RecalcularIntegridadUsuarios();
                string admin = SessionManager.getInstance().getUsuario()?.Usuario ?? "sistema";
                int? id = _dal.ObtenerIdPorNombre(usuario);
                if (id.HasValue) _historial.RegistrarCambio(id.Value, "ALTA", admin);
            }
            return resultado;
        }

        public void Desbloquear(string usuario)
        {
            _dal.Desbloquear(usuario);
            string admin = SessionManager.getInstance().getUsuario().Usuario;
            _bitacora.RegistrarAccion(admin, "DESBLOQUEO_USUARIO:" + usuario);
            _integridad.RecalcularIntegridadUsuarios();
            int? id = _dal.ObtenerIdPorNombre(usuario);
            if (id.HasValue) _historial.RegistrarCambio(id.Value, "DESBLOQUEO", admin);
        }
    }
}
