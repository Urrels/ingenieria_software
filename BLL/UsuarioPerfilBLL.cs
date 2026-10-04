using System.Collections.Generic;
using SeguridadYServicios;

namespace BLL
{
    public class UsuarioPerfilBLL
    {
        private readonly DAL.UsuarioPerfilDAL _dal = new DAL.UsuarioPerfilDAL();
        private readonly DAL.UsuarioDAL _usuarioDal = new DAL.UsuarioDAL();
        private readonly IntegridadBLL _integridad = new IntegridadBLL();
        private readonly UsuarioHistorialBLL _historial = new UsuarioHistorialBLL();
        private readonly BitacoraBLL _bitacora = new BitacoraBLL();

        public List<string> ObtenerPermisos(int usuarioId)
        {
            return _dal.ListarPermisosDeUsuario(usuarioId);
        }

        public List<int> ObtenerPerfilesAsignados(int usuarioId)
        {
            return _dal.ListarPerfilesPorUsuario(usuarioId);
        }

        public void GuardarAsignaciones(int usuarioId, List<int> perfilIds)
        {
            new PerfilBLL().ValidarQueQuedeAdministracion(usuarioId: usuarioId, nuevosRolIds: perfilIds);

            _dal.ReemplazarAsignaciones(usuarioId, perfilIds);
            _integridad.RecalcularIntegridadUsuarios();
            string admin = SessionManager.getInstance().getUsuario()?.Usuario ?? "sistema";
            _historial.RegistrarCambio(usuarioId, "ASIGNACION_PERFIL", admin);
            string nombreUsuario = _usuarioDal.ObtenerPorId(usuarioId)?.Usuario ?? usuarioId.ToString();
            _bitacora.RegistrarAccion(admin, "PERFILES_ASIGNADOS:" + nombreUsuario);

            BE.USUARIO usuarioSesion = SessionManager.getInstance().getUsuario();
            if (usuarioSesion != null && usuarioSesion.Id == usuarioId)
            {
                BE.USUARIO usuarioActualizado = _usuarioDal.ObtenerPorId(usuarioId);
                SessionManager.getInstance().setUsuario(usuarioActualizado);

                List<string> permisosActualizados = _dal.ListarPermisosDeUsuario(usuarioId);
                SessionManager.getInstance().setPermisos(permisosActualizados);
            }
        }
    }
}