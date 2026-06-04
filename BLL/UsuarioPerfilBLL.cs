using System.Collections.Generic;

namespace BLL
{
    public class UsuarioPerfilBLL
    {
        private readonly DAL.UsuarioPerfilDAL _dal        = new DAL.UsuarioPerfilDAL();
        private readonly IntegridadBLL        _integridad = new IntegridadBLL();
        private readonly UsuarioHistorialBLL  _historial  = new UsuarioHistorialBLL();

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
            _dal.BorrarTodos(usuarioId);
            foreach (int id in perfilIds)
                _dal.Asignar(usuarioId, id);
            _integridad.RecalcularIntegridadUsuarios();
            string admin = SeguridadYServicios.SessionManager.getInstance().getUsuario()?.Usuario ?? "sistema";
            _historial.RegistrarCambio(usuarioId, "ASIGNACION_PERFIL", admin);
        }
    }
}
