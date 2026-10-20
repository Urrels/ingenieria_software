using System;

namespace BE
{
    public class RegistroAsistenciaPersonal
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Empleado { get; set; }
        public string Rol { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan HoraIngreso { get; set; }
        public TimeSpan? HoraEgreso { get; set; }
        public bool Corregido { get; set; }
        public string MotivoCorreccion { get; set; }

        public DateTime Ingreso => Fecha.Date + HoraIngreso;

        // Un egreso anterior al ingreso es un turno que cruza la medianoche.
        public DateTime? Egreso
        {
            get
            {
                if (!HoraEgreso.HasValue) return null;
                DateTime egreso = Fecha.Date + HoraEgreso.Value;
                return egreso < Ingreso ? egreso.AddDays(1) : egreso;
            }
        }

        public int MinutosTrabajados => Egreso.HasValue ? (int)Math.Round((Egreso.Value - Ingreso).TotalMinutes) : 0;
    }
}
