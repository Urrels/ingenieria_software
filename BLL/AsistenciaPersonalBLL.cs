using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    public class AsistenciaPersonalBLL
    {
        private const int LargoMaximoMotivo = 150;

        private readonly DAL.AsistenciaPersonalDAL _dal = new DAL.AsistenciaPersonalDAL();
        private readonly DAL.LiquidacionDAL _liquidacionDAL = new DAL.LiquidacionDAL();
        private readonly BitacoraBLL _bitacora = new BitacoraBLL();

        public BE.EstadoFichaje ObtenerEstado(BE.USUARIO usuario)
        {
            var abierto = _dal.ObtenerAbierta(usuario.Id);
            if (abierto == null) return new BE.EstadoFichaje();

            int max = _liquidacionDAL.ObtenerParametros().MaxHorasFichajeAbierto;
            return new BE.EstadoFichaje
            {
                Abierto = abierto,
                Vencido = (Reloj.Ahora - abierto.Ingreso).TotalHours > max
            };
        }

        public BE.ResultadoFichaje RegistrarIngreso(BE.USUARIO usuario)
        {
            var estado = ObtenerEstado(usuario);
            if (estado.Abierto != null)
                return estado.Vencido ? BE.ResultadoFichaje.IngresoVencido : BE.ResultadoFichaje.IngresoAbierto;

            if (_dal.RegistrarIngreso(usuario.Id, Reloj.Hoy, Reloj.Ahora.TimeOfDay) == 0)
                return BE.ResultadoFichaje.IngresoAbierto;
            return BE.ResultadoFichaje.Ok;
        }

        public BE.ResultadoFichaje RegistrarEgreso(BE.USUARIO usuario)
        {
            var estado = ObtenerEstado(usuario);
            if (estado.Abierto == null) return BE.ResultadoFichaje.SinIngreso;
            if (estado.Vencido) return BE.ResultadoFichaje.IngresoVencido;

            _dal.RegistrarEgreso(estado.Abierto.Id, Reloj.Ahora.TimeOfDay);
            return BE.ResultadoFichaje.Ok;
        }

        public List<BE.RegistroAsistenciaPersonal> Listar(DateTime desde, DateTime hasta, bool soloIncompletos)
        {
            var lista = _dal.ListarRango(desde, hasta);
            if (!soloIncompletos) return lista;

            int max = _liquidacionDAL.ObtenerParametros().MaxHorasFichajeAbierto;
            return lista.Where(f => !f.HoraEgreso.HasValue && (Reloj.Ahora - f.Ingreso).TotalHours > max).ToList();
        }

        public void Corregir(int id, TimeSpan ingreso, TimeSpan egreso, string motivo, BE.USUARIO administrador)
        {
            motivo = ValidarMotivo(motivo);
            if (ingreso == egreso)
                throw new InvalidOperationException("El ingreso y el egreso no pueden ser iguales.");

            var registro = _dal.ObtenerPorId(id);
            if (registro == null) throw new InvalidOperationException("El fichaje no existe.");
            ValidarPeriodoAbierto(registro.Fecha);

            string antes = registro.HoraEgreso.HasValue
                ? $"{registro.HoraIngreso:hh\\:mm}-{registro.HoraEgreso.Value:hh\\:mm}"
                : $"{registro.HoraIngreso:hh\\:mm}-sin egreso";
            string motivoFinal = $"{motivo} [antes: {antes}]";

            ValidarSinSuperposicion(registro.UsuarioId, registro.Fecha, ingreso, egreso, id);

            _dal.Corregir(id, ingreso, egreso, motivoFinal, administrador.Usuario);
            _bitacora.RegistrarAccion(administrador.Usuario, "FICHAJE_CORREGIDO:" + registro.Empleado);
        }

        public void AgregarManual(int usuarioId, string empleado, DateTime fecha, TimeSpan ingreso, TimeSpan egreso,
            string motivo, BE.USUARIO administrador)
        {
            motivo = ValidarMotivo(motivo);
            if (ingreso == egreso)
                throw new InvalidOperationException("El ingreso y el egreso no pueden ser iguales.");
            if (fecha.Date > Reloj.Hoy)
                throw new InvalidOperationException("No se puede cargar un fichaje con fecha futura.");
            ValidarPeriodoAbierto(fecha);
            ValidarSinSuperposicion(usuarioId, fecha, ingreso, egreso, 0);

            _dal.InsertarManual(usuarioId, fecha, ingreso, egreso, motivo, administrador.Usuario);
            _bitacora.RegistrarAccion(administrador.Usuario, "FICHAJE_MANUAL:" + empleado);
        }

        private static string ValidarMotivo(string motivo)
        {
            motivo = (motivo ?? "").Trim();
            if (motivo.Length == 0)
                throw new InvalidOperationException("Indicá el motivo de la corrección.");
            if (motivo.Length > LargoMaximoMotivo)
                throw new InvalidOperationException($"El motivo no puede superar los {LargoMaximoMotivo} caracteres.");
            return motivo;
        }

        private void ValidarPeriodoAbierto(DateTime fecha)
        {
            if (_liquidacionDAL.HayLiquidacionEnRango(fecha, fecha))
                throw new InvalidOperationException("Esa fecha pertenece a un período ya liquidado.");
        }

        private void ValidarSinSuperposicion(int usuarioId, DateTime fecha, TimeSpan ingreso, TimeSpan egreso, int ignorarId)
        {
            DateTime inicio = fecha.Date + ingreso;
            DateTime fin = fecha.Date + egreso;
            if (fin < inicio) fin = fin.AddDays(1);

            int max = _liquidacionDAL.ObtenerParametros().MaxHorasFichajeAbierto;
            if ((fin - inicio).TotalHours > max)
                throw new InvalidOperationException($"El fichaje no puede durar más de {max} horas.");

            foreach (var otro in _dal.ListarRango(fecha.AddDays(-1), fecha.AddDays(1), usuarioId))
            {
                if (otro.Id == ignorarId) continue;
                DateTime finOtro = otro.Egreso ?? Reloj.Ahora;   // un ingreso abierto cuenta hasta ahora
                if (inicio < finOtro && fin > otro.Ingreso)
                    throw new InvalidOperationException("El fichaje se superpone con otro del mismo empleado.");
            }
        }
    }
}
