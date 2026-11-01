using Newtonsoft.Json;
using SeguridadYServicios;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BLL
{
    public class DatosDemoBLL
    {
        public const string NombreArchivo = "datos_demo.json";

        private readonly DAL.GestionDAL _gestionDAL = new DAL.GestionDAL();
        private readonly DAL.LiquidacionDAL _liquidacionDAL = new DAL.LiquidacionDAL();
        private readonly DAL.AsistenciaPersonalDAL _asistenciaDAL = new DAL.AsistenciaPersonalDAL();
        private readonly DAL.UsuarioDAL _usuarioDAL = new DAL.UsuarioDAL();
        private readonly UsuarioBLL _usuarioBLL = new UsuarioBLL();
        private readonly FranjaHorariaBLL _franjaBLL = new FranjaHorariaBLL();
        private readonly HistorialAsistenciaSociosBLL _historialBLL = new HistorialAsistenciaSociosBLL();
        private readonly DisponibilidadBLL _disponibilidadBLL = new DisponibilidadBLL();
        private readonly GrillaBLL _grillaBLL = new GrillaBLL();
        private readonly TurnoBLL _turnoBLL = new TurnoBLL();
        private readonly AlertaRevisionBLL _alertaBLL = new AlertaRevisionBLL();
        private readonly EvaluacionCoberturaBLL _evaluacionBLL = new EvaluacionCoberturaBLL();
        private readonly BitacoraBLL _bitacora = new BitacoraBLL();
        private readonly Dictionary<int, string> _turnoPorUsuario = new Dictionary<int, string>();

        public static BE.DatosDemo Leer(string ruta)
        {
            if (!File.Exists(ruta))
                throw new FileNotFoundException("No se encontró el archivo de datos de demostración.", ruta);
            return JsonConvert.DeserializeObject<BE.DatosDemo>(File.ReadAllText(ruta, Encoding.UTF8))
                   ?? throw new InvalidDataException("El archivo de datos de demostración está vacío.");
        }

        public static string BuscarArchivo()
        {
            var carpeta = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            string instalado = Path.Combine(carpeta.FullName, "Scripts", NombreArchivo);
            if (File.Exists(instalado)) return instalado;
            for (DirectoryInfo d = carpeta; d != null; d = d.Parent)
            {
                string desarrollo = Path.Combine(d.FullName, "DAL", NombreArchivo);
                if (File.Exists(desarrollo)) return desarrollo;
            }
            return null;
        }

        public BE.DatosNegocio ContarDatos() => _gestionDAL.ContarDatosNegocio();

        public BE.ResultadoCargaDemo Cargar(BE.DatosDemo datos)
        {
            Validar(datos);
            if (!_gestionDAL.ContarDatosNegocio().SistemaVacio)
                throw new InvalidOperationException(
                    "El sistema ya tiene datos cargados. La demostración solo se puede cargar en un sistema vacío.");

            var resultado = new BE.ResultadoCargaDemo { ContrasenaEmpleados = datos.ContrasenaEmpleados };
            DateTime hoy = Reloj.Hoy;
            DateTime lunesActual = hoy.AddDays(-(((int)hoy.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7));
            int semanasHistorial = datos.Franjas.Max(f => f.Socios.Count);
            var roles = new DAL.PerfilDAL().ListarRoles().ToDictionary(r => r.Nombre, r => r.Id);

            var empleados = new List<BE.USUARIO>();
            _turnoPorUsuario.Clear();
            DateTime vigenteDesde = lunesActual.AddDays(-7 * (semanasHistorial + 1));
            foreach (var e in datos.Empleados)
            {
                if (!_usuarioBLL.Crear(e.Usuario, datos.ContrasenaEmpleados, roles[e.Rol], e.Nombre, e.Apellido, e.Telefono, e.Email))
                    throw new InvalidOperationException("No se pudo crear el usuario " + e.Usuario + " (¿ya existe?).");
                var usuario = _usuarioDAL.ObtenerPorId(_usuarioDAL.ObtenerIdPorNombre(e.Usuario).Value);
                usuario.Rol = e.Rol;
                _turnoPorUsuario[usuario.Id] = e.Turno ?? "";
                empleados.Add(usuario);
                if (e.ValorHora > 0 && e.Rol != "Tecnico")
                    _liquidacionDAL.GuardarValorHora(usuario.Id, e.ValorHora, vigenteDesde, "demo");
            }
            resultado.Empleados = empleados.Count;

            var franjas = new List<BE.FranjaHoraria>();
            foreach (var f in datos.Franjas)
            {
                TimeSpan inicio = TimeSpan.Parse(f.Inicio), fin = TimeSpan.Parse(f.Fin);
                int id = _franjaBLL.Insertar(f.Dia, inicio, fin, f.Rol);
                var franja = new BE.FranjaHoraria { Id = id, Dia = f.Dia, HoraInicio = inicio, HoraFin = fin, RolRequerido = f.Rol };
                franjas.Add(franja);
                for (int k = 0; k < f.Socios.Count; k++)
                {
                    DateTime lunes = lunesActual.AddDays(-7 * (f.Socios.Count - k));
                    _historialBLL.RegistrarAsistencia(franja, FechaDelDia(lunes, f.Dia), f.Socios[k]);
                }
            }
            resultado.Franjas = franjas.Count;

            foreach (var q in datos.Equipos)
            {
                int equipoId = _gestionDAL.InsertarEquipo(q.Nombre, q.Uso, q.Critico);
                for (int j = 0; j < q.UsoDiario.Count; j++)
                    _gestionDAL.InsertarUsoConFecha(equipoId, hoy.AddDays(-(q.UsoDiario.Count - j)).AddHours(11), q.UsoDiario[j]);
                if (q.Uso >= q.Critico) _alertaBLL.GenerarAlerta(equipoId);
            }
            resultado.Equipos = datos.Equipos.Count;

            foreach (var f in datos.Feriados)
                _liquidacionDAL.GuardarFeriado(FechaDelDia(lunesActual.AddDays(-7 * f.SemanasAtras), f.Dia), f.Descripcion);

            var prob = new ProbabilidadesFichaje
            {
                Ausencia = datos.Comportamiento.Ausencias, Tardanza = datos.Comportamiento.Tardanzas,
                HorasExtra = datos.Comportamiento.HorasExtra, SalidaAnticipada = datos.Comportamiento.SalidasAnticipadas,
                OlvidoEgreso = 0
            };
            var personal = empleados.Where(u => u.Rol != "Tecnico").ToList();
            for (int s = datos.SemanasConGrilla; s >= 1; s--)
            {
                DateTime lunes = lunesActual.AddDays(-7 * s);
                RegistrarDisponibilidades(personal, franjas, lunes);
                ArmarGrillaConfirmada(lunes, franjas);
                resultado.Grillas++;

                var planificador = new PlanificadorFichajes(prob, datos.Semilla + s);
                var eventos = planificador.Planificar(lunes, lunes.AddDays(6));
                if (s == 1)
                {
                    var egresos = eventos.Where(e => e.Tipo == TipoFichaje.Egreso).ToList();
                    if (egresos.Count > 0)
                    {
                        var olvido = egresos[egresos.Count / 2];
                        olvido.Omitir = true;
                        olvido.Nota = "se olvidó de fichar la salida";
                    }
                }
                resultado.Fichajes += GuardarFichajes(eventos);
                resultado.Evaluaciones += _evaluacionBLL.EvaluarSemana(lunes).Count;
            }

            DateTime proxima = lunesActual.AddDays(7);
            RegistrarDisponibilidades(personal, franjas, proxima);
            resultado.DisponibilidadesProximaSemana = personal.Count;

            string admin = SessionManager.getInstance().getUsuario()?.Usuario ?? "sistema";
            _bitacora.RegistrarAccion(admin, "DATOS_DEMO_CARGADOS");
            return resultado;
        }

        private void RegistrarDisponibilidades(List<BE.USUARIO> personal, List<BE.FranjaHoraria> franjas, DateTime lunes)
        {
            foreach (var u in personal)
            {
                string turno = _turnoPorUsuario.TryGetValue(u.Id, out string t) ? t : "";
                var propias = franjas.Where(f => f.RolRequerido == u.Rol
                    && (turno == "Manana" ? f.HoraInicio < TimeSpan.FromHours(12)
                      : turno == "Tarde" ? f.HoraInicio >= TimeSpan.FromHours(12) || f.Dia == "Saturday"
                      : true)).ToList();
                if (propias.Count > 0) _disponibilidadBLL.RegistrarDisponibilidad(u, lunes, propias);
            }
        }

        private void ArmarGrillaConfirmada(DateTime lunes, List<BE.FranjaHoraria> franjas)
        {
            var grilla = _grillaBLL.GenerarGrilla(lunes);
            var cargaPorUsuario = new Dictionary<int, int>();
            foreach (var turno in grilla.Turnos.Where(t => t.UsuarioId == null).ToList())
            {
                var franja = franjas.First(f => f.Id == turno.FranjaId);
                var candidatos = _turnoBLL.BuscarEmpleadosCompatibles(turno.RolRequerido, franja, grilla.Id)
                    .OrderBy(c => cargaPorUsuario.TryGetValue(c.Id, out int n) ? n : 0).ThenBy(c => c.Id).ToList();
                bool asignado = false;
                foreach (var c in candidatos)
                {
                    if (!_turnoBLL.AsignarEmpleado(turno, c)) continue;
                    cargaPorUsuario[c.Id] = (cargaPorUsuario.TryGetValue(c.Id, out int actual) ? actual : 0) + 1;
                    asignado = true;
                    break;
                }
                if (!asignado) _turnoBLL.MarcarDeficitCobertura(turno);
            }
            _grillaBLL.ConfirmarGrilla(grilla);
        }

        private int GuardarFichajes(List<EventoFichaje> eventos)
        {
            var abiertos = new Dictionary<int, int>();
            int cantidad = 0;
            foreach (var e in eventos)
            {
                if (e.Tipo == TipoFichaje.Ingreso)
                {
                    abiertos[e.UsuarioId] = _asistenciaDAL.RegistrarIngreso(e.UsuarioId, e.Cuando.Date, e.Cuando.TimeOfDay, permitirAbierto: true);
                    cantidad++;
                }
                else if (!e.Omitir && abiertos.TryGetValue(e.UsuarioId, out int id))
                {
                    _asistenciaDAL.RegistrarEgreso(id, e.Cuando.TimeOfDay);
                    abiertos.Remove(e.UsuarioId);
                }
            }
            return cantidad;
        }

        private static DateTime FechaDelDia(DateTime lunes, string dia)
        {
            DateTime? fecha = DAL.LiquidacionDAL.FechaDeFranja(lunes, dia);
            if (!fecha.HasValue) throw new InvalidDataException("Día inválido en los datos de demostración: " + dia);
            return fecha.Value;
        }

        private static void Validar(BE.DatosDemo d)
        {
            if (d.Empleados.Count == 0 || d.Franjas.Count == 0)
                throw new InvalidDataException("El archivo de demostración no tiene empleados o franjas.");
            string errorClave = ValidadorContrasena.ObtenerError(d.ContrasenaEmpleados);
            if (errorClave != null)
                throw new InvalidDataException("La contraseña de los empleados de demostración no cumple las reglas: " + errorClave);
            if (d.SemanasConGrilla < 1 || d.SemanasConGrilla > 8)
                throw new InvalidDataException("SemanasConGrilla debe estar entre 1 y 8.");
            foreach (var f in d.Franjas)
                if (f.Socios.Count < 4)
                    throw new InvalidDataException("Cada franja necesita al menos 4 semanas de socios (el motor predictivo usa 4).");
        }
    }
}
