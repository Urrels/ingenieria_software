using System;
using System.Windows.Forms;

namespace CAPAS
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (!VerificarIntegridadBD())
                return;

            Application.Run(new LogIn());
        }

        private static bool VerificarIntegridadBD()
        {
            try
            {
                BLL.IntegridadBLL integridadBLL = new BLL.IntegridadBLL();

                // Primer arranque tras la migración: inicializar los dígitos.
                if (!integridadBLL.EstaInicializado())
                    integridadBLL.RecalcularIntegridad();

                BLL.ResultadoIntegridad resultado = integridadBLL.VerificarIntegridad();

                if (!resultado.EsValido)
                {
                    string detalle = string.Join(Environment.NewLine, resultado.Errores);
                    MessageBox.Show(
                        "Se detectaron problemas de integridad en la base de datos:" +
                        Environment.NewLine + Environment.NewLine + detalle +
                        Environment.NewLine + Environment.NewLine +
                        "Contacte al administrador del sistema.",
                        "Error de integridad de datos",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo verificar la integridad de la base de datos:" +
                    Environment.NewLine + ex.Message,
                    "Error de integridad",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }
        }
    }
}