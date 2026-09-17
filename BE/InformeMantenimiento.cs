using System;

namespace BE
{
    public class InformeMantenimiento
    {
        public int Id { get; set; }
        public int VisitaId { get; set; }
        public string Resultado { get; set; }
        public bool PendienteRepuesto { get; set; }
        public DateTime FechaEmision { get; set; }
    }
}