using System;
using System.Collections.Generic;

namespace BE
{
    public class Disponibilidad
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public DateTime Semana { get; set; }
        public List<FranjaHoraria> Franjas { get; set; } = new List<FranjaHoraria>();
    }
}