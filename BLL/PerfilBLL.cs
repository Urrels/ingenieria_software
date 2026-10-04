using BE;
using SeguridadYServicios;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    public class PerfilBLL
    {
        private static readonly string[] PermisosDeAdministracion = { "Administrar usuarios", "Gestión de roles" };

        private readonly DAL.PerfilDAL _dal = new DAL.PerfilDAL();
        private readonly DAL.UsuarioPerfilDAL _usuarioPerfilDal = new DAL.UsuarioPerfilDAL();
        private readonly BitacoraBLL _bitacora = new BitacoraBLL();

        public List<NodoPermiso> ObtenerArbol()
        {
            List<NodoPermiso> roles = _dal.ListarRoles();
            Dictionary<int, NodoPermiso> mapa = new Dictionary<int, NodoPermiso>();
            foreach (NodoPermiso r in roles) mapa[r.Id] = r;

            List<NodoPermiso> raices = new List<NodoPermiso>();
            foreach (NodoPermiso r in roles)
            {
                if (r.PadreId == null)
                    raices.Add(r);
                else if (mapa.ContainsKey(r.PadreId.Value))
                    mapa[r.PadreId.Value].Agregar(r);
            }

            foreach (Permiso p in _dal.ListarRolPermisos())
            {
                if (p.PadreId.HasValue && mapa.ContainsKey(p.PadreId.Value))
                    mapa[p.PadreId.Value].Agregar(p);
            }

            return raices;
        }

        public List<Permiso> ObtenerPermisosDisponibles()
        {
            return _dal.ListarPermisosDisponibles();
        }

        public List<Rol> ListarRolesParaCombo()
        {
            return _dal.ListarParaCombo();
        }

        public void ActualizarPermisosDeRol(int rolId, List<int> permisoIds)
        {
            ValidarQueQuedeAdministracion(rolId: rolId, nuevosPermisoIds: permisoIds);
            _dal.GuardarPermisosDeRol(rolId, permisoIds);
            RegistrarEnBitacora("ROL_PERMISOS_MODIFICADOS:", rolId);
        }

        public NodoPermiso AgregarRol(string nombre, int? padreId)
        {
            int id = _dal.Insertar(nombre, padreId);
            _bitacora.RegistrarAccion(UsuarioActual(), "ROL_CREADO:" + nombre);
            return new Rol { Id = id, Nombre = nombre, PadreId = padreId };
        }

        public void CambiarPadre(int rolId, int? nuevoPadreId)
        {
            if (nuevoPadreId.HasValue)
            {
                List<NodoPermiso> todos = _dal.ListarRoles();
                if (GenerariaCiclo(rolId, nuevoPadreId.Value, todos))
                    throw new InvalidOperationException(
                        "No se puede asignar ese padre: generaría una referencia circular.");

                int? padreActual = todos.FirstOrDefault(n => n.Id == rolId)?.PadreId;
                if (EsAncestroDelPadreActual(padreActual, nuevoPadreId.Value, todos))
                    throw new InvalidOperationException(
                        "No se puede asignar como padre a un ancestro del padre actual del rol.");
            }
            _dal.CambiarPadre(rolId, nuevoPadreId);
            RegistrarEnBitacora("ROL_PADRE_CAMBIADO:", rolId);
        }

        public void Eliminar(int id)
        {
            NodoPermiso nodo = _dal.ListarRoles().FirstOrDefault(n => n.Id == id);
            if (nodo is Rol r && r.Protegido)
                throw new InvalidOperationException(
                    "Este rol es del sistema y no puede eliminarse.");
            if (_dal.TieneUsuariosAsignados(id))
                throw new InvalidOperationException(
                    "No se puede eliminar un rol que tiene usuarios asignados. Reasignálos primero.");
            _dal.Eliminar(id);
            _bitacora.RegistrarAccion(UsuarioActual(), "ROL_ELIMINADO:" + (nodo?.Nombre ?? id.ToString()));
        }

        internal void ValidarQueQuedeAdministracion(int? rolId = null, List<int> nuevosPermisoIds = null,
                                                    int? usuarioId = null, List<int> nuevosRolIds = null)
        {
            Dictionary<int, List<int>> rolesPorUsuario = _usuarioPerfilDal.ListarRolesDeUsuariosActivos();
            Dictionary<int, HashSet<string>> permisosPorRol = new Dictionary<int, HashSet<string>>();
            foreach (Permiso p in _dal.ListarRolPermisos())
            {
                if (!p.PadreId.HasValue) continue;
                if (!permisosPorRol.ContainsKey(p.PadreId.Value))
                    permisosPorRol[p.PadreId.Value] = new HashSet<string>();
                permisosPorRol[p.PadreId.Value].Add(p.Nombre);
            }

            HashSet<string> nuevosPermisos = null;
            if (rolId.HasValue && nuevosPermisoIds != null)
            {
                nuevosPermisos = new HashSet<string>(
                    _dal.ListarPermisosDisponibles()
                        .Where(p => nuevosPermisoIds.Contains(p.Id))
                        .Select(p => p.Nombre));
            }

            string perdido = PermisoQueSePerderia(rolesPorUsuario, permisosPorRol,
                                                  rolId, nuevosPermisos, usuarioId, nuevosRolIds);
            if (perdido != null)
                throw new InvalidOperationException(
                    $"No se puede guardar: el sistema quedaría sin ningún usuario activo con el permiso '{perdido}'.");
        }

        internal static string PermisoQueSePerderia(Dictionary<int, List<int>> rolesPorUsuario,
                                                    Dictionary<int, HashSet<string>> permisosPorRol,
                                                    int? rolId, HashSet<string> nuevosPermisos,
                                                    int? usuarioId, List<int> nuevosRolIds)
        {
            HashSet<string> cubiertosAntes = PermisosCubiertos(rolesPorUsuario, permisosPorRol);

            var permisosDespues = new Dictionary<int, HashSet<string>>(permisosPorRol);
            if (rolId.HasValue && nuevosPermisos != null)
                permisosDespues[rolId.Value] = nuevosPermisos;

            var rolesDespues = new Dictionary<int, List<int>>(rolesPorUsuario);
            if (usuarioId.HasValue && nuevosRolIds != null && rolesDespues.ContainsKey(usuarioId.Value))
                rolesDespues[usuarioId.Value] = nuevosRolIds;

            HashSet<string> cubiertosDespues = PermisosCubiertos(rolesDespues, permisosDespues);
            return cubiertosAntes.FirstOrDefault(p => !cubiertosDespues.Contains(p));
        }

        private static HashSet<string> PermisosCubiertos(Dictionary<int, List<int>> rolesPorUsuario,
                                                         Dictionary<int, HashSet<string>> permisosPorRol)
        {
            HashSet<string> cubiertos = new HashSet<string>();
            foreach (List<int> roles in rolesPorUsuario.Values)
                foreach (int rol in roles)
                    if (permisosPorRol.TryGetValue(rol, out HashSet<string> permisos))
                        foreach (string permiso in permisos)
                            if (PermisosDeAdministracion.Contains(permiso))
                                cubiertos.Add(permiso);
            return cubiertos;
        }

        private void RegistrarEnBitacora(string accion, int rolId)
        {
            string nombre = _dal.ListarRoles().FirstOrDefault(n => n.Id == rolId)?.Nombre ?? rolId.ToString();
            _bitacora.RegistrarAccion(UsuarioActual(), accion + nombre);
        }

        private static string UsuarioActual()
        {
            return SessionManager.getInstance().getUsuario()?.Usuario ?? "sistema";
        }

        private bool GenerariaCiclo(int rolId, int candidatoPadreId, List<NodoPermiso> todos)
        {
            int? actual = candidatoPadreId;
            while (actual.HasValue)
            {
                if (actual.Value == rolId) return true;
                actual = todos.FirstOrDefault(n => n.Id == actual.Value)?.PadreId;
            }
            return false;
        }

        private bool EsAncestroDelPadreActual(int? padreActualId, int candidatoPadreId, List<NodoPermiso> todos)
        {
            int? actual = padreActualId.HasValue
                ? todos.FirstOrDefault(n => n.Id == padreActualId.Value)?.PadreId
                : null;
            while (actual.HasValue)
            {
                if (actual.Value == candidatoPadreId) return true;
                actual = todos.FirstOrDefault(n => n.Id == actual.Value)?.PadreId;
            }
            return false;
        }
    }
}
