using System;
using System.Windows.Forms;

namespace CAPAS
{
    public partial class frmMenu : Form
    {
        public frmMenu()
        {
            InitializeComponent();
        }

        private void frmMenu_Load(object sender, EventArgs e)
        {
            // Mostrar usuario logueado en el título
            BE.USUARIO u = BE.SessionManager.getInstane().getUsuario();
            this.Text = "Menu — " + u.Usuario;
        }

        

        private void cambiarContraseñaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmContraseña frmContraseña1 = new frmContraseña();
            frmContraseña1.ShowDialog();
        }

        private void cerrarSesionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Registrar logout en bitácora
            string usuario = BE.SessionManager.getInstane().getUsuario().Usuario;
            BLL.BitacoraBLL bitacora = new BLL.BitacoraBLL();
            bitacora.RegistrarLogout(usuario);

            // Cerrar sesión en el Singleton
            BE.SessionManager.getInstane().cerrarSesion();

            // Volver al login
            LogIn login = new LogIn();
            login.Show();
            this.Close();
        }
        private void bitacoraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmBitacora frmBit = new frmBitacora();
            frmBit.ShowDialog();
        }
    }
}