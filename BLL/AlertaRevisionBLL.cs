using System.Collections.Generic;

namespace BLL
{
    public class AlertaRevisionBLL
    {
        private readonly DAL.AlertaRevisionDAL _dal = new DAL.AlertaRevisionDAL();
        private readonly DAL.EquipoDAL _equipoDAL = new DAL.EquipoDAL();

        public BE.AlertaRevision GenerarAlerta(int equipoId)
        {
            int id = _dal.Insertar(equipoId);
            return _dal.Obtener(id);
        }

        public List<BE.AlertaRevision> ListarPendientes() => _dal.ListarPendientes();

        public BE.Equipo ObtenerEquipoDeAlerta(BE.AlertaRevision alerta) => _equipoDAL.ObtenerPorId(alerta.EquipoId);

        // UC2, flujo Sí
        public void AutorizarVisita(BE.AlertaRevision alerta)
        {
            _dal.ActualizarEstado(alerta.Id, "Autorizada");
            alerta.Estado = "Autorizada";
        }

        // UC2, flujo 3a (falso positivo)
        public void DescartarAlerta(BE.AlertaRevision alerta)
        {
            _dal.ActualizarEstado(alerta.Id, "Descartada");
            alerta.Estado = "Descartada";
        }

        // UC4, flujo 3a (repuesto pendiente): la alerta vuelve a "Pendiente" hasta la próxima visita
        public void MantenerActiva(int alertaId)
        {
            _dal.ActualizarEstado(alertaId, "Pendiente");
        }

        public List<BE.AlertaRevision> ListarAutorizadas() => _dal.ListarAutorizadas();

        public void MarcarCoordinada(BE.AlertaRevision alerta)
        {
            _dal.ActualizarEstado(alerta.Id, "Coordinada");
            alerta.Estado = "Coordinada";
        }

        public BE.AlertaRevision ObtenerPorId(int id) => _dal.Obtener(id);
    }
}