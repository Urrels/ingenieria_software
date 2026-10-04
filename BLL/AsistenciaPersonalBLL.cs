using System;

namespace BLL
{
    public class AsistenciaPersonalBLL
    {
        private readonly DAL.AsistenciaPersonalDAL _dal = new DAL.AsistenciaPersonalDAL();

        public bool RegistrarIngreso(BE.USUARIO usuario)
        {
            var abierta = _dal.ObtenerAbierta(usuario.Id, DateTime.Today);
            if (abierta != null)
                return false;

            _dal.RegistrarIngreso(usuario.Id, DateTime.Today, DateTime.Now.TimeOfDay);
            return true;
        }

        public bool RegistrarEgreso(BE.USUARIO usuario)
        {
            var abierta = _dal.ObtenerAbierta(usuario.Id, DateTime.Today);
            if (abierta == null)
                return false;

            _dal.RegistrarEgreso(abierta.Id, DateTime.Now.TimeOfDay);
            return true;
        }

        public bool TieneIngresoAbierto(BE.USUARIO usuario)
        {
            return _dal.ObtenerAbierta(usuario.Id, DateTime.Today) != null;
        }
    }
}