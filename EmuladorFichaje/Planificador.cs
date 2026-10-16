using System;
using System.Collections.Generic;
using System.Linq;

namespace EmuladorFichaje
{
    public enum TipoFichaje { Ingreso, Egreso }

    public class EventoFichaje
    {
        public DateTime Cuando { get; set; }
        public int UsuarioId { get; set; }
        public string Empleado { get; set; }
        public TipoFichaje Tipo { get; set; }
        public string Nota { get; set; }
        public bool Omitir { get; set; } // el empleado se olvidó: no se ficha, queda abierto
    }

    // Convierte los turnos asignados en fichajes "humanos": llegan un poco antes o después,
    // a veces faltan, se quedan de más, se van antes o se olvidan de fichar la salida.
    public class Planificador
    {
        private readonly Opciones _o;
        private readonly Random _azar;

        public int Bloques { get; private set; }
        public int Ausencias { get; private set; }
        public int Omitidos { get; private set; }

        public Planificador(Opciones opciones)
        {
            _o = opciones;
            _azar = new Random(opciones.Semilla);
        }

        public List<EventoFichaje> Planificar(DateTime desde, DateTime hasta)
        {
            var turnos = new DAL.LiquidacionDAL().ListarTurnosAsignados(desde, hasta);
            var yaFichados = new BLL.PresenciaBLL().ObtenerFichajes(desde, hasta)
                .Select(f => (f.UsuarioId, f.Fecha.Date)).ToList();
            var eventos = new List<EventoFichaje>();

            foreach (var grupo in turnos.GroupBy(t => new { t.UsuarioId, Dia = t.Fecha.Date }))
            {
                // Franjas contiguas del mismo día (9–13 y 13–17) son un solo bloque de trabajo.
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
            if (Tirada(_o.PorcentajeAusencia))
            {
                Ausencias++;
                yield break;
            }

            // Llegada
            int minutosIngreso;
            string notaIngreso;
            if (Tirada(_o.PorcentajeTardanza)) { minutosIngreso = _azar.Next(12, 36); notaIngreso = "llegó tarde"; }
            else { minutosIngreso = _azar.Next(-10, 6); notaIngreso = null; }

            // Salida
            int minutosEgreso;
            string notaEgreso = null;
            bool olvida = Tirada(_o.PorcentajeOlvidoEgreso);
            if (Tirada(_o.PorcentajeHorasExtra)) { minutosEgreso = _azar.Next(30, 91); notaEgreso = "se quedó de más"; }
            else if (Tirada(_o.PorcentajeSalidaAnticipada)) { minutosEgreso = -_azar.Next(20, 46); notaEgreso = "se fue antes"; }
            else minutosEgreso = _azar.Next(-3, 11);

            DateTime ingreso = inicio.AddMinutes(minutosIngreso);
            DateTime egreso = fin.AddMinutes(minutosEgreso);
            if (egreso <= ingreso.AddMinutes(30)) egreso = ingreso.AddMinutes(30);

            yield return new EventoFichaje { Cuando = ingreso, UsuarioId = usuarioId, Empleado = empleado, Tipo = TipoFichaje.Ingreso, Nota = notaIngreso };
            if (olvida)
                yield return new EventoFichaje { Cuando = egreso, UsuarioId = usuarioId, Empleado = empleado, Tipo = TipoFichaje.Egreso, Nota = "se olvidó de fichar la salida", Omitir = true };
            else
                yield return new EventoFichaje { Cuando = egreso, UsuarioId = usuarioId, Empleado = empleado, Tipo = TipoFichaje.Egreso, Nota = notaEgreso };
        }

        private bool Tirada(int porcentaje) => _azar.Next(100) < porcentaje;
    }
}
