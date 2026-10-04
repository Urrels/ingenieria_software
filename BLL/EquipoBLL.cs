using System;
using System.Collections.Generic;

namespace BLL
{
    public class EquipoBLL
    {
        private readonly DAL.EquipoDAL _dal = new DAL.EquipoDAL();
        private readonly AlertaRevisionBLL _alertaBLL = new AlertaRevisionBLL();
        private readonly NotificacionBLL _notificacionBLL = new NotificacionBLL();

        public BE.Equipo ObtenerPorId(int id) => _dal.ObtenerPorId(id);

        public BE.Equipo ObtenerPorNombre(string nombre) => _dal.ObtenerPorNombre(nombre);

        public List<BE.Equipo> ListarTodos() => _dal.ListarTodos();

        public bool RegistrarUso(int equipoId, int incremento = 1)
        {
            var (usoAcumulado, nivelUsoCritico) = _dal.ActualizarUso(equipoId, incremento);
            _dal.RegistrarUsoLog(equipoId, incremento);

            if (!nivelUsoCritico.HasValue)
            {
                string nombre = _dal.ObtenerPorId(equipoId)?.Nombre ?? equipoId.ToString();
                _notificacionBLL.NotificarAdministradores(
                    $"El equipo '{nombre}' no tiene un nivel de uso crítico configurado. Definilo para que el sistema pueda generar alertas de revisión.",
                    unaVezPorDia: true);
                return false;
            }

            if (usoAcumulado >= nivelUsoCritico.Value)
                _alertaBLL.GenerarAlerta(equipoId);

            return true;
        }

        public void ReiniciarContador(int equipoId) => _dal.ReiniciarContador(equipoId);

        public void MarcarEnMantenimiento(int equipoId) => _dal.MarcarEnMantenimiento(equipoId);

        public int? EstimarDiasHastaCritico(BE.Equipo equipo, int ventanaDias = 14)
        {
            if (!equipo.NivelUsoCritico.HasValue) return null;

            var (usoReciente, desde) = _dal.ObtenerUsoUltimosDias(equipo.Id, ventanaDias);
            if (usoReciente == 0 || desde == null) return null;

            double diasTranscurridos = Math.Max(1, (DateTime.Now - desde.Value).TotalDays);
            double velocidadDiaria = usoReciente / diasTranscurridos;
            if (velocidadDiaria <= 0) return null;

            int faltante = equipo.NivelUsoCritico.Value - equipo.UsoAcumulado;
            if (faltante <= 0) return 0;

            return (int)Math.Ceiling(faltante / velocidadDiaria);
        }
    }
}