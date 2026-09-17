using System;

namespace BE
{
    public class AlertaRevision
    {
        public int Id { get; set; }
        public int EquipoId { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public string Estado { get; set; }   // "Pendiente" | "Autorizada" | "Descartada"
    }
}