using System.Collections.Generic;

namespace BLL
{
    public class TurnoBLL
    {
        private readonly DAL.TurnoDAL _turnoDAL = new DAL.TurnoDAL();
        private readonly DAL.FranjaHorariaDAL _franjaDAL = new DAL.FranjaHorariaDAL();

        public bool AsignarEmpleado(BE.Turno turno, BE.USUARIO usuario)
        {
            if (SuperaLimiteHoras(usuario, turno))
                return false;

            _turnoDAL.ActualizarAsignacion(turno.Id, usuario.Id, "Asignado");
            turno.UsuarioId = usuario.Id;
            turno.Estado = "Asignado";
            return true;
        }

        public bool ReemplazarEmpleado(BE.Turno turno, BE.USUARIO usuario) => AsignarEmpleado(turno, usuario);

        public bool SuperaLimiteHoras(BE.USUARIO usuario, BE.Turno turno)
        {
            if (!usuario.LimiteHorasSemanales.HasValue) return false;

            decimal horasAsignadas = _turnoDAL.HorasAsignadasEnSemana(usuario.Id, turno.GrillaId);
            var franja = _franjaDAL.ObtenerPorId(turno.FranjaId);
            decimal horasDelTurno = (decimal)(franja.HoraFin - franja.HoraInicio).TotalHours;

            return (horasAsignadas + horasDelTurno) > usuario.LimiteHorasSemanales.Value;
        }

        public List<BE.USUARIO> BuscarEmpleadosCompatibles(string rol, BE.FranjaHoraria franja, int grillaId)
        {
            return _turnoDAL.BuscarCompatibles(rol, franja.Id, grillaId);
        }

        public void MarcarDeficitCobertura(BE.Turno turno)
        {
            _turnoDAL.ActualizarAsignacion(turno.Id, null, "DeficitCobertura");
            turno.Estado = "DeficitCobertura";
            turno.UsuarioId = null;
        }

        public bool ReemplazarPorAusencia(BE.Turno turno, BE.USUARIO nuevoEmpleado)
        {
            if (SuperaLimiteHoras(nuevoEmpleado, turno))
                return false;

            _turnoDAL.ActualizarAsignacion(turno.Id, nuevoEmpleado.Id, "Asignado");
            turno.UsuarioId = nuevoEmpleado.Id;
            return true;
        }
    }
}