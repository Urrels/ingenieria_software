using System;
using System.Collections.Generic;

namespace BLL
{
    public class VisitaTecnicaBLL
    {
        private readonly DAL.VisitaTecnicaDAL _dal = new DAL.VisitaTecnicaDAL();
        private readonly UsuarioBLL _usuarioBLL = new UsuarioBLL();

        // UC3: si el equipo tiene técnico habitual asignado, se muestra directo
        public BE.USUARIO ObtenerTecnicoHabitual(BE.Equipo equipo)
        {
            return equipo.TecnicoHabitualId.HasValue
                ? _usuarioBLL.ObtenerPorId(equipo.TecnicoHabitualId.Value)
                : null;
        }

        // UC3, flujo 3a: técnico habitual no disponible
        public List<BE.USUARIO> ListarTecnicosAlternativos() => _dal.ListarTecnicosAlternativos();

        public int CoordinarVisita(int alertaId, int tecnicoId, DateTime fechaCoordinada)
        {
            return _dal.Insertar(alertaId, tecnicoId, fechaCoordinada);
        }

        // UC4: el técnico ve qué visitas tiene coordinadas para atender
        public List<BE.VisitaTecnica> ListarPendientesPorTecnico(int tecnicoId) =>
            _dal.ListarPendientesPorTecnico(tecnicoId);
    }
}