using System;
using System.Collections.Generic;

namespace BLL
{
    // Único cálculo de "quién estuvo presente": lo usan la evaluación de cobertura (CU-36) y la liquidación (CU-38).
    public class PresenciaBLL
    {
        // Un empleado cuenta como presente en una franja si estuvo al menos esta parte de ella.
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
                // Una franja de Sala solo cuenta al personal de Sala (Recepción trabaja en horarios que se superponen).
                if (rolRequerido != null && f.Rol != null && f.Rol != rolRequerido) continue;

                // Un ingreso sin egreso todavía vigente cuenta hasta ahora; uno vencido no se puede medir.
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
