using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    public class ResultadoEvaluacionFranjaVM
    {
        public BE.FranjaHoraria Franja { get; set; }
        public int PersonalAsignado { get; set; }
        public int SociosAsistidos { get; set; }
        public string Resultado { get; set; }   // "Exceso" | "Falta" | "Ajustado"
    }

    public class EvaluacionCoberturaBLL
    {
        private readonly DAL.DemandaEstimadaDAL _demandaDAL = new DAL.DemandaEstimadaDAL();
        private readonly DAL.TurnoDAL _turnoDAL = new DAL.TurnoDAL();
        private readonly GrillaBLL _grillaBLL = new GrillaBLL();
        private readonly FranjaHorariaBLL _franjaBLL = new FranjaHorariaBLL();

        private const int SOCIOS_POR_EMPLEADO = 15;

        // UC4
        public List<ResultadoEvaluacionFranjaVM> EvaluarSemana(DateTime semana)
        {
            var grilla = _grillaBLL.ObtenerConTurnosPorSemana(semana);
            if (grilla == null) return new List<ResultadoEvaluacionFranjaVM>();

            var franjas = _franjaBLL.ListarTodas();
            var resultados = new List<ResultadoEvaluacionFranjaVM>();

            foreach (var franjaId in grilla.Turnos.Select(t => t.FranjaId).Distinct())
            {
                var franja = franjas.First(f => f.Id == franjaId);

                int asignados = _turnoDAL.ContarAsignadosPorFranja(grilla.Id, franjaId);

                var historialDAL = new DAL.HistorialAsistenciaSociosDAL();
                var mediciones = historialDAL.ObtenerPorFranjaYSemana(franjaId, semana);
                int socios = mediciones.Count > 0 ? (int)mediciones.Average() : 0;

                int necesarioSegunSocios = Math.Max(1, socios / SOCIOS_POR_EMPLEADO);

                string resultado;
                if (asignados > necesarioSegunSocios) resultado = "Exceso";
                else if (asignados < necesarioSegunSocios) resultado = "Falta";
                else resultado = "Ajustado";

                _demandaDAL.InsertarEvaluacionCobertura(new BE.EvaluacionCobertura
                {
                    FranjaId = franjaId,
                    Semana = semana,
                    Resultado = resultado
                });

                resultados.Add(new ResultadoEvaluacionFranjaVM
                {
                    Franja = franja,
                    PersonalAsignado = asignados,
                    SociosAsistidos = socios,
                    Resultado = resultado
                });
            }

            return resultados;
        }

        public Dictionary<string, int> ObtenerResumen()
        {
            var crudo = _demandaDAL.ObtenerResumenCobertura();
            var completo = new Dictionary<string, int> { { "Falta", 0 }, { "Ajustado", 0 }, { "Exceso", 0 } };
            foreach (var kvp in crudo)
                completo[kvp.Key] = kvp.Value;
            return completo;
        }
    }
}