using BLL;
using System;
using System.Diagnostics;
using System.Threading;

namespace EmuladorFichaje
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Opciones o;
            try { o = Opciones.Leer(args); }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Console.WriteLine(Opciones.TextoAyuda);
                return 2;
            }
            if (o.Ayuda) { Console.WriteLine(Opciones.TextoAyuda); return 0; }

            Simulador sim;
            try
            {
                var planificador = new PlanificadorFichajes(new ProbabilidadesFichaje
                {
                    Ausencia = o.PorcentajeAusencia, Tardanza = o.PorcentajeTardanza, HorasExtra = o.PorcentajeHorasExtra,
                    SalidaAnticipada = o.PorcentajeSalidaAnticipada, OlvidoEgreso = o.PorcentajeOlvidoEgreso
                }, o.Semilla);
                var eventos = o.SinEmpleados
                    ? new System.Collections.Generic.List<EventoFichaje>()
                    : planificador.Planificar(o.Desde, o.Hasta);

                Console.WriteLine("Emulador de fichaje");
                Console.WriteLine($"  Período simulado : {o.Desde:dd/MM/yyyy} al {o.Hasta:dd/MM/yyyy}");
                Console.WriteLine($"  Velocidad        : 1 segundo real = {Descripcion(o.Velocidad)}");
                if (!o.SinEmpleados)
                    Console.WriteLine($"  Turnos           : {planificador.Bloques} bloques, {planificador.Ausencias} ausencias, " +
                                      $"{planificador.Omitidos} omitidos (ya tenían fichajes), {eventos.Count} fichajes a registrar (semilla {o.Semilla})");
                if (!o.SinEmpleados && planificador.Bloques == 0)
                    Console.WriteLine("  ATENCIÓN: no hay turnos asignados en una grilla confirmada para ese período; solo funciona la API.");

                sim = new Simulador(o.Desde, o.Hasta.AddDays(1), o.Velocidad, eventos);
            }
            catch (Exception ex)
            {
                Console.WriteLine("No se pudo preparar la simulación: " + ex.Message);
                return 1;
            }

            sim.Mensaje += Escribir;

            ApiFichaje api = null;
            if (!o.SinApi)
            {
                try
                {
                    api = new ApiFichaje(sim, o.Puerto);
                    api.Mensaje += Escribir;
                    api.Iniciar();
                    Console.WriteLine($"  API              : http://localhost:{o.Puerto}/api/  (reloj, fichar, estado)");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("  No se pudo iniciar la API: " + ex.Message);
                    api = null;
                }
            }
            Console.WriteLine("  Teclas           : P pausa · + / - velocidad · Q salir");
            Console.WriteLine();

            bool avisoFin = false;
            var cronometro = Stopwatch.StartNew();
            TimeSpan anterior = TimeSpan.Zero;
            while (true)
            {
                if (!Console.IsInputRedirected && Console.KeyAvailable)
                {
                    char tecla = char.ToLowerInvariant(Console.ReadKey(true).KeyChar);
                    if (tecla == 'q') break;
                    if (tecla == 'p') { sim.Pausar(!sim.Pausado); Escribir(sim.Pausado ? "— pausado —" : "— reanudado —"); }
                    if (tecla == '+') { sim.CambiarVelocidad(sim.Velocidad * 2); Escribir("velocidad: 1 s = " + Descripcion(sim.Velocidad)); }
                    if (tecla == '-') { sim.CambiarVelocidad(Math.Max(1, sim.Velocidad / 2)); Escribir("velocidad: 1 s = " + Descripcion(sim.Velocidad)); }
                }

                TimeSpan ahora = cronometro.Elapsed;
                try { sim.Avanzar(ahora - anterior); }
                catch (Exception ex) { Escribir("Error al fichar: " + ex.Message); }
                anterior = ahora;

                if (sim.Terminada && !avisoFin)
                {
                    avisoFin = true;
                    Escribir($"Simulación terminada: {sim.Registrados} fichajes registrados, {sim.Rechazados} rechazados.");
                    Escribir(api != null ? "La API sigue activa con el reloj detenido. Q para salir." : "Q para salir.");
                    if (api == null) break;
                }
                Thread.Sleep(50);
            }

            api?.Detener();
            return 0;
        }

        private static readonly object CandadoConsola = new object();

        private static void Escribir(string linea)
        {
            lock (CandadoConsola) Console.WriteLine(linea);
        }

        private static string Descripcion(double segundos)
        {
            if (segundos >= 3600) return $"{segundos / 3600:0.##} hora(s) simulada(s)";
            if (segundos >= 60) return $"{segundos / 60:0.##} minuto(s) simulado(s)";
            return $"{segundos:0.##} segundo(s) simulado(s)";
        }
    }
}
