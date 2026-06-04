using System;
using System.Collections.Generic;
using SeguridadYServicios;

namespace BLL
{
    /// <summary>
    /// Gestiona el historial de cambios de USUARIO.
    /// La tabla USUARIO_HISTORIAL es inmutable: solo se insertan registros, nunca se modifican.
    /// Cada fila es un snapshot completo del estado del usuario en ese momento.
    /// PASS no se almacena: es sensible y no puede ser revertida.
    ///
    /// Tipos de cambio registrados:
    ///   ALTA, CAMBIO_CLAVE, BLOQUEO, DESBLOQUEO, ASIGNACION_PERFIL, BAJA, ROLLBACK
    ///
    /// El ROLLBACK también es un cambio: genera un nuevo registro con VERSION_ORIGEN
    /// apuntando al snapshot que se restauró, haciendo la operación trazable y reversible.
    /// </summary>
    public class UsuarioHistorialBLL
    {
        private readonly DAL.UsuarioHistorialDAL _dal       = new DAL.UsuarioHistorialDAL();
        private readonly DAL.UsuarioDAL          _usuarioDal = new DAL.UsuarioDAL();
        private readonly DAL.UsuarioPerfilDAL    _perfilDal  = new DAL.UsuarioPerfilDAL();

        // -------------------------------------------------------
        // Registro de cambios
        // -------------------------------------------------------

        /// <summary>
        /// Captura el estado ACTUAL del usuario y lo inserta en el historial.
        /// Debe llamarse DESPUÉS de aplicar el cambio (excepto para BAJA: llamar antes).
        /// </summary>
        public void RegistrarCambio(int usuarioId, string tipoCambio, string realizadoPor,
                                    int? versionOrigen = null)
        {
            BE.USUARIO u = _usuarioDal.ObtenerPorId(usuarioId);
            if (u == null) return;

            List<int> ids = _perfilDal.ListarPerfilesPorUsuario(usuarioId);
            ids.Sort();
            string perfilesStr = string.Join(",", ids);

            _dal.Insertar(new BE.UsuarioHistorial
            {
                UsuarioId        = usuarioId,
                UsuarioLogin     = u.Usuario,
                Rol              = u.Rol,
                Bloqueado        = u.Bloqueado,
                IntentosFallidos = u.IntentosFallidos,
                Perfiles         = perfilesStr,
                RealizadoPor     = realizadoPor,
                TipoCambio       = tipoCambio,
                VersionOrigen    = versionOrigen
            });
        }

        // -------------------------------------------------------
        // Consulta
        // -------------------------------------------------------

        public List<BE.UsuarioHistorial> ObtenerHistorial(int usuarioId)
        {
            return _dal.ListarPorUsuario(usuarioId);
        }

        // -------------------------------------------------------
        // Rollback
        // -------------------------------------------------------

        /// <summary>
        /// Restaura el estado de un usuario al snapshot indicado por historialId.
        /// PASS no se modifica. El rollback se registra como un nuevo cambio ROLLBACK.
        /// </summary>
        public void Rollback(int historialId, string realizadoPor)
        {
            BE.UsuarioHistorial snapshot = _dal.ObtenerPorId(historialId);
            if (snapshot == null)
                throw new InvalidOperationException("Versión histórica no encontrada.");

            // 1 — Aplicar campos al usuario actual (sin PASS)
            _usuarioDal.AplicarEstado(
                snapshot.UsuarioId,
                snapshot.Rol,
                snapshot.Bloqueado,
                snapshot.IntentosFallidos);

            // 2 — Restaurar perfiles
            _perfilDal.BorrarTodos(snapshot.UsuarioId);
            foreach (int pid in ParsearPerfiles(snapshot.Perfiles))
                _perfilDal.Asignar(snapshot.UsuarioId, pid);

            // 3 — Registrar el rollback como nuevo cambio (apuntando al snapshot restaurado)
            RegistrarCambio(snapshot.UsuarioId, "ROLLBACK", realizadoPor, historialId);

            // 4 — Recalcular integridad
            new IntegridadBLL().RecalcularIntegridadUsuarios();
        }

        private List<int> ParsearPerfiles(string perfilesStr)
        {
            List<int> result = new List<int>();
            if (string.IsNullOrEmpty(perfilesStr)) return result;
            foreach (string s in perfilesStr.Split(','))
                if (int.TryParse(s.Trim(), out int id)) result.Add(id);
            return result;
        }
    }
}
