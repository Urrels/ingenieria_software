using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    public static class LiquidacionCalculo
    {
        public static string Hhmm(int minutos)
        {
            int m = Math.Abs(minutos);
            return $"{(minutos < 0 ? "-" : "")}{m / 60}:{m % 60:00}";
        }

        public static decimal Horas(int minutos) => Math.Round(minutos / 60m, 2, MidpointRounding.AwayFromZero);

        public static int Redondear(int minutos, int multiplo)
        {
            if (multiplo <= 0) return minutos;
            return (int)(Math.Round(minutos / (double)multiplo, MidpointRounding.AwayFromZero) * multiplo);
        }

        public static decimal ValorHoraVigente(IEnumerable<BE.ValorHoraEmpleado> valores, int usuarioId, DateTime fecha)
        {
            var vigente = valores
                .Where(v => v.UsuarioId == usuarioId && v.VigenteDesde.Date <= fecha.Date)
                .OrderByDescending(v => v.VigenteDesde)
                .FirstOrDefault();
            return vigente == null ? 0m : vigente.Valor;
        }

        public static List<BE.LiquidacionDia> Calcular(BE.EntradaLiquidacion e, DateTime desde, DateTime hasta)
        {
            BE.ParametrosLiquidacion p = e.Parametros;
            desde = desde.Date;
            hasta = hasta.Date;

            var fichajes = e.Fichajes.Where(f => f.Fecha.Date >= desde && f.Fecha.Date <= hasta).ToList();
            var turnos = e.Turnos.Where(t => t.Fecha.Date >= desde && t.Fecha.Date <= hasta).ToList();

            var nombres = new Dictionary<int, string>();
            foreach (var f in fichajes) nombres[f.UsuarioId] = f.Empleado;
            foreach (var t in turnos) if (!nombres.ContainsKey(t.UsuarioId)) nombres[t.UsuarioId] = t.Empleado;

            var claves = fichajes.Select(f => (UsuarioId: f.UsuarioId, Fecha: f.Fecha.Date))
                .Union(turnos.Select(t => (UsuarioId: t.UsuarioId, Fecha: t.Fecha.Date)))
                .ToList();

            var resultado = new List<BE.LiquidacionDia>();
            foreach (var clave in claves)
            {
                var fichajesDia = fichajes.Where(f => f.UsuarioId == clave.UsuarioId && f.Fecha.Date == clave.Fecha).ToList();
                var turnosDia = turnos.Where(t => t.UsuarioId == clave.UsuarioId && t.Fecha.Date == clave.Fecha).ToList();
                var completos = fichajesDia.Where(f => f.HoraEgreso.HasValue).ToList();

                int crudo = completos.Sum(f => f.MinutosTrabajados);
                int asignados = turnosDia.Sum(t => t.Minutos);
                bool hayIncompleto = fichajesDia.Any(f => !f.HoraEgreso.HasValue);

                if (fichajesDia.Count == 0 && turnosDia.Any(t => t.Fin > e.Ahora)) continue;

                int efectivo = crudo;
                if (asignados > 0 && Math.Abs(crudo - asignados) <= p.ToleranciaMinutos) efectivo = asignados;
                efectivo = Redondear(efectivo, p.RedondeoMinutos);

                int excedente = asignados > 0 ? Math.Max(0, efectivo - asignados) : efectivo;
                bool aprobado = excedente > 0 && e.ExcedentesAprobados.Contains((clave.UsuarioId, clave.Fecha));
                int pagables = Math.Min(efectivo, asignados) + (aprobado ? excedente : 0);

                bool ausente = asignados > 0 && crudo == 0 && !hayIncompleto;

                bool esFeriado = clave.Fecha.DayOfWeek == DayOfWeek.Sunday || e.Feriados.Contains(clave.Fecha);
                int normales = 0, extra = 0, feriado = 0;
                if (esFeriado)
                {
                    feriado = pagables;
                }
                else
                {
                    int jornada = (int)Math.Round(p.JornadaNormalHoras * 60);
                    normales = Math.Min(pagables, jornada);
                    extra = pagables - normales;
                }

                decimal valor = ValorHoraVigente(e.ValoresHora, clave.UsuarioId, clave.Fecha);
                decimal importe = Math.Round(valor *
                    (normales + extra * (1 + p.RecargoExtraPct / 100m) + feriado * (1 + p.RecargoFeriadoPct / 100m)) / 60m,
                    2, MidpointRounding.AwayFromZero);

                var observaciones = new List<string>();
                if (hayIncompleto) observaciones.Add("Fichaje incompleto");
                if (ausente) observaciones.Add("Ausente");
                if (asignados == 0 && efectivo > 0) observaciones.Add("Sin turno asignado");
                if (excedente > 0)
                    observaciones.Add(aprobado ? "Excedente aprobado " + Hhmm(excedente) : "Excedente a aprobar " + Hhmm(excedente));
                if (esFeriado && pagables > 0) observaciones.Add("Domingo o feriado");
                if (fichajesDia.Any(f => f.Corregido)) observaciones.Add("Fichaje corregido");
                if (valor == 0m && pagables > 0) observaciones.Add("Sin valor hora");
                string observacion = string.Join("; ", observaciones);
                if (observacion.Length > 200) observacion = observacion.Substring(0, 200);

                resultado.Add(new BE.LiquidacionDia
                {
                    UsuarioId = clave.UsuarioId,
                    Empleado = nombres[clave.UsuarioId],
                    Fecha = clave.Fecha,
                    Ingreso = completos.Count == 0 ? (TimeSpan?)null : completos.Min(f => f.HoraIngreso),
                    Egreso = completos.Count == 0 ? (TimeSpan?)null : completos.OrderBy(f => f.Egreso).Last().HoraEgreso,
                    MinTrabajados = crudo,
                    MinAsignados = asignados,
                    MinPagables = pagables,
                    MinNormales = normales,
                    MinExtra = extra,
                    MinFeriado = feriado,
                    MinExcedente = excedente,
                    ExcedenteAprobado = aprobado,
                    Ausente = ausente,
                    ValorHora = valor,
                    RecargoExtraPct = p.RecargoExtraPct,
                    RecargoFeriadoPct = p.RecargoFeriadoPct,
                    Importe = importe,
                    Observacion = observacion.Length == 0 ? null : observacion
                });
            }

            return resultado.OrderBy(d => d.Empleado).ThenBy(d => d.Fecha).ToList();
        }

        public static List<BE.LiquidacionResumenEmpleado> Resumir(IEnumerable<BE.LiquidacionDia> dias)
        {
            return dias.GroupBy(d => d.UsuarioId)
                .Select(g => new BE.LiquidacionResumenEmpleado
                {
                    UsuarioId = g.Key,
                    Empleado = g.First().Empleado,
                    MinTrabajados = g.Sum(d => d.MinTrabajados),
                    MinAsignados = g.Sum(d => d.MinAsignados),
                    MinPagables = g.Sum(d => d.MinPagables),
                    MinNormales = g.Sum(d => d.MinNormales),
                    MinExtra = g.Sum(d => d.MinExtra),
                    MinFeriado = g.Sum(d => d.MinFeriado),
                    MinExcedentePendiente = g.Sum(d => d.MinExcedentePendiente),
                    Ausencias = g.Count(d => d.Ausente),
                    Importe = g.Sum(d => d.Importe)
                })
                .OrderBy(r => r.Empleado)
                .ToList();
        }
    }
}
