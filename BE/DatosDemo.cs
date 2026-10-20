using System.Collections.Generic;

namespace BE
{
    // Contenido de datos_demo.json: lo que se carga para no mostrar el sistema vacío.
    public class DatosDemo
    {
        public int Version { get; set; }
        public string ContrasenaEmpleados { get; set; }
        public int Semilla { get; set; } = 2026;
        public int SemanasConGrilla { get; set; } = 3;
        public ComportamientoDemo Comportamiento { get; set; } = new ComportamientoDemo();
        public List<EmpleadoDemo> Empleados { get; set; } = new List<EmpleadoDemo>();
        public List<FranjaDemo> Franjas { get; set; } = new List<FranjaDemo>();
        public List<EquipoDemo> Equipos { get; set; } = new List<EquipoDemo>();
        public List<FeriadoDemo> Feriados { get; set; } = new List<FeriadoDemo>();
    }

    public class ComportamientoDemo
    {
        public int Ausencias { get; set; } = 4;
        public int Tardanzas { get; set; } = 15;
        public int HorasExtra { get; set; } = 10;
        public int SalidasAnticipadas { get; set; } = 5;
    }

    public class EmpleadoDemo
    {
        public string Usuario { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Rol { get; set; }
        public decimal ValorHora { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }

        // "Manana" o "Tarde": solo declara disponibilidad en franjas de ese turno ("" = cualquiera).
        public string Turno { get; set; }
    }

    public class FranjaDemo
    {
        public string Dia { get; set; }
        public string Inicio { get; set; }
        public string Fin { get; set; }
        public string Rol { get; set; }

        // Socios que asistieron en esa franja, de la semana más antigua a la más reciente.
        public List<int> Socios { get; set; } = new List<int>();
    }

    public class EquipoDemo
    {
        public string Nombre { get; set; }
        public int Uso { get; set; }
        public int Critico { get; set; }

        // Incrementos de uso de los últimos días (el último es el de ayer).
        public List<int> UsoDiario { get; set; } = new List<int>();
    }

    public class FeriadoDemo
    {
        public int SemanasAtras { get; set; }
        public string Dia { get; set; }
        public string Descripcion { get; set; }
    }

    public class ResultadoCargaDemo
    {
        public int Empleados { get; set; }
        public int Franjas { get; set; }
        public int Grillas { get; set; }
        public int Fichajes { get; set; }
        public int Equipos { get; set; }
        public int Evaluaciones { get; set; }
        public int DisponibilidadesProximaSemana { get; set; }
        public string ContrasenaEmpleados { get; set; }
    }

    public class DatosNegocio
    {
        public int Franjas { get; set; }
        public int Equipos { get; set; }
        public int Grillas { get; set; }
        public int Usuarios { get; set; }

        public bool SistemaVacio => Franjas == 0 && Equipos == 0 && Grillas == 0 && Usuarios <= 1;
    }
}
