using System;

namespace BE
{
    public class AlertaRevision
    {
        public int Id { get; set; }
        public int EquipoId { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public string Estado { get; set; }   // "Pendiente" | "Autorizada" | "Descartada"

        // Solo se completa cuando viene de un listado con JOIN (ALERTA_LISTAR_PENDIENTES);
        // no es una columna real de ALERTA_REVISION.
        public string EquipoNombre { get; set; }
    }
}