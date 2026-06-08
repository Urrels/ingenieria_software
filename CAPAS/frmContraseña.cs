using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ReaLTaiizor.Forms;
using ReaLTaiizor.Manager;

namespace CAPAS
{
    public partial class frmContraseña : MaterialForm, SeguridadYServicios.IObservadorIdioma
    {
        private readonly Dictionary<string, Control> _controles = new Dictionary<string, Control>();
        private readonly Dictionary<string, string>  _defaults  = new Dictionary<string, string>();

        public frmContraseña()
        {
            InitializeComponent();
        }

        private void frmContraseña_Load(object sender, EventArgs e)
        {
            txtPassActual.PasswordChar = '*';
            txtNuevaPass.PasswordChar  = '*';
            txtConfPass.PasswordChar   = '*';

            GuardarDefaults(this.Controls);
            _controles[this.Name] = this;
            _defaults[this.Name]  = this.Text;
            SeguridadYServicios.IdiomaManager.getInstance().Registrar(this);
            ActualizarIdioma();
            IdiomaUIHelper.AgregarSelector(this);
            MaterialSkinManager.Instance.AddFormToManage(this);
            AppTheme.AplicarTema(this);
        }

        private void frmContraseña_FormClosed(object sender, FormClosedEventArgs e)
        {
            SeguridadYServicios.IdiomaManager.getInstance().Desregistrar(this);
        }

        public void ActualizarIdioma()
        {
            foreach (var kvp in _controles)
            {
                string t = SeguridadYServicios.IdiomaManager.getInstance().Traducir(kvp.Key)
                           ?? _defaults[kvp.Key];
                kvp.Value.Text = t;
            }
        }

        private void GuardarDefaults(Control.ControlCollection controles)
        {
            foreach (Control c in controles)
            {
                if (!string.IsNullOrEmpty(c.Name) && !string.IsNullOrEmpty(c.Text))
                {
                    _controles[c.Name] = c;
                    _defaults[c.Name]  = c.Text;
                }
                if (c.HasChildren) GuardarDefaults(c.Controls);
            }
        }

        private void btnContinuar_Click(object sender, EventArgs e)
        {
            string passActual = txtPassActual.Text.Trim();
            string nuevaPass  = txtNuevaPass.Text.Trim();
            string confPass   = txtConfPass.Text.Trim();

            if (string.IsNullOrEmpty(passActual) || string.IsNullOrEmpty(nuevaPass) || string.IsNullOrEmpty(confPass))
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

            string errorValidacion = SeguridadYServicios.ValidadorContrasena.ObtenerError(nuevaPass);
            if (errorValidacion != null)
            {
                MessageBox.Show(errorValidacion, "Contraseña inválida",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string usuario = SeguridadYServicios.SessionManager.getInstance().getUsuario().Usuario;
            BLL.UsuarioBLL bll = new BLL.UsuarioBLL();

            if (!bll.VerificarContrasena(usuario, passActual))
            {
                MessageBox.Show("La contraseña actual es incorrecta.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (bll.CambiarContrasena(usuario, nuevaPass))
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

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
