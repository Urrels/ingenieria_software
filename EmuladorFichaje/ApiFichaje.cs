using BLL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;

namespace EmuladorFichaje
{
    // API HTTP mínima para fichar "desde afuera" usando la hora simulada (como un reloj de fichaje real).
    public class ApiFichaje
    {
        private readonly Simulador _sim;
        private readonly int _puerto;
        private readonly JavaScriptSerializer _json = new JavaScriptSerializer();
        private HttpListener _listener;
        private volatile bool _corriendo;

        public event Action<string> Mensaje;

        public ApiFichaje(Simulador sim, int puerto)
        {
            _sim = sim;
            _puerto = puerto;
        }

        public void Iniciar()
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://localhost:{_puerto}/api/");
            _listener.Start();
            _corriendo = true;
            new Thread(Escuchar) { IsBackground = true }.Start();
        }

        public void Detener()
        {
            _corriendo = false;
            try { _listener?.Stop(); _listener?.Close(); } catch { }
        }

        private void Escuchar()
        {
            while (_corriendo)
            {
                try { Atender(_listener.GetContext()); }
                catch (HttpListenerException) { break; }
                catch (ObjectDisposedException) { break; }
                catch (Exception ex) { Mensaje?.Invoke("API: error interno: " + ex.Message); }
            }
        }

        private void Atender(HttpListenerContext ctx)
        {
            string ruta = ctx.Request.Url.AbsolutePath.TrimEnd('/').ToLowerInvariant();
            string metodo = ctx.Request.HttpMethod;
            try
            {
                if (ruta == "/api/reloj" && metodo == "GET") { Responder(ctx, 200, EstadoReloj()); return; }

                if (ruta == "/api/reloj" && metodo == "POST")
                {
                    var datos = Cuerpo(ctx);
                    if (datos.ContainsKey("velocidad")) _sim.CambiarVelocidad(Convert.ToDouble(datos["velocidad"]));
                    if (datos.ContainsKey("pausado")) _sim.Pausar(Convert.ToBoolean(datos["pausado"]));
                    Responder(ctx, 200, EstadoReloj());
                    return;
                }

                if (ruta == "/api/fichar" && metodo == "POST")
                {
                    var datos = Cuerpo(ctx);
                    string usuario = datos.ContainsKey("usuario") ? Convert.ToString(datos["usuario"]) : null;
                    string tipoTxt = datos.ContainsKey("tipo") ? Convert.ToString(datos["tipo"]).ToLowerInvariant() : null;
                    if (string.IsNullOrWhiteSpace(usuario) || (tipoTxt != "ingreso" && tipoTxt != "egreso"))
                    {
                        Responder(ctx, 400, new Dictionary<string, object> { { "error", "Enviar {\"usuario\":\"...\",\"tipo\":\"ingreso|egreso\"}" } });
                        return;
                    }
                    string linea = _sim.FicharAhora(usuario, tipoTxt == "ingreso" ? TipoFichaje.Ingreso : TipoFichaje.Egreso, out BE.ResultadoFichaje r);
                    Responder(ctx, r == BE.ResultadoFichaje.Ok ? 200 : 409, new Dictionary<string, object>
                    {
                        { "resultado", r.ToString() }, { "ahora", _sim.Ahora.ToString("s") }, { "detalle", linea }
                    });
                    return;
                }

                if (ruta == "/api/estado" && metodo == "GET")
                {
                    string usuario = ctx.Request.QueryString["usuario"];
                    var estado = _sim.Estado(usuario ?? "");
                    Responder(ctx, 200, new Dictionary<string, object>
                    {
                        { "usuario", usuario },
                        { "ingresoAbierto", estado.Abierto != null },
                        { "vencido", estado.Vencido },
                        { "desde", estado.Abierto == null ? null : estado.Abierto.Ingreso.ToString("s") },
                        { "ahora", _sim.Ahora.ToString("s") }
                    });
                    return;
                }

                Responder(ctx, 404, new Dictionary<string, object> { { "error", "Ruta no encontrada" } });
            }
            catch (KeyNotFoundException ex) { Responder(ctx, 404, new Dictionary<string, object> { { "error", ex.Message } }); }
            catch (Exception ex) when (ex is ArgumentException || ex is FormatException || ex is InvalidCastException)
            { Responder(ctx, 400, new Dictionary<string, object> { { "error", ex.Message } }); }
            catch (Exception ex) { Responder(ctx, 500, new Dictionary<string, object> { { "error", ex.Message } }); }
        }

        private Dictionary<string, object> EstadoReloj()
        {
            return new Dictionary<string, object>
            {
                { "ahora", _sim.Ahora.ToString("s") },
                { "velocidad", _sim.Velocidad },
                { "pausado", _sim.Pausado },
                { "terminada", _sim.Terminada },
                { "fichajesPendientes", _sim.Pendientes },
                { "registrados", _sim.Registrados },
                { "rechazados", _sim.Rechazados }
            };
        }

        private Dictionary<string, object> Cuerpo(HttpListenerContext ctx)
        {
            string texto;
            using (var lector = new StreamReader(ctx.Request.InputStream, ctx.Request.ContentEncoding))
                texto = lector.ReadToEnd();
            if (string.IsNullOrWhiteSpace(texto)) return new Dictionary<string, object>();
            return _json.Deserialize<Dictionary<string, object>>(texto);
        }

        private void Responder(HttpListenerContext ctx, int codigo, object cuerpo)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(_json.Serialize(cuerpo));
            ctx.Response.StatusCode = codigo;
            ctx.Response.ContentType = "application/json; charset=utf-8";
            ctx.Response.ContentLength64 = bytes.Length;
            ctx.Response.OutputStream.Write(bytes, 0, bytes.Length);
            ctx.Response.OutputStream.Close();
        }
    }
}
