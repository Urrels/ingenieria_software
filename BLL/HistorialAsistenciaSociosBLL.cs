using System;

namespace BLL
{
    public class HistorialAsistenciaSociosBLL
    {
        private readonly DAL.HistorialAsistenciaSociosDAL _dal = new DAL.HistorialAsistenciaSociosDAL();

        // UC3
        public void RegistrarAsistencia(BE.FranjaHoraria franja, DateTime fecha, int cantidadSocios)
        {
            _dal.Insertar(franja.Id, fecha, cantidadSocios);
        }
    }
}