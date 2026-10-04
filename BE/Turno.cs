namespace BE
{
    public class Turno
    {
        public int Id { get; set; }
        public int GrillaId { get; set; }
        public int FranjaId { get; set; }
        public int? UsuarioId { get; set; }
        public string RolRequerido { get; set; }
        public string Estado { get; set; }
    }
}