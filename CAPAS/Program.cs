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
            Application.Run(new LogIn()); // ← Arranca en LogIn
        }
    }
}