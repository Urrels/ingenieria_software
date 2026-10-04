using System;

namespace BE
{
    public class VisitaTecnica
    {
        public int Id { get; set; }
        public int AlertaId { get; set; }
        public int TecnicoId { get; set; }
        public DateTime FechaCoordinada { get; set; }
        public string Estado { get; set; }
    }
}