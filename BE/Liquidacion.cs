using System;
using System.Collections.Generic;

namespace BE
{
    public enum ResultadoFichaje { Ok, IngresoAbierto, SinIngreso, IngresoVencido }

    public class EstadoFichaje
    {
        public RegistroAsistenciaPersonal Abierto { get; set; }
        public bool Vencido { get; set; }
    }

    public class ParametrosLiquidacion
    {
        public int ToleranciaMinutos { get; set; } = 10;
        public int RedondeoMinutos { get; set; } = 15;
        public decimal JornadaNormalHoras { get; set; } = 8;
        public decimal RecargoExtraPct { get; set; } = 50;
        public decimal RecargoFeriadoPct { get; set; } = 100;
        public int MaxHorasFichajeAbierto { get; set; } = 16;
    }

    public class ValorHoraEmpleado
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Empleado { get; set; }
        public decimal Valor { get; set; }
        public DateTime VigenteDesde { get; set; }
    }

    public class Feriado
    {
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; }
    }

    public class EmpleadoLiquidable
    {
        public int Id { get; set; }
        public string Usuario { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }

        public string NombreCompleto =>
            string.IsNullOrWhiteSpace(Nombre + Apellido) ? Usuario : (Apellido + ", " + Nombre).Trim(' ', ',');

        public override string ToString() => NombreCompleto;
    }

    public class LiquidacionDia
    {
        public int UsuarioId { get; set; }
        public string Empleado { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan? Ingreso { get; set; }
        public TimeSpan? Egreso { get; set; }
        public int MinTrabajados { get; set; }
        public int MinAsignados { get; set; }
        public int MinPagables { get; set; }
        public int MinNormales { get; set; }
        public int MinExtra { get; set; }
        public int MinFeriado { get; set; }
        public int MinExcedente { get; set; }
        public bool ExcedenteAprobado { get; set; }
        public bool Ausente { get; set; }
        public decimal ValorHora { get; set; }
        public decimal RecargoExtraPct { get; set; }
        public decimal RecargoFeriadoPct { get; set; }
        public decimal Importe { get; set; }
        public string Observacion { get; set; }

        public int MinExcedentePendiente => ExcedenteAprobado ? 0 : MinExcedente;
    }

    public class LiquidacionResumenEmpleado
    {
        public int UsuarioId { get; set; }
        public string Empleado { get; set; }
        public int MinTrabajados { get; set; }
        public int MinAsignados { get; set; }
        public int MinPagables { get; set; }
        public int MinNormales { get; set; }
        public int MinExtra { get; set; }
        public int MinFeriado { get; set; }
        public int MinExcedentePendiente { get; set; }
        public int Ausencias { get; set; }
        public decimal Importe { get; set; }
    }

    public class Liquidacion
    {
        public int Id { get; set; }
        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }
        public string Estado { get; set; }
        public string CerradaPor { get; set; }
        public DateTime FechaCierre { get; set; }
        public string PagadaPor { get; set; }
        public DateTime? FechaPago { get; set; }

        public override string ToString() =>
            $"{Desde:dd/MM/yyyy} - {Hasta:dd/MM/yyyy} ({Estado})";
    }

    public class TurnoAsignadoFecha
    {
        public int UsuarioId { get; set; }
        public string Empleado { get; set; }
        public int FranjaId { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }

        public DateTime Inicio => Fecha.Date + HoraInicio;

        public DateTime Fin
        {
            get
            {
                DateTime fin = Fecha.Date + HoraFin;
                return fin <= Inicio ? fin.AddDays(1) : fin;
            }
        }

        public int Minutos => (int)Math.Round((Fin - Inicio).TotalMinutes);
    }

    public class EntradaLiquidacion
    {
        public List<RegistroAsistenciaPersonal> Fichajes { get; set; } = new List<RegistroAsistenciaPersonal>();
        public List<TurnoAsignadoFecha> Turnos { get; set; } = new List<TurnoAsignadoFecha>();
        public List<ValorHoraEmpleado> ValoresHora { get; set; } = new List<ValorHoraEmpleado>();
        public HashSet<DateTime> Feriados { get; set; } = new HashSet<DateTime>();
        public HashSet<(int usuarioId, DateTime fecha)> ExcedentesAprobados { get; set; } = new HashSet<(int, DateTime)>();
        public ParametrosLiquidacion Parametros { get; set; } = new ParametrosLiquidacion();
        public DateTime Ahora { get; set; } = DateTime.Now;
    }
}
