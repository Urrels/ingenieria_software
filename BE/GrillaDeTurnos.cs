using System;
using System.Collections.Generic;

namespace BE
{
    public class GrillaDeTurnos
    {
        public int Id { get; set; }
        public DateTime Semana { get; set; }
        public string Estado { get; set; }   // "Propuesta" | "Confirmada" | "Comunicada"
        public int AdministradorId { get; set; }
        public List<Turno> Turnos { get; set; } = new List<Turno>();
    }
}