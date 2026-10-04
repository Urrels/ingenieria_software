using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using ReaLTaiizor.Colors;
using ReaLTaiizor.Manager;
using ReaLTaiizor.Util;

namespace CAPAS
{
    static class Program
    {
        internal static BLL.ResultadoIntegridad ResultadoIntegridad { get; private set; }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var skin = MaterialSkinManager.Instance;
            skin.Theme = MaterialSkinManager.Themes.LIGHT;
            skin.ColorScheme = new MaterialColorScheme(
                AppTheme.Acento,
                AppTheme.AcentoHover,
                AppTheme.Seleccion,
                AppTheme.AcentoHover,
                MaterialTextShade.WHITE
            );

            if (!new BLL.ConexionBLL().Disponible())
            {
                OfrecerConfigurador();
                return;
            }

            InicializarIntegridad();
            Application.Run(new LogIn());
        }

        private static void OfrecerConfigurador()
        {
            string configurador = Path.Combine(Application.StartupPath, "ConfiguradorBD.exe");
            if (!File.Exists(configurador))
            {
                MsgBox.Show(Textos.T("msg_SinConexionSinConfigurador",
                        "No se pudo conectar a la base de datos y no se encontró el configurador (ConfiguradorBD.exe).\n\nRevisá la cadena de conexión en CAPAS.exe.config."),
                    "Error", MsgBox.Botones.OK, MsgBox.Icono.Error);
                return;
            }

            if (MsgBox.Show(Textos.T("msg_SinConexionAbrirConfigurador",
                    "No se pudo conectar a la base de datos.\n\n¿Querés abrir el configurador para crearla o elegir otro servidor?"),
                    "Error", MsgBox.Botones.SiNo, MsgBox.Icono.Error) != DialogResult.Yes)
                return;

            try
            {
                Process.Start(new ProcessStartInfo(configurador) { UseShellExecute = true });
            }
            catch (Win32Exception)
            {
            }
        }

        private static void InicializarIntegridad()
        {
            try
            {
                BLL.IntegridadBLL integridadBLL = new BLL.IntegridadBLL();
                if (!integridadBLL.EstaInicializado())
                    integridadBLL.RecalcularIntegridad();
                ResultadoIntegridad = integridadBLL.VerificarIntegridad();
            }
            catch (Exception ex)
            {
                ResultadoIntegridad = new BLL.ResultadoIntegridad
                {
                    EsValido = false,
                    Errores  = new List<string> { ex.ToString() }
                };
            }
        }

        internal static void GuardarLogIntegridad(List<string> errores)
        {
            try
            {
                string carpeta = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CAPAS");
                Directory.CreateDirectory(carpeta);
                string ruta = Path.Combine(carpeta, "integridad_error.log");
                using (StreamWriter sw = new StreamWriter(ruta, append: true))
                {
                    sw.WriteLine($"=== {DateTime.Now:yyyy-MM-dd HH:mm:ss} ===");
                    foreach (string error in errores)
                        sw.WriteLine(error);
                    sw.WriteLine();
                }
            }
            catch { }
        }
    }
}
