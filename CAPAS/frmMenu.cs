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
            BE.USUARIO u = BE.SessionManager.getInstane().getUsuario();
            this.Text = "Menu — " + u.Usuario;
        }

        private void FrmPersonasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 FrmPersonas = new Form1();
            FrmPersonas.ShowDialog();
        }

        private void cambiarContraseñaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmContraseña frmContraseña1 = new frmContraseña();
            frmContraseña1.ShowDialog();
        }

        private void cerrarSesionToolStripMenuItem_Click(object sender, EventArgs e)
        {

            string usuario = BE.SessionManager.getInstane().getUsuario().Usuario;
            BLL.BitacoraBLL bitacora = new BLL.BitacoraBLL();
            bitacora.RegistrarLogout(usuario);


            BE.SessionManager.getInstane().cerrarSesion();

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