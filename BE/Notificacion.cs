using System;

namespace BE
{
    public class Notificacion
    {
        public int Id { get; set; }
        public int? GrillaId { get; set; }
        public int UsuarioId { get; set; }
        public string Mensaje { get; set; }
        public DateTime FechaEnvio { get; set; }
        public string Estado { get; set; }
    }
}