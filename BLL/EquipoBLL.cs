using DAL;
using System.Collections.Generic;

namespace BLL
{
    public class EquipoBLL
    {
        private readonly DAL.EquipoDAL _dal = new DAL.EquipoDAL();
        private readonly AlertaRevisionBLL _alertaBLL = new AlertaRevisionBLL();

        public BE.Equipo ObtenerPorId(int id) => _dal.ObtenerPorId(id);

        public List<BE.Equipo> ListarTodos() => _dal.ListarTodos();

        // UC1: Registrar uso de la máquina.
        // Devuelve false en el flujo 3a (sin umbral configurado) para que la UI avise al Administrador.
        public bool RegistrarUso(int equipoId, int incremento = 1)
        {
            var (usoAcumulado, nivelUsoCritico) = _dal.ActualizarUso(equipoId, incremento);

            if (!nivelUsoCritico.HasValue)
                return false;

            if (usoAcumulado >= nivelUsoCritico.Value)
                _alertaBLL.GenerarAlerta(equipoId);

            return true;
        }

        public void ReiniciarContador(int equipoId) => _dal.ReiniciarContador(equipoId);

        public void MarcarEnMantenimiento(int equipoId) => _dal.MarcarEnMantenimiento(equipoId);
    }
}