using System;
using System.Linq;
using System.Windows.Forms;

namespace ConfiguradorBD
{
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            bool automatico = args.Any(a => a.Equals("/auto", StringComparison.OrdinalIgnoreCase));
            string servidor = args
                .Where(a => a.StartsWith("/servidor=", StringComparison.OrdinalIgnoreCase))
                .Select(a => a.Substring("/servidor=".Length).Trim('"'))
                .FirstOrDefault();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmConfigurador(automatico, servidor));
        }
    }
}
