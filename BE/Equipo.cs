namespace BE
{
    public class Equipo
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int UsoAcumulado { get; set; }
        public int? NivelUsoCritico { get; set; }
        public string Estado { get; set; }
        public int? TecnicoHabitualId { get; set; }
    }
}