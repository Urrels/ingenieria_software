using BLL;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace EmuladorFichaje
{
    // Reloj acelerado + ejecución de los fichajes planificados. Todo el acceso pasa por un candado
    // para que la API y el bucle del reloj vean siempre la misma hora.
    public class Simulador
    {
        private static readonly CultureInfo Es = new CultureInfo("es-AR");

        private readonly object _candado = new object();
        private readonly Queue<EventoFichaje> _pendientes;
        private readonly BLL.AsistenciaPersonalBLL _asistencia = new BLL.AsistenciaPersonalBLL();
        private readonly DAL.UsuarioDAL _usuarios = new DAL.UsuarioDAL();
        private readonly Dictionary<int, BE.USUARIO> _cacheUsuarios = new Dictionary<int, BE.USUARIO>();

        private DateTime _reloj;
        private readonly DateTime _fin;

        public double Velocidad { get; private set; }
        public bool Pausado { get; private set; }
        public int Registrados { get; private set; }
        public int Rechazados { get; private set; }

        public event Action<string> Mensaje;

        public Simulador(DateTime inicio, DateTime fin, double velocidad, IEnumerable<EventoFichaje> eventos)
        {
            _reloj = inicio;
            _fin = fin;
            Velocidad = velocidad;
            _pendientes = new Queue<EventoFichaje>(eventos);
            BLL.Reloj.Usar(() => { lock (_candado) return _reloj; });
        }

        public DateTime Ahora { get { lock (_candado) return _reloj; } }

        public bool Terminada { get { lock (_candado) return _reloj >= _fin; } }

        public int Pendientes { get { lock (_candado) return _pendientes.Count; } }

        public void CambiarVelocidad(double velocidad)
        {
            if (velocidad <= 0) throw new ArgumentException("La velocidad debe ser mayor a cero.");
            lock (_candado) Velocidad = velocidad;
        }

        public void Pausar(bool pausado)
        {
            lock (_candado) Pausado = pausado;
        }

        // Avanza el reloj según el tiempo real transcurrido y ejecuta los fichajes que vencieron.
        public void Avanzar(TimeSpan tiempoReal)
        {
            lock (_candado)
            {
                if (Pausado || _reloj >= _fin) return;

                DateTime destino = _reloj.AddSeconds(tiempoReal.TotalSeconds * Velocidad);
                if (destino > _fin) destino = _fin;

                // Cada fichaje se guarda con su hora exacta, no con la del paso del reloj.
                while (_pendientes.Count > 0 && _pendientes.Peek().Cuando <= destino)
                {
                    var evento = _pendientes.Dequeue();
                    _reloj = evento.Cuando > _reloj ? evento.Cuando : _reloj;
                    Ejecutar(evento);
                }
                _reloj = destino;
            }
        }

        // Ficha a mano (API) con la hora simulada actual.
        public string FicharAhora(string nombreUsuario, TipoFichaje tipo, out BE.ResultadoFichaje resultado)
        {
            lock (_candado)
            {
                int? id = _usuarios.ObtenerIdPorNombre(nombreUsuario);
                if (!id.HasValue) throw new KeyNotFoundException("No existe el usuario '" + nombreUsuario + "'.");
                BE.USUARIO usuario = Usuario(id.Value);
                resultado = tipo == TipoFichaje.Ingreso ? _asistencia.RegistrarIngreso(usuario) : _asistencia.RegistrarEgreso(usuario);
                if (resultado == BE.ResultadoFichaje.Ok) Registrados++; else Rechazados++;
                string linea = Linea(usuario.Usuario, tipo, resultado, "API");
                Mensaje?.Invoke(linea);
                return linea;
            }
        }

        public BE.EstadoFichaje Estado(string nombreUsuario)
        {
            lock (_candado)
            {
                int? id = _usuarios.ObtenerIdPorNombre(nombreUsuario);
                if (!id.HasValue) throw new KeyNotFoundException("No existe el usuario '" + nombreUsuario + "'.");
                return _asistencia.ObtenerEstado(Usuario(id.Value));
            }
        }

        private void Ejecutar(EventoFichaje e)
        {
            if (e.Omitir)
            {
                Mensaje?.Invoke($"[{Hora()}] {e.Empleado,-22} EGRESO   — {e.Nota}");
                return;
            }

            BE.USUARIO usuario = Usuario(e.UsuarioId);
            BE.ResultadoFichaje r = e.Tipo == TipoFichaje.Ingreso ? _asistencia.RegistrarIngreso(usuario) : _asistencia.RegistrarEgreso(usuario);
            if (r == BE.ResultadoFichaje.Ok) Registrados++; else Rechazados++;
            Mensaje?.Invoke(Linea(e.Empleado, e.Tipo, r, e.Nota));
        }

        private string Linea(string empleado, TipoFichaje tipo, BE.ResultadoFichaje r, string nota)
        {
            string tipoTxt = tipo == TipoFichaje.Ingreso ? "INGRESO " : "EGRESO  ";
            string resultado = r == BE.ResultadoFichaje.Ok ? "ok" : "RECHAZADO (" + r + ")";
            return $"[{Hora()}] {empleado,-22} {tipoTxt} {resultado}{(string.IsNullOrEmpty(nota) ? "" : " — " + nota)}";
        }

        private string Hora() => _reloj.ToString("ddd dd/MM HH:mm", Es);

        private BE.USUARIO Usuario(int id)
        {
            if (!_cacheUsuarios.TryGetValue(id, out BE.USUARIO u))
                _cacheUsuarios[id] = u = _usuarios.ObtenerPorId(id);
            return u;
        }
    }
}
