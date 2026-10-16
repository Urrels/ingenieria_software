using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    public class LiquidacionBLL
    {
        private readonly DAL.LiquidacionDAL _dal = new DAL.LiquidacionDAL();
        private readonly PresenciaBLL _presenciaBLL = new PresenciaBLL();
        private readonly BitacoraBLL _bitacora = new BitacoraBLL();

        // ---------- Cálculo ----------

        public List<BE.LiquidacionDia> Calcular(DateTime desde, DateTime hasta, int? usuarioId = null)
        {
            ValidarRango(desde, hasta);

            var entrada = new BE.EntradaLiquidacion
            {
                Fichajes = _presenciaBLL.ObtenerFichajes(desde, hasta, usuarioId),
                Turnos = _dal.ListarTurnosAsignados(desde, hasta),
                ValoresHora = _dal.ListarValoresHora(),
                Feriados = new HashSet<DateTime>(_dal.ListarFeriados(desde, hasta).Select(f => f.Fecha.Date)),
                ExcedentesAprobados = _dal.ListarExcedentesAprobados(desde, hasta),
                Parametros = _dal.ObtenerParametros(),
                Ahora = DateTime.Now
            };
            if (usuarioId.HasValue)
                entrada.Turnos = entrada.Turnos.Where(t => t.UsuarioId == usuarioId.Value).ToList();

            return LiquidacionCalculo.Calcular(entrada, desde, hasta);
        }

        public List<BE.LiquidacionResumenEmpleado> Resumir(IEnumerable<BE.LiquidacionDia> dias) =>
            LiquidacionCalculo.Resumir(dias);

        // Ingresos sin egreso dentro del período (impiden cerrarlo hasta que el administrador los corrija).
        public List<BE.RegistroAsistenciaPersonal> ObtenerIncompletos(DateTime desde, DateTime hasta)
        {
            return _presenciaBLL.ObtenerFichajes(desde, hasta).Where(f => !f.HoraEgreso.HasValue).ToList();
        }

        // ---------- Cierre y pago ----------

        public int Cerrar(DateTime desde, DateTime hasta, BE.USUARIO administrador)
        {
            ValidarRango(desde, hasta);
            if (hasta.Date >= DateTime.Today)
                throw new InvalidOperationException("Solo se puede liquidar un período que ya terminó.");
            if (_dal.HayLiquidacionEnRango(desde, hasta))
                throw new InvalidOperationException("Ya existe una liquidación que se superpone con ese período.");

            int incompletos = ObtenerIncompletos(desde, hasta).Count;
            if (incompletos > 0)
                throw new InvalidOperationException(
                    $"Hay {incompletos} fichaje(s) incompleto(s) en el período. Corregilos antes de liquidar.");

            var dias = Calcular(desde, hasta);
            if (dias.Count == 0)
                throw new InvalidOperationException("No hay horas para liquidar en el período.");

            var sinValor = dias.Where(d => d.MinPagables > 0 && d.ValorHora == 0m)
                .Select(d => d.Empleado).Distinct().ToList();
            if (sinValor.Count > 0)
                throw new InvalidOperationException("Falta cargar el valor hora de: " + string.Join(", ", sinValor) + ".");

            int id = _dal.GuardarLiquidacion(desde, hasta, administrador.Usuario, dias);
            _bitacora.RegistrarAccion(administrador.Usuario,
                $"LIQUIDACION_CERRADA:{desde:yyyyMMdd}-{hasta:yyyyMMdd}");
            return id;
        }

        public void MarcarPagada(BE.Liquidacion liquidacion, BE.USUARIO administrador)
        {
            if (liquidacion.Estado != "Cerrada")
                throw new InvalidOperationException("Solo se puede marcar como pagada una liquidación cerrada.");
            _dal.MarcarPagada(liquidacion.Id, administrador.Usuario);
            _bitacora.RegistrarAccion(administrador.Usuario,
                $"LIQUIDACION_PAGADA:{liquidacion.Desde:yyyyMMdd}-{liquidacion.Hasta:yyyyMMdd}");
        }

        public List<BE.Liquidacion> ListarLiquidaciones() => _dal.ListarLiquidaciones();

        public List<BE.LiquidacionDia> ObtenerDiasDeLiquidacion(int liquidacionId) =>
            _dal.ListarDiasDeLiquidacion(liquidacionId);

        // ---------- Excedentes ----------

        public void AprobarExcedente(int usuarioId, string empleado, DateTime fecha, BE.USUARIO administrador)
        {
            ValidarPeriodoAbierto(fecha);
            _dal.AprobarExcedente(usuarioId, fecha, administrador.Usuario);
            _bitacora.RegistrarAccion(administrador.Usuario, $"EXCEDENTE_APROBADO:{fecha:yyyyMMdd} {empleado}");
        }

        public void RevocarExcedente(int usuarioId, string empleado, DateTime fecha, BE.USUARIO administrador)
        {
            ValidarPeriodoAbierto(fecha);
            _dal.RevocarExcedente(usuarioId, fecha);
            _bitacora.RegistrarAccion(administrador.Usuario, $"EXCEDENTE_REVOCADO:{fecha:yyyyMMdd} {empleado}");
        }

        // ---------- Parámetros, valor hora y feriados ----------

        public BE.ParametrosLiquidacion ObtenerParametros() => _dal.ObtenerParametros();

        public void GuardarParametros(BE.ParametrosLiquidacion p, BE.USUARIO administrador)
        {
            if (p.ToleranciaMinutos < 0 || p.ToleranciaMinutos > 60)
                throw new InvalidOperationException("La tolerancia debe estar entre 0 y 60 minutos.");
            if (p.RedondeoMinutos < 0 || p.RedondeoMinutos > 60)
                throw new InvalidOperationException("El redondeo debe estar entre 0 y 60 minutos.");
            if (p.JornadaNormalHoras < 1 || p.JornadaNormalHoras > 24)
                throw new InvalidOperationException("La jornada normal debe estar entre 1 y 24 horas.");
            if (p.RecargoExtraPct < 0 || p.RecargoExtraPct > 300 || p.RecargoFeriadoPct < 0 || p.RecargoFeriadoPct > 300)
                throw new InvalidOperationException("Los recargos deben estar entre 0% y 300%.");
            if (p.MaxHorasFichajeAbierto < 1 || p.MaxHorasFichajeAbierto > 24)
                throw new InvalidOperationException("El máximo de horas de un ingreso abierto debe estar entre 1 y 24.");

            _dal.GuardarParametros(p);
            _bitacora.RegistrarAccion(administrador.Usuario, "PARAMETROS_LIQUIDACION_MODIFICADOS");
        }

        public List<BE.EmpleadoLiquidable> ListarEmpleados() => _dal.ListarEmpleados();

        public List<BE.ValorHoraEmpleado> ListarValoresHora() => _dal.ListarValoresHora();

        public void GuardarValorHora(BE.EmpleadoLiquidable empleado, decimal valor, DateTime vigenteDesde, BE.USUARIO administrador)
        {
            if (valor <= 0m)
                throw new InvalidOperationException("El valor hora debe ser mayor a cero.");
            _dal.GuardarValorHora(empleado.Id, valor, vigenteDesde, administrador.Usuario);
            _bitacora.RegistrarAccion(administrador.Usuario, "VALOR_HORA_MODIFICADO:" + empleado.NombreCompleto);
        }

        public List<BE.Feriado> ListarFeriados(DateTime desde, DateTime hasta) => _dal.ListarFeriados(desde, hasta);

        public void GuardarFeriado(DateTime fecha, string descripcion, BE.USUARIO administrador)
        {
            _dal.GuardarFeriado(fecha, (descripcion ?? "").Trim());
            _bitacora.RegistrarAccion(administrador.Usuario, "FERIADO_GUARDADO:" + fecha.ToString("yyyyMMdd"));
        }

        public void EliminarFeriado(DateTime fecha, BE.USUARIO administrador)
        {
            _dal.EliminarFeriado(fecha);
            _bitacora.RegistrarAccion(administrador.Usuario, "FERIADO_ELIMINADO:" + fecha.ToString("yyyyMMdd"));
        }

        // ---------- Indicadores ----------

        // Horas fichadas contra horas asignadas por empleado en los últimos días (panel de indicadores).
        public List<BE.LiquidacionResumenEmpleado> ResumenReciente(int dias)
        {
            DateTime hasta = DateTime.Today;
            DateTime desde = hasta.AddDays(-(dias - 1));
            return Resumir(Calcular(desde, hasta));
        }

        // ---------- Validaciones ----------

        private static void ValidarRango(DateTime desde, DateTime hasta)
        {
            if (desde.Date > hasta.Date)
                throw new InvalidOperationException("La fecha desde no puede ser posterior a la fecha hasta.");
            if ((hasta.Date - desde.Date).TotalDays > 62)
                throw new InvalidOperationException("El período no puede superar los 62 días.");
        }

        private void ValidarPeriodoAbierto(DateTime fecha)
        {
            if (_dal.HayLiquidacionEnRango(fecha, fecha))
                throw new InvalidOperationException("Esa fecha pertenece a un período ya liquidado.");
        }
    }
}
