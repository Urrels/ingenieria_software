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
                return null;   // flujo 5a: el llamador (UI) muestra el aviso

            disponibilidad.Id = _dal.Guardar(disponibilidad);
            return disponibilidad;
        }

        // Flujo 3a (ya había disponibilidad cargada): la UI llama esto primero
        // para precargar el formulario con lo que el empleado ya había puesto.
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