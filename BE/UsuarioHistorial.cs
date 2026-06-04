using System;

namespace BE
{
    public class UsuarioHistorial
    {
        public int      Id                { get; set; }
        public int      UsuarioId         { get; set; }
        public string   UsuarioLogin      { get; set; }
        public string   Rol               { get; set; }
        public bool     Bloqueado         { get; set; }
        public int      IntentosFallidos  { get; set; }
        public string   Perfiles          { get; set; }
        public DateTime FechaCambio       { get; set; }
        public string   RealizadoPor      { get; set; }
        public string   TipoCambio        { get; set; }
        public int?     VersionOrigen     { get; set; }
    }
}
