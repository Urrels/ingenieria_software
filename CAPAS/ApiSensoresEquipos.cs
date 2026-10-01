using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace CAPAS
{
    public class UsoEquipoRequest
    {
        public string NombreEquipo { get; set; }
        public int Incremento { get; set; } = 1;
    }

    // Mini API local: simula el endpoint al que un sensor IoT real le reportaría
    // el uso de una máquina, sin necesitar IIS ni un servidor aparte.
    public class ApiSensoresEquipos
    {
        private HttpListener _listener;
        private Thread _hiloEscucha;
        private volatile bool _corriendo;

        public event Action<string> OnEvento;

        public void Iniciar(int puerto = 5050)
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://localhost:{puerto}/api/");
            _listener.Start();
            _corriendo = true;

            _hiloEscucha = new Thread(EscucharSolicitudes) { IsBackground = true };
            _hiloEscucha.Start();

            OnEvento?.Invoke($"[{DateTime.Now:HH:mm:ss}] API escuchando en http://localhost:{puerto}/api/equipos/uso");
        }

        public void Detener()
        {
            _corriendo = false;
            try { _listener?.Stop(); _listener?.Close(); } catch { }
            OnEvento?.Invoke($"[{DateTime.Now:HH:mm:ss}] API detenida.");
        }

        private void EscucharSolicitudes()
        {
            while (_corriendo)
            {
                try
                {
                    var contexto = _listener.GetContext();   // se bloquea hasta que llega un request
                    ProcesarSolicitud(contexto);
                }
                catch (HttpListenerException)
                {
                    break;   // listener detenido, salida normal
                }
                catch (Exception ex)
                {
                    OnEvento?.Invoke($"[{DateTime.Now:HH:mm:ss}] Error interno: {ex.Message}");
                }
            }
        }

        private void ProcesarSolicitud(HttpListenerContext contexto)
        {
            var request = contexto.Request;
            var response = contexto.Response;

            if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/api/equipos/uso")
            {
                string cuerpo;
                using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                    cuerpo = reader.ReadToEnd();

                try
                {
                    var datos = JsonConvert.DeserializeObject<UsoEquipoRequest>(cuerpo);
                    var equipoBLL = new BLL.EquipoBLL();
                    var equipo = equipoBLL.ObtenerPorNombre(datos.NombreEquipo);

                    if (equipo == null)
                        throw new Exception($"Equipo '{datos.NombreEquipo}' no encontrado.");

                    equipoBLL.RegistrarUso(equipo.Id, datos.Incremento);

                    OnEvento?.Invoke($"[{DateTime.Now:HH:mm:ss}] Sensor → {datos.NombreEquipo} (+{datos.Incremento})");

                    response.StatusCode = 200;
                    EscribirRespuesta(response, "{\"ok\":true}");
                }
                catch (Exception ex)
                {
                    OnEvento?.Invoke($"[{DateTime.Now:HH:mm:ss}] Rechazado: {ex.Message}");
                    response.StatusCode = 400;
                    EscribirRespuesta(response, "{\"ok\":false,\"error\":\"" + ex.Message.Replace("\"", "'") + "\"}");
                }
            }
            else
            {
                response.StatusCode = 404;
                EscribirRespuesta(response, "{\"error\":\"ruta no encontrada\"}");
            }

            response.Close();
        }

        private void EscribirRespuesta(HttpListenerResponse response, string json)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(json);
            response.ContentType = "application/json";
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
        }
    }
}