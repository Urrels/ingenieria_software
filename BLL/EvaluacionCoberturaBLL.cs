using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    public class ResultadoEvaluacionFranjaVM
    {
        public BE.FranjaHoraria Franja { get; set; }
        public int PersonalAsignado { get; set; }
        public int PersonalPresente { get; set; }
        public int SociosAsistidos { get; set; }
        public string Resultado { get; set; }
    }

    public class EvaluacionCoberturaBLL
    {
        private readonly DAL.DemandaEstimadaDAL _demandaDAL = new DAL.DemandaEstimadaDAL();
        private readonly DAL.TurnoDAL _turnoDAL = new DAL.TurnoDAL();
        private readonly DAL.LiquidacionDAL _liquidacionDAL = new DAL.LiquidacionDAL();
        private readonly GrillaBLL _grillaBLL = new GrillaBLL();
        private readonly FranjaHorariaBLL _franjaBLL = new FranjaHorariaBLL();
        private readonly PresenciaBLL _presenciaBLL = new PresenciaBLL();

        private const int SOCIOS_POR_EMPLEADO = 15;

        public List<ResultadoEvaluacionFranjaVM> EvaluarSemana(DateTime semana)
        {
            var grilla = _grillaBLL.ObtenerConTurnosPorSemana(semana);
            if (grilla == null) return new List<ResultadoEvaluacionFranjaVM>();

            var franjas = _franjaBLL.ListarTodas();
            var resultados = new List<ResultadoEvaluacionFranjaVM>();
            var historialDAL = new DAL.HistorialAsistenciaSociosDAL();

            var fichajes = _presenciaBLL.ObtenerFichajes(semana.AddDays(-1), semana.AddDays(7));
            int maxHorasAbierto = _liquidacionDAL.ObtenerParametros().MaxHorasFichajeAbierto;
            DateTime ahora = Reloj.Ahora;

            foreach (var franjaId in grilla.Turnos.Select(t => t.FranjaId).Distinct())
            {
                var franja = franjas.First(f => f.Id == franjaId);

                DateTime? fecha = DAL.LiquidacionDAL.FechaDeFranja(semana, franja.Dia);
                if (!fecha.HasValue) continue;
                DateTime inicio = fecha.Value + franja.HoraInicio;
                DateTime fin = fecha.Value + franja.HoraFin;
                if (fin <= inicio) fin = fin.AddDays(1);
                if (fin > ahora) continue;

                int asignados = _turnoDAL.ContarAsignadosPorFranja(grilla.Id, franjaId);
                int presentes = PresenciaBLL.EmpleadosPresentes(fichajes, inicio, fin, ahora, maxHorasAbierto, franja.RolRequerido).Count;

                var mediciones = historialDAL.ObtenerPorFranjaYSemana(franjaId, semana);
                int socios = mediciones.Count > 0 ? (int)mediciones.Average() : 0;

                string resultado = Clasificar(presentes, socios);

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
                    PersonalPresente = presentes,
                    SociosAsistidos = socios,
                    Resultado = resultado
                });
            }

            return resultados;
        }

        public static string Clasificar(int personalPresente, int socios)
        {
            int necesario = Math.Max(1, socios / SOCIOS_POR_EMPLEADO);
            if (personalPresente > necesario) return "Exceso";
            if (personalPresente < necesario) return "Falta";
            return "Ajustado";
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
