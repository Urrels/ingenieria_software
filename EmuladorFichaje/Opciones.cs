using System;
using System.Globalization;

namespace EmuladorFichaje
{
    // Argumentos de la línea de comandos: --nombre valor
    public class Opciones
    {
        public DateTime Desde { get; private set; }
        public DateTime Hasta { get; private set; }
        public double Velocidad { get; private set; } = 3600; // segundos simulados por segundo real
        public int Puerto { get; private set; } = 5060;
        public bool SinApi { get; private set; }
        public bool SinEmpleados { get; private set; }
        public int Semilla { get; private set; } = Environment.TickCount;
        public int PorcentajeAusencia { get; private set; } = 4;
        public int PorcentajeTardanza { get; private set; } = 15;
        public int PorcentajeHorasExtra { get; private set; } = 10;
        public int PorcentajeSalidaAnticipada { get; private set; } = 5;
        public int PorcentajeOlvidoEgreso { get; private set; } = 2;
        public bool Ayuda { get; private set; }

        public static Opciones Leer(string[] args)
        {
            var o = new Opciones();
            DateTime hoy = DateTime.Today;
            DateTime lunesPasado = hoy.AddDays(-(((int)hoy.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7) - 7);
            o.Desde = lunesPasado;
            o.Hasta = lunesPasado.AddDays(6);

            for (int i = 0; i < args.Length; i++)
            {
                string a = args[i].ToLowerInvariant();
                if (a == "--ayuda" || a == "-h" || a == "--help" || a == "/?") { o.Ayuda = true; continue; }
                if (a == "--sin-api") { o.SinApi = true; continue; }
                if (a == "--sin-empleados") { o.SinEmpleados = true; continue; }

                if (i + 1 >= args.Length) throw new ArgumentException("Falta el valor de " + args[i]);
                string v = args[++i];
                switch (a)
                {
                    case "--desde": o.Desde = Fecha(v); break;
                    case "--hasta": o.Hasta = Fecha(v); break;
                    case "--velocidad": o.Velocidad = Numero(v); break;
                    case "--horas-por-segundo": o.Velocidad = Numero(v) * 3600; break;
                    case "--horas-por-minuto": o.Velocidad = Numero(v) * 60; break;
                    case "--puerto": o.Puerto = (int)Numero(v); break;
                    case "--semilla": o.Semilla = (int)Numero(v); break;
                    case "--ausencias": o.PorcentajeAusencia = (int)Numero(v); break;
                    case "--tardanzas": o.PorcentajeTardanza = (int)Numero(v); break;
                    case "--horas-extra": o.PorcentajeHorasExtra = (int)Numero(v); break;
                    case "--salidas-anticipadas": o.PorcentajeSalidaAnticipada = (int)Numero(v); break;
                    case "--olvidos-egreso": o.PorcentajeOlvidoEgreso = (int)Numero(v); break;
                    default: throw new ArgumentException("Opción desconocida: " + args[i]);
                }
            }

            if (o.Hasta < o.Desde) throw new ArgumentException("--hasta no puede ser anterior a --desde.");
            if (o.Velocidad <= 0) throw new ArgumentException("La velocidad debe ser mayor a cero.");
            return o;
        }

        private static DateTime Fecha(string v)
        {
            if (DateTime.TryParseExact(v, new[] { "yyyy-MM-dd", "dd/MM/yyyy" }, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out DateTime f)) return f.Date;
            throw new ArgumentException("Fecha inválida (usar yyyy-MM-dd o dd/MM/yyyy): " + v);
        }

        private static double Numero(string v)
        {
            if (double.TryParse(v.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out double n)) return n;
            throw new ArgumentException("Número inválido: " + v);
        }

        public static string TextoAyuda => @"
Emulador de fichaje — simula un reloj acelerado y empleados que fichan según la grilla.

  EmuladorFichaje.exe [opciones]

Tiempo
  --desde yyyy-MM-dd          primer día a simular   (por defecto: lunes de la semana pasada)
  --hasta yyyy-MM-dd          último día a simular   (por defecto: domingo de esa semana)
  --horas-por-segundo N       1 segundo real = N horas simuladas (por defecto 1)
  --horas-por-minuto N        1 minuto real = N horas simuladas
  --velocidad N               segundos simulados por segundo real (1 = tiempo real)

Comportamiento de los empleados (porcentaje de turnos)
  --ausencias 4  --tardanzas 15  --horas-extra 10  --salidas-anticipadas 5  --olvidos-egreso 2
  --semilla N                 repite exactamente la misma simulación
  --sin-empleados             no ficha nadie solo; queda la API para fichar a mano

API (HTTP, para fichar a mano con la hora simulada)
  --puerto 5060               (por defecto 5060)   --sin-api   no levantar la API
  GET  /api/reloj             hora simulada y velocidad
  POST /api/reloj             {""velocidad"": 7200, ""pausado"": false}   cambia la velocidad / pausa
  POST /api/fichar            {""usuario"": ""ana"", ""tipo"": ""ingreso""}   (tipo: ingreso | egreso)
  GET  /api/estado?usuario=ana

Teclas: P pausa · + / - duplica / reduce la velocidad · Q salir
";
    }
}
