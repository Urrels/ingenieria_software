using System;
using System.Collections.Generic;
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
                MaterialPrimary.Blue700,
                MaterialPrimary.Blue900,
                MaterialPrimary.Blue200,
                MaterialAccent.LightBlue200,
                MaterialTextShade.LIGHT
            );
            BLL.BackupBLL backupBll = new BLL.BackupBLL();

            try { backupBll.GenerarBackupFabrica(); } catch { }


            BLL.ResultadoSistema sistema = backupBll.VerificarSistema();

            if (!sistema.EstaIntegro)
            {
                string detalle = string.Join("\n", sistema.Problemas);
                string ultimoBackup = backupBll.ObtenerUltimoBackup();

                if (ultimoBackup == null)
                    ultimoBackup = backupBll.ObtenerBackupFabrica();


                if (ultimoBackup == null)
                {
                    MessageBox.Show(
                        "Se detectaron problemas críticos y no hay backups disponibles.\n\n" +
                        "Problemas:\n" + detalle + "\n\n" +
                        "Contacte al administrador de la base de datos: Jesica Andrea Funes, celular: 1122334455",
                        "Error crítico del sistema",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                    return;
                }

                DialogResult r = MessageBox.Show(
                    "Se detectaron los siguientes problemas:\n\n" +
                    detalle +
                    "\n\n¿Desea restaurar desde el último backup?\n" +
                    "(" + Path.GetFileName(ultimoBackup) + ")",
                    "Error del sistema",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (r == DialogResult.Yes)
                {
                    var frm = new frmRestore();
                    frm.Problemas = sistema.Problemas;
                    frm.ArchivoBackup = ultimoBackup;
                    if (frm.ShowDialog() != DialogResult.OK)
                    {
                        Application.Exit();
                        return;
                    }
                }
                else
                {
                    Application.Exit();
                    return;
                }
            }

            // backup automático silencioso al iniciar
            try { backupBll.GenerarBackup(); } catch { }

            InicializarIntegridad();
            Application.Run(new LogIn());
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
                    Errores = new List<string> { ex.ToString() }
                };
            }
        }

        internal static void GuardarLogIntegridad(List<string> errores)
        {
            try
            {
                string ruta = Path.Combine(Application.StartupPath, "integridad_error.log");
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