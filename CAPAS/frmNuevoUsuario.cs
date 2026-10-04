using System;
using System.Windows.Forms;

namespace CAPAS
{
    public partial class frmNuevoUsuario : FormBase
    {
        private readonly BLL.PerfilBLL _perfilBll = new BLL.PerfilBLL();

        public string NombreUsuario => txtNombre.Text.Trim();
        public string Contrasena => txtContrasena.Text;
        public int RolId => ((BE.Rol)cboRol.SelectedItem).Id;
        public string Nombre => txtNombrePersona.Text.Trim();
        public string Apellido => txtApellido.Text.Trim();
        public string Telefono => txtTelefono.Text.Trim();
        public string Email => txtEmail.Text.Trim();


        public frmNuevoUsuario()
        {
            InitializeComponent();
        }

        private void frmNuevoUsuario_Load(object sender, EventArgs e)
        {
            cboRol.DataSource = _perfilBll.ListarRolesParaCombo();
            cboRol.DisplayMember = "Nombre";
            cboRol.ValueMember = "Id";
            if (cboRol.Items.Count > 0) cboRol.SelectedIndex = 0;

            InicializarFormulario();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NombreUsuario))
            {
                MsgBox.Show(Textos.T("msg_IngresaUnNombreDeUsuario", "Ingresá un nombre de usuario."), "Atención",
                    MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }
            if (string.IsNullOrWhiteSpace(Contrasena))
            {
                MsgBox.Show(Textos.T("msg_IngresaUnaContrasena", "Ingresá una contraseña."), "Atención",
                    MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }
            if (cboRol.SelectedItem == null)
            {
                MsgBox.Show(Textos.T("msg_SeleccionaUnRol", "Seleccioná un rol."), "Atención",
                    MsgBox.Botones.OK, MsgBox.Icono.Atencion);
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
