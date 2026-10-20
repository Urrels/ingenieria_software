using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    public enum TipoFichaje { Ingreso, Egreso }

    public class EventoFichaje
    {
        public DateTime Cuando { get; set; }
        public int UsuarioId { get; set; }
        public string Empleado { get; set; }
        public TipoFichaje Tipo { get; set; }
        public string Nota { get; set; }
        public bool Omitir { get; set; }
    }

    public class ProbabilidadesFichaje
    {
        public int Ausencia { get; set; } = 4;
        public int Tardanza { get; set; } = 15;
        public int HorasExtra { get; set; } = 10;
        public int SalidaAnticipada { get; set; } = 5;
        public int OlvidoEgreso { get; set; } = 2;
    }

    public class PlanificadorFichajes
    {
        private readonly ProbabilidadesFichaje _p;
        private readonly Random _azar;

        public int Bloques { get; private set; }
        public int Ausencias { get; private set; }
        public int Omitidos { get; private set; }

        public PlanificadorFichajes(ProbabilidadesFichaje probabilidades, int semilla)
        {
            _p = probabilidades;
            _azar = new Random(semilla);
        }

        public List<EventoFichaje> Planificar(DateTime desde, DateTime hasta)
        {
            var turnos = new DAL.LiquidacionDAL().ListarTurnosAsignados(desde, hasta);
            var yaFichados = new PresenciaBLL().ObtenerFichajes(desde, hasta)
                .Select(f => (f.UsuarioId, f.Fecha.Date)).ToList();
            var eventos = new List<EventoFichaje>();

            foreach (var grupo in turnos.GroupBy(t => new { t.UsuarioId, Dia = t.Fecha.Date }))
            {
                foreach (var bloque in Unir(grupo.OrderBy(t => t.Inicio).ToList()))
                {
                    Bloques++;
                    if (yaFichados.Contains((grupo.Key.UsuarioId, grupo.Key.Dia)))
                    {
                        Omitidos++;
                        continue;
                    }
                    eventos.AddRange(Simular(grupo.Key.UsuarioId, grupo.First().Empleado, bloque.Item1, bloque.Item2));
                }
            }
            return eventos.OrderBy(e => e.Cuando).ToList();
        }

        private static List<Tuple<DateTime, DateTime>> Unir(List<BE.TurnoAsignadoFecha> turnos)
        {
            var bloques = new List<Tuple<DateTime, DateTime>>();
            foreach (var t in turnos)
            {
                if (bloques.Count > 0 && t.Inicio <= bloques[bloques.Count - 1].Item2)
                {
                    var anterior = bloques[bloques.Count - 1];
                    bloques[bloques.Count - 1] = Tuple.Create(anterior.Item1, t.Fin > anterior.Item2 ? t.Fin : anterior.Item2);
                }
                else bloques.Add(Tuple.Create(t.Inicio, t.Fin));
            }
            return bloques;
        }

        private IEnumerable<EventoFichaje> Simular(int usuarioId, string empleado, DateTime inicio, DateTime fin)
        {
            if (Tirada(_p.Ausencia))
            {
                Ausencias++;
                yield break;
            }

            int minutosIngreso;
            string notaIngreso = null;
            if (Tirada(_p.Tardanza)) { minutosIngreso = _azar.Next(12, 36); notaIngreso = "llegó tarde"; }
            else minutosIngreso = _azar.Next(-10, 6);

            int minutosEgreso;
            string notaEgreso = null;
            bool olvida = Tirada(_p.OlvidoEgreso);
            if (Tirada(_p.HorasExtra)) { minutosEgreso = _azar.Next(30, 91); notaEgreso = "se quedó de más"; }
            else if (Tirada(_p.SalidaAnticipada)) { minutosEgreso = -_azar.Next(20, 46); notaEgreso = "se fue antes"; }
            else minutosEgreso = _azar.Next(-3, 11);

            DateTime ingreso = inicio.AddMinutes(minutosIngreso);
            DateTime egreso = fin.AddMinutes(minutosEgreso);
            if (egreso <= ingreso.AddMinutes(30)) egreso = ingreso.AddMinutes(30);

            yield return new EventoFichaje { Cuando = ingreso, UsuarioId = usuarioId, Empleado = empleado, Tipo = TipoFichaje.Ingreso, Nota = notaIngreso };
            yield return new EventoFichaje
            {
                Cuando = egreso, UsuarioId = usuarioId, Empleado = empleado, Tipo = TipoFichaje.Egreso,
                Nota = olvida ? "se olvidó de fichar la salida" : notaEgreso, Omitir = olvida
            };
        }

        private bool Tirada(int porcentaje) => _azar.Next(100) < porcentaje;
    }
}
