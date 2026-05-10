using System;
using System.Windows.Forms;

namespace CAPAS
{
    public partial class frmAdminUsuarios : Form
    {
        private readonly BLL.UsuarioBLL _bll = new BLL.UsuarioBLL();

        public frmAdminUsuarios()
        {
            InitializeComponent();
        }

        private void frmAdminUsuarios_Load(object sender, EventArgs e)
        {
            CargarBloqueados();
        }

        private void CargarBloqueados()
        {
            dgvUsuarios.DataSource = _bll.ListarBloqueados();

            if (dgvUsuarios.Columns.Count > 0)
            {
                if (dgvUsuarios.Columns["Id"] != null) dgvUsuarios.Columns["Id"].HeaderText = "ID";
                if (dgvUsuarios.Columns["Usuario"] != null) dgvUsuarios.Columns["Usuario"].HeaderText = "Usuario";
                if (dgvUsuarios.Columns["Rol"] != null) dgvUsuarios.Columns["Rol"].HeaderText = "Rol";
                if (dgvUsuarios.Columns["IntentosFallidos"] != null) dgvUsuarios.Columns["IntentosFallidos"].HeaderText = "Intentos fallidos";

                if (dgvUsuarios.Columns["Contrasena"] != null) dgvUsuarios.Columns["Contrasena"].Visible = false;
                if (dgvUsuarios.Columns["Bloqueado"] != null) dgvUsuarios.Columns["Bloqueado"].Visible = false;
            }
        }

        private void btnDesbloquear_Click(object sender, EventArgs e)
        {
            if (dgvUsuarios.CurrentRow == null)
            {
                MessageBox.Show("Seleccioná un usuario.", "Atención",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            BE.USUARIO seleccionado = dgvUsuarios.CurrentRow.DataBoundItem as BE.USUARIO;
            if (seleccionado == null) return;

            DialogResult confirm = MessageBox.Show(
                "¿Desbloquear al usuario '" + seleccionado.Usuario + "'?",
                "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            _bll.Desbloquear(seleccionado.Usuario);
            MessageBox.Show("Usuario desbloqueado.", "Éxito",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            CargarBloqueados();
        }

        private void btnRefrescar_Click(object sender, EventArgs e)
        {
            CargarBloqueados();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
