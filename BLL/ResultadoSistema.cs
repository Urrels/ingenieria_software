using System.Collections.Generic;
namespace BLL
{
    public class ResultadoSistema
    {
        public bool EstaIntegro { get; set; } = true;
        public List<string> Problemas { get; set; } = new List<string>();
    }
}
