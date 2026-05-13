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
            BE.USUARIO u = Servicio.SessionManager.getInstance().getUsuario();
            this.Text = "Menu — " + u.Usuario;

            administracionToolStripMenuItem.Visible = Servicio.SessionManager.getInstance().EsAdmin();
        }

        private void usuariosBloqueadosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAdminUsuarios frm = new frmAdminUsuarios();
            frm.ShowDialog();
        }

        

        private void cambiarContraseñaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmContraseña frmContraseña1 = new frmContraseña();
            frmContraseña1.ShowDialog();
        }

        private void cerrarSesionToolStripMenuItem_Click(object sender, EventArgs e)
        {

            string usuario = Servicio.SessionManager.getInstance().getUsuario().Usuario;
            BLL.BitacoraBLL bitacora = new BLL.BitacoraBLL();
            bitacora.RegistrarLogout(usuario);


            Servicio.SessionManager.getInstance().cerrarSesion();

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