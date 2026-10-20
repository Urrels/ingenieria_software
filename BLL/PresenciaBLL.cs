using System;
using System.Collections.Generic;

namespace BLL
{
    public class PresenciaBLL
    {
        public const double PorcentajeMinimoDeFranja = 0.5;

        private readonly DAL.AsistenciaPersonalDAL _dal = new DAL.AsistenciaPersonalDAL();

        public List<BE.RegistroAsistenciaPersonal> ObtenerFichajes(DateTime desde, DateTime hasta, int? usuarioId = null)
        {
            return _dal.ListarRango(desde, hasta, usuarioId);
        }

        public static HashSet<int> EmpleadosPresentes(IEnumerable<BE.RegistroAsistenciaPersonal> fichajes,
            DateTime inicioFranja, DateTime finFranja, DateTime ahora, int maxHorasFichajeAbierto, string rolRequerido = null)
        {
            var presentes = new HashSet<int>();
            double minutosFranja = (finFranja - inicioFranja).TotalMinutes;
            if (minutosFranja <= 0) return presentes;

            foreach (var f in fichajes)
            {
                if (rolRequerido != null && f.Rol != null && f.Rol != rolRequerido) continue;

                DateTime? fin = f.Egreso;
                if (!fin.HasValue && (ahora - f.Ingreso).TotalHours <= maxHorasFichajeAbierto)
                    fin = ahora;
                if (!fin.HasValue) continue;

                DateTime desde = f.Ingreso > inicioFranja ? f.Ingreso : inicioFranja;
                DateTime hasta = fin.Value < finFranja ? fin.Value : finFranja;
                double solapado = (hasta - desde).TotalMinutes;
                if (solapado >= minutosFranja * PorcentajeMinimoDeFranja)
                    presentes.Add(f.UsuarioId);
            }
            return presentes;
        }
    }
}
