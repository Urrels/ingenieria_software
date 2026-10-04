using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    public class TurnoBLL
    {
        private readonly DAL.TurnoDAL _turnoDAL = new DAL.TurnoDAL();
        private readonly DAL.FranjaHorariaDAL _franjaDAL = new DAL.FranjaHorariaDAL();
        private readonly NotificacionBLL _notificacionBLL = new NotificacionBLL();
        private readonly UsuarioBLL _usuarioBLL = new UsuarioBLL();

        public bool AsignarEmpleado(BE.Turno turno, BE.USUARIO usuario)
        {
            if (SuperaLimiteHoras(usuario, turno))
                return false;

            _turnoDAL.ActualizarAsignacion(turno.Id, usuario.Id, "Asignado");
            turno.UsuarioId = usuario.Id;
            turno.Estado = "Asignado";
            return true;
        }

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

        public List<BE.Turno> ListarPorUsuario(int usuarioId) => _turnoDAL.ListarPorUsuario(usuarioId);

        public List<BE.USUARIO> CancelarPorEmpleado(BE.Turno turno, BE.USUARIO empleadoQueCancela)
        {
            _turnoDAL.ActualizarAsignacion(turno.Id, null, "PendienteCobertura");
            turno.UsuarioId = null;
            turno.Estado = "PendienteCobertura";

            var franja = _franjaDAL.ObtenerPorId(turno.FranjaId);

            List<BE.USUARIO> candidatos = _turnoDAL.BuscarCompatibles(turno.RolRequerido, turno.FranjaId, turno.GrillaId)
                .Where(u => u.Id != empleadoQueCancela.Id)
                .ToList();

            string mensajeCandidatos = $"Se necesita cobertura para el turno del {franja.Dia} " +
                $"{franja.HoraInicio:hh\\:mm}-{franja.HoraFin:hh\\:mm}. Avisale al administrador si podés cubrirlo.";

            foreach (var candidato in candidatos)
                _notificacionBLL.Notificar(candidato, turno.GrillaId, mensajeCandidatos);

            string mensajeAdmin = $"{empleadoQueCancela.Nombre} {empleadoQueCancela.Apellido} canceló su turno del " +
                $"{franja.Dia} {franja.HoraInicio:hh\\:mm}-{franja.HoraFin:hh\\:mm}. " +
                $"Se notificó a {candidatos.Count} empleado(s) disponible(s).";

            foreach (var admin in _usuarioBLL.ListarAdmins())
                _notificacionBLL.Notificar(admin, turno.GrillaId, mensajeAdmin);

            return candidatos;
        }

        public bool TomarCobertura(BE.Turno turno, BE.USUARIO empleado)
        {
            if (SuperaLimiteHoras(empleado, turno))
                return false;

            bool gano = _turnoDAL.TomarCobertura(turno.Id, empleado.Id);
            if (!gano) return false;

            turno.UsuarioId = empleado.Id;
            turno.Estado = "Asignado";

            var franja = _franjaDAL.ObtenerPorId(turno.FranjaId);
            string mensajeAdmin = $"{empleado.Nombre} {empleado.Apellido} tomó el turno del {franja.Dia} " +
                $"{franja.HoraInicio:hh\\:mm}-{franja.HoraFin:hh\\:mm} que estaba pendiente de cobertura.";
            foreach (var admin in _usuarioBLL.ListarAdmins())
                _notificacionBLL.Notificar(admin, turno.GrillaId, mensajeAdmin);

            return true;
        }

        public List<BE.Turno> ListarPendientesPorRol(string rolRequerido) => _turnoDAL.ListarPendientesPorRol(rolRequerido);
    }
}