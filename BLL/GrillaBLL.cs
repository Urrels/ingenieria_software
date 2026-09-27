using SeguridadYServicios;
using System;
using System.Collections.Generic;

namespace BLL
{
    public class GrillaBLL
    {
        private readonly DAL.GrillaDAL _grillaDAL = new DAL.GrillaDAL();
        private readonly DAL.TurnoDAL _turnoDAL = new DAL.TurnoDAL();
        private readonly BLL.FranjaHorariaBLL _franjaBLL = new BLL.FranjaHorariaBLL();
        private readonly DemandaEstimadaBLL _demandaBLL = new DemandaEstimadaBLL();
        private readonly NotificacionBLL _notificacionBLL = new NotificacionBLL();
        private readonly UsuarioBLL _usuarioBLL = new UsuarioBLL();

        public BE.GrillaDeTurnos GenerarGrilla(DateTime semana)
        {
            var existente = _grillaDAL.ObtenerPorSemana(semana);
            if (existente != null)
            {
                existente.Turnos = _turnoDAL.ListarPorGrilla(existente.Id);
                return existente;   // ya había una grilla para esa semana, se reusa
            }

            var admin = SessionManager.getInstance().getUsuario();

            var grilla = new BE.GrillaDeTurnos
            {
                Semana = semana,
                Estado = "Propuesta",
                AdministradorId = admin.Id
            };
            grilla.Id = _grillaDAL.Crear(grilla);

            foreach (var franja in _franjaBLL.ListarTodas())
            {
                BE.DemandaEstimada demanda = _demandaBLL.CalcularDemanda(semana, franja);

                for (int i = 0; i < demanda.CantidadPersonalNecesario; i++)
                {
                    var turno = new BE.Turno
                    {
                        GrillaId = grilla.Id,
                        FranjaId = franja.Id,
                        RolRequerido = demanda.RolRequerido,
                        Estado = "SinAsignar"
                    };
                    turno.Id = _turnoDAL.Agregar(turno);
                    grilla.Turnos.Add(turno);
                }
            }
            return grilla;
        }

        public bool ConfirmarGrilla(BE.GrillaDeTurnos grilla)
        {
            // Bloquea solo si queda un turno sin asignar Y sin marcar como déficit
            if (grilla.Turnos.Exists(t => t.UsuarioId == null && t.Estado != "DeficitCobertura"))
                return false;

            _grillaDAL.Confirmar(grilla.Id);
            grilla.Estado = "Confirmada";
            return true;
        }
        public Dictionary<int, List<BE.Turno>> ObtenerTurnosPorEmpleado(BE.GrillaDeTurnos grilla)
        {
            return _grillaDAL.ObtenerTurnosPorEmpleado(grilla.Id);
        }

        public void ComunicarHorarios(BE.GrillaDeTurnos grilla)
        {
            foreach (var par in ObtenerTurnosPorEmpleado(grilla))
            {
                BE.USUARIO usuario = _usuarioBLL.ObtenerPorId(par.Key);
                string mensaje = $"Tenés {par.Value.Count} turno(s) asignado(s) para la semana del {grilla.Semana:dd/MM}.";
                _notificacionBLL.Notificar(usuario, grilla.Id, mensaje);
            }

            _grillaDAL.MarcarComunicada(grilla.Id);
            grilla.Estado = "Comunicada";
        }

        public BE.GrillaDeTurnos ObtenerConTurnosPorSemana(DateTime semana)
        {
            var grilla = _grillaDAL.ObtenerPorSemana(semana);
            if (grilla == null) return null;
            grilla.Turnos = _turnoDAL.ListarPorGrilla(grilla.Id);
            return grilla;
        }

    }
}