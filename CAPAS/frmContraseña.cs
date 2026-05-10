using System;
using System.Windows.Forms;

namespace CAPAS
{
    public partial class frmContraseña : Form
    {
        public frmContraseña()
        {
            InitializeComponent();
        }

        private void frmContraseña_Load(object sender, EventArgs e)
        {
            txtPassActual.PasswordChar = '*';
            txtNuevaPass.PasswordChar = '*';
            txtConfPass.PasswordChar = '*';
        }

        private void btnContinuar_Click(object sender, EventArgs e)
        {
            string passActual = txtPassActual.Text.Trim();
            string nuevaPass = txtNuevaPass.Text.Trim();
            string confPass = txtConfPass.Text.Trim();

            if (string.IsNullOrEmpty(passActual) ||
                string.IsNullOrEmpty(nuevaPass) ||
                string.IsNullOrEmpty(confPass))
            {
                MessageBox.Show("Completá todos los campos.", "Atención",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (nuevaPass != confPass)
            {
                MessageBox.Show("Las contraseñas no coinciden.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

   
            if (!ValidarContrasena(nuevaPass))
            {
                MessageBox.Show("La contraseña debe tener:\n- 6 o más caracteres\n- 1 o más letras MAYÚSCULAS\n- 1 o más NÚMEROS",
                    "Contraseña inválida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string usuario = BE.SessionManager.getInstance().getUsuario().Usuario;
            BLL.UsuarioBLL bll = new BLL.UsuarioBLL();
            bool passCorrecta = bll.VerificarContrasena(usuario, passActual);

            if (!passCorrecta)
            {
                MessageBox.Show("La contraseña actual es incorrecta.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool ok = bll.CambiarContrasena(usuario, nuevaPass);

            if (ok)
            {
                MessageBox.Show("Contraseña cambiada exitosamente.", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Error al cambiar la contraseña.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidarContrasena(string pass)
        {
            if (pass.Length < 6) return false;
            bool tieneMayuscula = false;
            bool tieneNumero = false;
            foreach (char c in pass)
            {
                if (char.IsUpper(c)) tieneMayuscula = true;
                if (char.IsDigit(c)) tieneNumero = true;
            }
            return tieneMayuscula && tieneNumero;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}