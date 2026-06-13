using System;
using System.Collections.Generic;
using SeguridadYServicios;

namespace BLL
{
    public class UsuarioHistorialBLL
    {
        private readonly DAL.UsuarioHistorialDAL _dal       = new DAL.UsuarioHistorialDAL();
        private readonly DAL.UsuarioDAL          _usuarioDal = new DAL.UsuarioDAL();
        private readonly DAL.UsuarioPerfilDAL    _perfilDal  = new DAL.UsuarioPerfilDAL();


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
                Nombre           = u.Nombre,
                Apellido         = u.Apellido,
                RealizadoPor     = realizadoPor,
                TipoCambio       = tipoCambio,
                VersionOrigen    = versionOrigen
            });
        }


        public List<BE.UsuarioHistorial> ObtenerHistorial(int usuarioId)
        {
            return _dal.ListarPorUsuario(usuarioId);
        }


        public void Rollback(int historialId, string realizadoPor)
        {
            BE.UsuarioHistorial snapshot = _dal.ObtenerPorId(historialId);
            if (snapshot == null)
                throw new InvalidOperationException("Versión histórica no encontrada.");

            if (snapshot.TipoCambio == "ROLLBACK")
                throw new InvalidOperationException("No se puede restaurar una versión que ya es un rollback.");

            _usuarioDal.AplicarEstado(
                snapshot.UsuarioId,
                snapshot.Rol,
                snapshot.Bloqueado,
                snapshot.IntentosFallidos);

            _perfilDal.BorrarTodos(snapshot.UsuarioId);
            foreach (int pid in ParsearPerfiles(snapshot.Perfiles))
                _perfilDal.Asignar(snapshot.UsuarioId, pid);

            _usuarioDal.ActualizarDatos(snapshot.UsuarioId, snapshot.Nombre, snapshot.Apellido);

            RegistrarCambio(snapshot.UsuarioId, "ROLLBACK", realizadoPor, historialId);
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
