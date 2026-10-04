using System;
using System.IO;

namespace ConfiguradorBD
{
    internal class RegistroInstalacion
    {
        internal event Action<string> LineaAgregada;

        internal string Ruta { get; }

        internal RegistroInstalacion()
        {
            string carpeta = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "GestionGimnasio");
            Directory.CreateDirectory(carpeta);
            Ruta = Path.Combine(carpeta, "install.log");
        }

        internal void Escribir(string mensaje)
        {
            string linea = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {mensaje}";
            try
            {
                File.AppendAllText(Ruta, linea + Environment.NewLine);
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
            LineaAgregada?.Invoke(linea);
        }
    }
}
