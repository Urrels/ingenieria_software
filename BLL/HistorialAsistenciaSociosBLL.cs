using System;
using System.Collections.Generic;

namespace BLL
{
    public class HistorialAsistenciaSociosBLL
    {
        private readonly DAL.HistorialAsistenciaSociosDAL _dal = new DAL.HistorialAsistenciaSociosDAL();

        public void RegistrarAsistencia(BE.FranjaHoraria franja, DateTime fecha, int cantidadSocios)
        {
            _dal.Insertar(franja.Id, fecha, cantidadSocios);
        }
        public List<(DateTime fecha, int cantidad)> ObtenerSerieCompleta(int franjaId) => _dal.ObtenerSerieCompleta(franjaId);
    }
}