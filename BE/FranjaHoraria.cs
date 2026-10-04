using System;

namespace BE
{
    public class FranjaHoraria
    {
        public int Id { get; set; }
        public string Dia { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public string RolRequerido { get; set; }
    }
}