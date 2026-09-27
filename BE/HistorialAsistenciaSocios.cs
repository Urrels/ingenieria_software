using System;

namespace BE
{
    public class HistorialAsistenciaSocios
    {
        public int Id { get; set; }
        public int FranjaId { get; set; }
        public DateTime Fecha { get; set; }
        public int CantidadSocios { get; set; }
    }
}