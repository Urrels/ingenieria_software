namespace BE
{
    public class DemandaEstimada
    {
        public int Id { get; set; }
        public int FranjaId { get; set; }
        public System.DateTime Semana { get; set; }
        public string RolRequerido { get; set; }
        public int CantidadPersonalNecesario { get; set; }
        public string OrigenDato { get; set; }
    }
}