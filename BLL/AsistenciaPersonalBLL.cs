using System;

namespace BLL
{
    public class AsistenciaPersonalBLL
    {
        private readonly DAL.AsistenciaPersonalDAL _dal = new DAL.AsistenciaPersonalDAL();

        // UC1
        public bool RegistrarIngreso(BE.USUARIO usuario)
        {
            var abierta = _dal.ObtenerAbierta(usuario.Id, DateTime.Today);
            if (abierta != null)
                return false;   // ya tiene un ingreso sin egreso hoy

            _dal.RegistrarIngreso(usuario.Id, DateTime.Today, DateTime.Now.TimeOfDay);
            return true;
        }

        // UC2, flujo 1a incluido: si no hay ingreso abierto, devuelve false
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