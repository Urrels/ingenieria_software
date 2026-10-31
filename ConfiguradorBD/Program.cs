using System;
using System.Linq;
using System.Windows.Forms;

namespace ConfiguradorBD
{
    internal static class Program
    {
        private static RegistroInstalacion _log;

        [STAThread]
        private static void Main(string[] args)
        {
            bool automatico = args.Any(a => a.Equals("/auto", StringComparison.OrdinalIgnoreCase));
            string servidor = args
                .Where(a => a.StartsWith("/servidor=", StringComparison.OrdinalIgnoreCase))
                .Select(a => a.Substring("/servidor=".Length).Trim('"'))
                .FirstOrDefault();

            try
            {
                _log = new RegistroInstalacion();
                _log.Escribir($"Configurador iniciado ({(automatico ? "modo automático" : "modo manual")}) en {AppDomain.CurrentDomain.BaseDirectory}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo preparar el registro de instalación:\n\n" + ex.Message,
                    "Configuración de la base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (s, e) => Informar(e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (s, e) => Informar(e.ExceptionObject as Exception);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmConfigurador(automatico, servidor));
        }

        // Cualquier error inesperado queda en install.log y se muestra: el configurador no debe cerrarse en silencio.
        private static void Informar(Exception ex)
        {
            try { _log?.Escribir("ERROR inesperado: " + ex); } catch (Exception) { }
            MessageBox.Show("El configurador tuvo un error inesperado:\n\n" + (ex?.Message ?? "desconocido") +
                            (_log != null ? "\n\nEl detalle quedó en:\n" + _log.Ruta : ""),
                "Configuración de la base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
