using System;
using System.Collections.Generic;

namespace BLL
{
    public class FranjaHorariaBLL
    {
        private readonly DAL.FranjaHorariaDAL _dal = new DAL.FranjaHorariaDAL();

        public List<BE.FranjaHoraria> ListarTodas() => _dal.ListarTodas();

        public int Insertar(string dia, TimeSpan horaInicio, TimeSpan horaFin, string rolRequerido)
        {
            if (horaFin <= horaInicio)
                throw new ArgumentException("La hora de fin tiene que ser posterior a la hora de inicio.");

            return _dal.Insertar(dia, horaInicio, horaFin, rolRequerido);
        }

        public bool Eliminar(int id)
        {
            return _dal.Eliminar(id);
        }
    }

}