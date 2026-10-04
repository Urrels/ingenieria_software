using System;
using System.Windows.Forms;

namespace CAPAS
{
    public partial class frmEditarUsuario : FormBase
    {
        private readonly BE.USUARIO _usuario;

        public string Nombre => txtNombrePersona.Text.Trim();
        public string Apellido => txtApellido.Text.Trim();
        public string Telefono => txtTelefono.Text.Trim();
        public string Email => txtEmail.Text.Trim();


        public frmEditarUsuario(BE.USUARIO usuario)
        {
            _usuario = usuario;
            InitializeComponent();
        }

        private void frmEditarUsuario_Load(object sender, EventArgs e)
        {
            txtNombrePersona.Text = _usuario.Nombre;
            txtApellido.Text = _usuario.Apellido;
            txtTelefono.Text = _usuario.Telefono;
            txtEmail.Text = _usuario.Email;

            InicializarFormulario();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
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
