using System.Collections.Generic;

namespace BLL
{
    public class InformeMantenimientoBLL
    {
        private readonly DAL.InformeMantenimientoDAL _dal = new DAL.InformeMantenimientoDAL();
        private readonly AlertaRevisionBLL _alertaBLL = new AlertaRevisionBLL();
        private readonly EquipoBLL _equipoBLL = new EquipoBLL();

        // UC4
        public BE.InformeMantenimiento RegistrarInforme(BE.VisitaTecnica visita, int alertaId, string resultado, bool pendienteRepuesto)
        {
            int id = _dal.Insertar(visita.Id, resultado, pendienteRepuesto);

            if (pendienteRepuesto)
                _alertaBLL.MantenerActiva(alertaId);

            return _dal.Obtener(id);
        }

        public List<BE.InformeMantenimiento> ListarPendientesCierre() => _dal.ListarPendientesCierre();

        public BE.InformeMantenimiento ObtenerPorId(int id) => _dal.Obtener(id);

        // UC5
        public void ConfirmarCierre(BE.InformeMantenimiento informe)
        {
            if (informe.PendienteRepuesto)
                _equipoBLL.MarcarEnMantenimiento(informe.EquipoId);
            else
                _equipoBLL.ReiniciarContador(informe.EquipoId);

            _dal.MarcarCerrado(informe.Id);
        }
    }
}