using System.Collections.Generic;

namespace BLL
{
    public class InformeMantenimientoBLL
    {
        private readonly DAL.InformeMantenimientoDAL _dal = new DAL.InformeMantenimientoDAL();
        private readonly AlertaRevisionBLL _alertaBLL = new AlertaRevisionBLL();
        private readonly EquipoBLL _equipoBLL = new EquipoBLL();
        private readonly NotificacionBLL _notificacionBLL = new NotificacionBLL();

        public BE.InformeMantenimiento RegistrarInforme(BE.VisitaTecnica visita, int alertaId, string resultado, bool pendienteRepuesto)
        {
            int id = _dal.Insertar(visita.Id, resultado, pendienteRepuesto);

            if (pendienteRepuesto)
                _alertaBLL.MantenerActiva(alertaId);

            BE.AlertaRevision alerta = _alertaBLL.ObtenerPorId(alertaId);
            BE.Equipo equipo = alerta != null ? _equipoBLL.ObtenerPorId(alerta.EquipoId) : null;
            string tecnico = SeguridadYServicios.SessionManager.getInstance().getUsuario()?.Usuario ?? "El técnico";
            _notificacionBLL.NotificarAdministradores(
                $"{tecnico} registró el informe de mantenimiento de '{equipo?.Nombre ?? "equipo"}'" +
                (pendienteRepuesto ? " (falta un repuesto)" : "") + ". Está pendiente de cierre.");

            return _dal.Obtener(id);
        }

        public List<BE.InformeMantenimiento> ListarPendientesCierre() => _dal.ListarPendientesCierre();

        public void ConfirmarCierre(BE.InformeMantenimiento informe)
        {
            if (informe.PendienteRepuesto)
                _equipoBLL.MarcarEnMantenimiento(informe.EquipoId);
            else
            {
                _equipoBLL.ReiniciarContador(informe.EquipoId);
                if (informe.AlertaId > 0)
                    _alertaBLL.MarcarResuelta(informe.AlertaId);
            }

            _dal.MarcarCerrado(informe.Id);
        }
    }
}