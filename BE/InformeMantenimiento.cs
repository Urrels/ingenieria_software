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

        // Se completan al leer con INFORME_OBTENER / INFORME_LISTAR_PENDIENTES_CIERRE
        // (vienen de un JOIN, no son columnas de INFORME_MANTENIMIENTO).
        public int AlertaId { get; set; }
        public int EquipoId { get; set; }
    }
}