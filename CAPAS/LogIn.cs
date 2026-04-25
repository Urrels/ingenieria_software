using System;
using System.Windows.Forms;

namespace CAPAS
{
    public partial class LogIn : Form
    {
        public LogIn()
        {
            InitializeComponent();
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsuario.Text) ||
                string.IsNullOrWhiteSpace(txtContrasena.Text))
            {
                MessageBox.Show("Completá usuario y contraseña.", "Atención",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            BLL.LoginBLL bll = new BLL.LoginBLL();
            bool ok = bll.AutenticarUsuario(txtUsuario.Text.Trim(), txtContrasena.Text.Trim());

            if (ok)
            {
                BE.USUARIO usuarioActual = BE.SessionManager.getInstane().getUsuario();
                MessageBox.Show("Bienvenido, " + usuarioActual.Usuario + "!",
                    "Login exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                Form1 form1 = new Form1();
                form1.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Usuario o contraseña incorrectos.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


    }
}