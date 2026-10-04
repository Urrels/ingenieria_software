namespace BE
{
    public class EvaluacionCobertura
    {
        public int Id { get; set; }
        public int FranjaId { get; set; }
        public System.DateTime Semana { get; set; }
        public string Resultado { get; set; }
    }
}