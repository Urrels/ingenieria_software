namespace BE
{
    public class Equipo
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int UsoAcumulado { get; set; }
        public int? NivelUsoCritico { get; set; }
        public string Estado { get; set; }           // "Operativo" | "EnMantenimiento"
        public int? TecnicoHabitualId { get; set; }
    }
}