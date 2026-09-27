using System;
using System.Linq;

namespace BLL
{
    public class DemandaEstimadaBLL
    {
        private readonly DAL.DemandaEstimadaDAL _dal = new DAL.DemandaEstimadaDAL();

        public BE.DemandaEstimada CalcularDemanda(DateTime semana, BE.FranjaHoraria franja)
        {
            var historial = _dal.ObtenerHistorial(franja.Id, 4);

            if (historial.Count < 4)
            {
                var manual = new BE.DemandaEstimada
                {
                    FranjaId = franja.Id,
                    Semana = semana,
                    RolRequerido = franja.RolRequerido,
                    CantidadPersonalNecesario = 0,   // la UI debe pedírselo al Administrador (flujo 3a de UC1)
                    OrigenDato = "Manual"
                };
                manual.Id = _dal.Insertar(manual);
                return manual;
            }

            var evalAnterior = ConsultarEvaluacionCobertura(semana.AddDays(-7), franja);

            int promedio = (int)historial.Average();
            int personalNecesario = promedio / 15;   // 1 empleado cada 15 socios — parametrizable a futuro

            if (evalAnterior != null && evalAnterior.Resultado == "Falta")
                personalNecesario++;

            var demanda = new BE.DemandaEstimada
            {
                FranjaId = franja.Id,
                Semana = semana,
                RolRequerido = franja.RolRequerido,
                CantidadPersonalNecesario = personalNecesario,
                OrigenDato = "Calculado"
            };
            demanda.Id = _dal.Insertar(demanda);
            return demanda;
        }

        public BE.EvaluacionCobertura ConsultarEvaluacionCobertura(DateTime semana, BE.FranjaHoraria franja)
        {
            return _dal.ObtenerEvaluacionCobertura(franja.Id, semana);
        }
    }
}