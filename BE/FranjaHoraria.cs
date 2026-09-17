using System;

namespace BE
{
    public class FranjaHoraria
    {
        public int Id { get; set; }
        public DayOfWeek Dia { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
    }
}