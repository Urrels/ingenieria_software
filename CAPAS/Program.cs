using System;
using System.Windows.Forms;
using BLL;

namespace UI
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                AppInicializador.Inicializar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo conectar a SQL Server:\n\n" + ex.Message,
                    "Error de base de datos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LogIn()); // ← Arranca en LogIn
        }
    }
}