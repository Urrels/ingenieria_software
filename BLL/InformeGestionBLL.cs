using Newtonsoft.Json;
using SeguridadYServicios;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace BLL
{
    public class InformeGestionBLL
    {
        private static readonly CultureInfo Es = new CultureInfo("es-AR");

        private readonly LiquidacionBLL _liquidacionBLL = new LiquidacionBLL();
        private readonly DAL.GestionDAL _gestionDAL = new DAL.GestionDAL();
        private readonly DAL.EquipoDAL _equipoDAL = new DAL.EquipoDAL();
        private readonly EquipoBLL _equipoBLL = new EquipoBLL();
        private readonly AlertaRevisionBLL _alertaBLL = new AlertaRevisionBLL();
        private readonly FranjaHorariaBLL _franjaBLL = new FranjaHorariaBLL();
        private readonly BitacoraBLL _bitacora = new BitacoraBLL();

        public BE.InformeGestion Generar(DateTime desde, DateTime hasta)
        {
            if (desde.Date > hasta.Date)
                throw new InvalidOperationException("La fecha desde no puede ser posterior a la fecha hasta.");
            if ((hasta.Date - desde.Date).TotalDays > 183)
                throw new InvalidOperationException("El informe no puede abarcar más de 6 meses.");

            var informe = new BE.InformeGestion
            {
                Desde = desde.Date,
                Hasta = hasta.Date,
                GeneradoEn = Reloj.Ahora,
                GeneradoPor = SessionManager.getInstance().getUsuario()?.Usuario
            };

            CompletarHoras(informe);
            CompletarCobertura(informe);
            CompletarEquipos(informe);
            CompletarConclusiones(informe);
            return informe;
        }

        private void CompletarHoras(BE.InformeGestion i)
        {
            var dias = _liquidacionBLL.Calcular(i.Desde, i.Hasta);
            var resumen = _liquidacionBLL.Resumir(dias);

            i.HorasTrabajadas = H(resumen.Sum(r => r.MinTrabajados));
            i.HorasAsignadas = H(resumen.Sum(r => r.MinAsignados));
            i.HorasAPagar = H(resumen.Sum(r => r.MinPagables));
            i.HorasExtra = H(resumen.Sum(r => r.MinExtra));
            i.HorasDomingoFeriado = H(resumen.Sum(r => r.MinFeriado));
            i.HorasExcedentePendiente = H(resumen.Sum(r => r.MinExcedentePendiente));
            i.Ausencias = resumen.Sum(r => r.Ausencias);
            i.TurnosPlanificados = dias.Count(d => d.MinAsignados > 0);
            i.CostoTotal = resumen.Sum(r => r.Importe);

            i.CumplimientoPct = i.HorasAsignadas == 0 ? 0 : Math.Round(Math.Min(i.HorasTrabajadas, i.HorasAsignadas) * 100m / i.HorasAsignadas, 1);
            i.ExtraPct = i.HorasAPagar == 0 ? 0 : Math.Round((i.HorasExtra + i.HorasDomingoFeriado) * 100m / i.HorasAPagar, 1);
            i.AusentismoPct = i.TurnosPlanificados == 0 ? 0 : Math.Round(i.Ausencias * 100m / i.TurnosPlanificados, 1);

            i.Empleados = resumen.OrderByDescending(r => r.Importe).Select(r => new BE.InformeEmpleado
            {
                Empleado = r.Empleado,
                HorasTrabajadas = H(r.MinTrabajados),
                HorasAsignadas = H(r.MinAsignados),
                HorasAPagar = H(r.MinPagables),
                HorasExtra = H(r.MinExtra + r.MinFeriado),
                Ausencias = r.Ausencias,
                Importe = r.Importe
            }).ToList();
        }

        private void CompletarCobertura(BE.InformeGestion i)
        {
            DateTime primerLunes = LunesDe(i.Desde);
            var franjas = _franjaBLL.ListarTodas().ToDictionary(f => f.Id);
            var evaluaciones = _gestionDAL.ListarEvaluaciones(primerLunes, i.Hasta);
            var socios = _gestionDAL.ListarSocios(i.Desde, i.Hasta);
            i.SociosAtendidos = socios.Sum(s => s.Cantidad);

            i.Cobertura = evaluaciones.GroupBy(e => e.FranjaId).Select(g =>
            {
                var f = g.First();
                var delasFranja = socios.Where(s => s.FranjaId == g.Key).Select(s => s.Cantidad).ToList();
                return new BE.InformeCobertura
                {
                    Franja = $"{NombreDia(f.Dia)} {f.HoraInicio:hh\\:mm}-{f.HoraFin:hh\\:mm}",
                    Rol = f.Rol,
                    Semanas = g.Count(),
                    Faltas = g.Count(e => e.Resultado == "Falta"),
                    Ajustadas = g.Count(e => e.Resultado == "Ajustado"),
                    Excesos = g.Count(e => e.Resultado == "Exceso"),
                    SociosPromedio = delasFranja.Count == 0 ? 0 : (int)Math.Round(delasFranja.Average())
                };
            }).OrderByDescending(c => c.Faltas).ThenByDescending(c => c.Excesos).ThenBy(c => c.Franja).ToList();

            i.CostoPorSocio = i.SociosAtendidos == 0 ? 0 : Math.Round(i.CostoTotal / i.SociosAtendidos, 2);
        }

        private void CompletarEquipos(BE.InformeGestion i)
        {
            i.AlertasPendientes = _alertaBLL.ListarPendientes().Count;
            i.Equipos = _equipoDAL.ListarTodos().Select(e => new BE.InformeEquipo
            {
                Nombre = e.Nombre,
                Estado = e.Estado,
                Uso = e.UsoAcumulado,
                Critico = e.NivelUsoCritico ?? 0,
                UsoPct = e.NivelUsoCritico.HasValue && e.NivelUsoCritico.Value > 0
                    ? (int)Math.Round(e.UsoAcumulado * 100.0 / e.NivelUsoCritico.Value) : 0,
                DiasHastaCritico = _equipoBLL.EstimarDiasHastaCritico(e)
            }).OrderByDescending(e => e.UsoPct).ToList();
        }

        private static void CompletarConclusiones(BE.InformeGestion i)
        {
            var c = i.Conclusiones;

            foreach (var f in i.Cobertura.Where(f => f.Semanas >= 2 && f.Faltas * 2 >= f.Semanas).Take(3))
                c.Add(Conclusion("Cobertura", "Alta",
                    $"La franja {f.Franja} ({f.Rol}) tuvo personal insuficiente en {f.Faltas} de {f.Semanas} semanas " +
                    $"(promedio de {f.SociosPromedio} socios). Conviene reforzarla con un empleado más."));

            foreach (var f in i.Cobertura.Where(f => f.Semanas >= 2 && f.Excesos * 2 >= f.Semanas).Take(2))
                c.Add(Conclusion("Cobertura", "Media",
                    $"La franja {f.Franja} ({f.Rol}) tuvo exceso de personal en {f.Excesos} de {f.Semanas} semanas: " +
                    "se puede redistribuir ese turno hacia las franjas con faltante."));

            if (i.AusentismoPct >= 10)
                c.Add(Conclusion("Asistencia", "Alta",
                    $"El ausentismo fue de {i.AusentismoPct.ToString("0.#", Es)}% de los turnos planificados ({i.Ausencias} ausencias). " +
                    "Revisar los patrones por empleado y prever suplentes."));
            else if (i.Ausencias > 0)
                c.Add(Conclusion("Asistencia", "Informativa",
                    $"Hubo {i.Ausencias} ausencia(s) ({i.AusentismoPct.ToString("0.#", Es)}% de los turnos): dentro de lo esperable."));

            if (i.ExtraPct >= 15)
                c.Add(Conclusion("Costos", "Alta",
                    $"Las horas con recargo (extra, domingos y feriados) son el {i.ExtraPct.ToString("0.#", Es)}% de lo pagado. " +
                    "Conviene replanificar turnos para bajar ese costo."));

            if (i.HorasExcedentePendiente > 0)
                c.Add(Conclusion("Costos", "Media",
                    $"Hay {i.HorasExcedentePendiente.ToString("0.##", Es)} h fichadas de más sin aprobar. Decidir si se pagan o se corrigen los turnos."));

            if (i.CumplimientoPct > 0 && i.CumplimientoPct < 90)
                c.Add(Conclusion("Asistencia", "Media",
                    $"Se cumplió el {i.CumplimientoPct.ToString("0.#", Es)}% de las horas planificadas: hay turnos sin cubrir."));

            var peorEmpleado = i.Empleados.OrderByDescending(e => e.Ausencias).FirstOrDefault(e => e.Ausencias >= 2);
            if (peorEmpleado != null)
                c.Add(Conclusion("Asistencia", "Media",
                    $"{peorEmpleado.Empleado} acumula {peorEmpleado.Ausencias} ausencias en el período."));

            foreach (var e in i.Equipos.Where(e => e.Critico > 0 && e.UsoPct >= 90 && e.Estado == "Operativo").Take(3))
                c.Add(Conclusion("Mantenimiento", e.UsoPct >= 100 ? "Alta" : "Media",
                    $"El equipo {e.Nombre} está al {e.UsoPct}% de su nivel crítico" +
                    (e.UsoPct < 100 && e.DiasHastaCritico.HasValue && e.DiasHastaCritico.Value > 0
                        ? $" (se alcanzaría en unos {e.DiasHastaCritico.Value} días)" : e.UsoPct >= 100 ? " (ya lo superó)" : "") +
                    ". Programar la revisión técnica."));

            if (i.AlertasPendientes > 0)
                c.Add(Conclusion("Mantenimiento", "Alta",
                    $"Hay {i.AlertasPendientes} alerta(s) de revisión de equipos pendiente(s) de autorizar."));

            if (c.Count == 0)
                c.Add(Conclusion("General", "Informativa", "No se detectaron desvíos relevantes en el período."));
        }

        public static string ADatosJson(BE.InformeGestion informe) =>
            JsonConvert.SerializeObject(informe, Formatting.Indented);

        public static BE.InformeGestion DesdeJson(string json) =>
            JsonConvert.DeserializeObject<BE.InformeGestion>(json);

        public static void GuardarJson(BE.InformeGestion informe, string ruta) =>
            File.WriteAllText(ruta, ADatosJson(informe), new UTF8Encoding(false));

        public void RegistrarExportacion(string formato)
        {
            string usuario = SessionManager.getInstance().getUsuario()?.Usuario ?? "sistema";
            _bitacora.RegistrarAccion(usuario, "INFORME_GESTION_EXPORTADO:" + formato);
        }

        private static BE.InformeConclusion Conclusion(string area, string prioridad, string texto) =>
            new BE.InformeConclusion { Area = area, Prioridad = prioridad, Texto = texto };

        private static decimal H(int minutos) => LiquidacionCalculo.Horas(minutos);

        private static DateTime LunesDe(DateTime fecha) =>
            fecha.Date.AddDays(-(((int)fecha.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7));

        private static string NombreDia(string dia) =>
            Enum.TryParse(dia, true, out DayOfWeek d) ? Es.DateTimeFormat.GetDayName(d) : dia;
    }
}
