using System;

namespace BE
{
    public class RegistroAsistenciaPersonal
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan HoraIngreso { get; set; }
        public TimeSpan? HoraEgreso { get; set; }
    }
}