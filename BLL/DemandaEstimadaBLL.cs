using System;
using System.Linq;

namespace BLL
{
    public class DemandaEstimadaBLL
    {
        private readonly DAL.DemandaEstimadaDAL _dal = new DAL.DemandaEstimadaDAL();

        private static readonly double[] PesosSemanales = { 0.4, 0.3, 0.2, 0.1 };

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
                    CantidadPersonalNecesario = 0,
                    OrigenDato = "Manual"
                };
                manual.Id = _dal.Insertar(manual);
                return manual;
            }

            double promedioPonderado = 0;
            for (int i = 0; i < 4; i++)
                promedioPonderado += historial[i] * PesosSemanales[i];

            double tendenciaSemanal = (historial[0] - historial[3]) / 3.0;
            double proyeccion = promedioPonderado + tendenciaSemanal;
            if (proyeccion < 0) proyeccion = promedioPonderado;

            var evalAnterior = ConsultarEvaluacionCobertura(semana.AddDays(-7), franja);

            int personalNecesario = (int)Math.Ceiling(proyeccion / 15.0);
            if (personalNecesario < 1) personalNecesario = 1;

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