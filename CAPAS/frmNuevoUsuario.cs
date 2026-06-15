using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ReaLTaiizor.Forms;
using ReaLTaiizor.Manager;

namespace CAPAS
{
    public partial class frmNuevoUsuario : MaterialForm, SeguridadYServicios.IObservadorIdioma
    {
        public string NombreUsuario => txtNombre.Text.Trim();
        public string Contrasena    => txtContrasena.Text;
        public string Rol           => (string)cboRol.SelectedValue;
        public string Nombre        => txtNombrePersona.Text.Trim();
        public string Apellido      => txtApellido.Text.Trim();

        private readonly Dictionary<string, Control> _controles = new Dictionary<string, Control>();
        private readonly Dictionary<string, string>  _defaults  = new Dictionary<string, string>();

        public frmNuevoUsuario()
        {
            InitializeComponent();
        }

        private void frmNuevoUsuario_Load(object sender, EventArgs e)
        {
            cboRol.DataSource = new[]
            {
                new { Texto = "Administrador", Valor = "admin"   },
                new { Texto = "Usuario común",  Valor = "usuario" }
            };
            cboRol.DisplayMember = "Texto";
            cboRol.ValueMember   = "Valor";
            cboRol.SelectedIndex = 1;

            GuardarDefaults(this.Controls);
            _controles[this.Name] = this;
            _defaults[this.Name]  = this.Text;
            SeguridadYServicios.IdiomaManager.getInstance().Registrar(this);
            ActualizarIdioma();
            IdiomaUIHelper.AgregarSelector(this);
            MaterialSkinManager.Instance.AddFormToManage(this);
            AppTheme.AplicarTema(this);
        }

        private void frmNuevoUsuario_FormClosed(object sender, FormClosedEventArgs e)
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

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NombreUsuario))
            {
                MessageBox.Show("Ingresá un nombre de usuario.", "Atención",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(Contrasena))
            {
                MessageBox.Show("Ingresá una contraseña.", "Atención",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
