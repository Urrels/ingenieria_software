using BE;
using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI
{
    public partial class FormLogin : Form
    {
        private readonly LoginBLL _loginBLL = new LoginBLL();

        public FormLogin()
        {
            InitializeComponent();
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            string nombre = txtUsuario.Text.Trim();
            string contrasena = txtContrasena.Text.Trim();

            UsuarioComponente resultado = _loginBLL.AutenticarUsuario(nombre, contrasena);

            if (resultado != null)
            {
                MessageBox.Show(
                    "Bienvenido, " + resultado.ToString() + "!",
                    "Login exitoso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // Acá abrís tu formulario principal
                // new FormPrincipal().Show();
                // this.Hide();
            }
            else
            {
                MessageBox.Show(
                    "Usuario o contraseña incorrectos.",
                    "Error de acceso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}