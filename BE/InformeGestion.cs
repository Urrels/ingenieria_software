using System;
using System.Collections.Generic;

namespace BE
{
    public class InformeGestion
    {
        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }
        public DateTime GeneradoEn { get; set; }
        public string GeneradoPor { get; set; }

        public decimal HorasTrabajadas { get; set; }
        public decimal HorasAsignadas { get; set; }
        public decimal HorasAPagar { get; set; }
        public decimal HorasExtra { get; set; }
        public decimal HorasDomingoFeriado { get; set; }
        public decimal HorasExcedentePendiente { get; set; }
        public int Ausencias { get; set; }
        public int TurnosPlanificados { get; set; }
        public decimal CostoTotal { get; set; }
        public int SociosAtendidos { get; set; }

        public decimal CumplimientoPct { get; set; }
        public decimal ExtraPct { get; set; }
        public decimal AusentismoPct { get; set; }
        public decimal CostoPorSocio { get; set; }

        public List<InformeEmpleado> Empleados { get; set; } = new List<InformeEmpleado>();
        public List<InformeCobertura> Cobertura { get; set; } = new List<InformeCobertura>();
        public List<InformeEquipo> Equipos { get; set; } = new List<InformeEquipo>();
        public int AlertasPendientes { get; set; }
        public List<InformeConclusion> Conclusiones { get; set; } = new List<InformeConclusion>();
    }

    public class InformeEmpleado
    {
        public string Empleado { get; set; }
        public decimal HorasTrabajadas { get; set; }
        public decimal HorasAsignadas { get; set; }
        public decimal HorasAPagar { get; set; }
        public decimal HorasExtra { get; set; }
        public int Ausencias { get; set; }
        public decimal Importe { get; set; }
    }

    public class InformeCobertura
    {
        public string Franja { get; set; }
        public string Rol { get; set; }
        public int Semanas { get; set; }
        public int Faltas { get; set; }
        public int Ajustadas { get; set; }
        public int Excesos { get; set; }
        public int SociosPromedio { get; set; }
    }

    public class InformeEquipo
    {
        public string Nombre { get; set; }
        public string Estado { get; set; }
        public int Uso { get; set; }
        public int Critico { get; set; }
        public int UsoPct { get; set; }
        public int? DiasHastaCritico { get; set; }
    }

    public class InformeConclusion
    {
        public string Area { get; set; }
        public string Prioridad { get; set; }
        public string Texto { get; set; }
    }

    public class EvaluacionFranjaSemana
    {
        public DateTime Semana { get; set; }
        public int FranjaId { get; set; }
        public string Dia { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public string Rol { get; set; }
        public string Resultado { get; set; }
    }

    public class SociosEnFecha
    {
        public int FranjaId { get; set; }
        public DateTime Fecha { get; set; }
        public int Cantidad { get; set; }
    }
}
