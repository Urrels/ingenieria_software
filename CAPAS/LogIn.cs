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
            BE.LoginResultado resultado = bll.AutenticarUsuario(
                txtUsuario.Text.Trim(), txtContrasena.Text.Trim());

            switch (resultado)
            {
                case BE.LoginResultado.Exito:
                    BE.USUARIO usuarioActual = Servicio.SessionManager.getInstance().getUsuario();
                    MessageBox.Show("Bienvenido, " + usuarioActual.Usuario + "!",
                        "Login exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    frmMenu frmMenu1 = new frmMenu();
                    frmMenu1.Show();
                    this.Hide();
                    break;

                case BE.LoginResultado.UsuarioBloqueado:
                    MessageBox.Show(
                        "Usuario bloqueado por intentos fallidos. Contactate con un administrador.",
                        "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;

                case BE.LoginResultado.CredencialesInvalidas:
                default:
                    MessageBox.Show("Usuario o contraseña incorrectos.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }


    }
}