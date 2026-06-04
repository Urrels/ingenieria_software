using BE;
using System.Collections.Generic;

namespace BLL
{
    public class BitacoraBLL
    {
        private readonly DAL.BitacoraDAL _dal        = new DAL.BitacoraDAL();
        private readonly IntegridadBLL   _integridad = new IntegridadBLL();

        public void RegistrarLogin(string usuario)
        {
            _dal.Registrar(usuario, "LOGIN");
            _integridad.RecalcularIntegridadBitacora();
        }

        public void RegistrarLogout(string usuario)
        {
            _dal.Registrar(usuario, "LOGOUT");
            _integridad.RecalcularIntegridadBitacora();
        }

        public void RegistrarAccion(string usuario, string accion)
        {
            _dal.Registrar(usuario, accion);
            _integridad.RecalcularIntegridadBitacora();
        }

        public List<BE.BITACORA> Listar()
        {
            return _dal.Listar();
        }
    }
}