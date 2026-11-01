using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;

namespace BLL
{
    // Cliente de la API pública ArgentinaDatos (feriados nacionales por año). Los días "puente" son no laborables
    // optativos para el empleador, no feriados: no se importan para no pagar recargo de más.
    internal static class FeriadosApi
    {
        private const string UrlPorAnio = "https://api.argentinadatos.com/v1/feriados/";
        private const int SegundosDeEspera = 10;

        internal static List<BE.Feriado> Descargar(int anio)
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            string json;
            try
            {
                using (var cliente = new HttpClient { Timeout = TimeSpan.FromSeconds(SegundosDeEspera) })
                    json = cliente.GetStringAsync(UrlPorAnio + anio.ToString(CultureInfo.InvariantCulture)).GetAwaiter().GetResult();
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is System.Threading.Tasks.TaskCanceledException || ex is WebException)
            {
                throw new InvalidOperationException(
                    "No se pudo consultar el servicio de feriados. Verificá la conexión a internet o cargalos a mano.", ex);
            }
            return Interpretar(json, anio);
        }

        internal static List<BE.Feriado> Interpretar(string json, int anio)
        {
            JArray filas;
            try { filas = JArray.Parse(json); }
            catch (Exception ex) when (ex is Newtonsoft.Json.JsonException)
            {
                throw new InvalidOperationException("El servicio de feriados devolvió una respuesta que no se pudo interpretar.", ex);
            }

            var feriados = new List<BE.Feriado>();
            foreach (JToken fila in filas)
            {
                string tipo = (string)fila["tipo"] ?? "";
                if (tipo.Equals("puente", StringComparison.OrdinalIgnoreCase)) continue;
                if (!DateTime.TryParseExact((string)fila["fecha"], "yyyy-MM-dd", CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out DateTime fecha) || fecha.Year != anio) continue;
                feriados.Add(new BE.Feriado { Fecha = fecha, Descripcion = ((string)fila["nombre"] ?? "").Trim() });
            }
            return feriados;
        }
    }
}
