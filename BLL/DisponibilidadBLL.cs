using System;
using System.Collections.Generic;

namespace BLL
{
    public class DisponibilidadBLL
    {
        private readonly DAL.DisponibilidadDAL _dal = new DAL.DisponibilidadDAL();

        public BE.Disponibilidad RegistrarDisponibilidad(BE.USUARIO usuario, DateTime semana, List<BE.FranjaHoraria> franjas)
        {
            var disponibilidad = new BE.Disponibilidad
            {
                UsuarioId = usuario.Id,
                Semana = semana,
                Franjas = franjas
            };

            if (!TieneAlMenosUnaFranja(disponibilidad))
                return null;

            BE.Disponibilidad previa = _dal.ObtenerPorUsuarioYSemana(usuario.Id, semana);
            if (previa != null)
            {
                disponibilidad.Id = previa.Id;
                _dal.Actualizar(disponibilidad);
            }
            else
            {
                disponibilidad.Id = _dal.Guardar(disponibilidad);
            }

            return disponibilidad;
        }

        public BE.Disponibilidad ObtenerPrevia(BE.USUARIO usuario, DateTime semana)
        {
            return _dal.ObtenerPorUsuarioYSemana(usuario.Id, semana);
        }

        public bool TieneAlMenosUnaFranja(BE.Disponibilidad disponibilidad)
        {
            return disponibilidad.Franjas != null && disponibilidad.Franjas.Count > 0;
        }
    }
}