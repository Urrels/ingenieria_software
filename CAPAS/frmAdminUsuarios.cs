using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CAPAS
{
    public partial class frmAdminUsuarios : Form, SeguridadYServicios.IObservadorIdioma
    {
        private readonly BLL.UsuarioBLL _bll = new BLL.UsuarioBLL();
        private readonly Dictionary<string, Control> _controles  = new Dictionary<string, Control>();
        private readonly Dictionary<string, string>  _defaults   = new Dictionary<string, string>();

        public frmAdminUsuarios()
        {
            InitializeComponent();
        }

        private void frmAdminUsuarios_Load(object sender, EventArgs e)
        {
            GuardarDefaults(this.Controls);
            _controles.Remove("lblTitulo");
            _defaults.Remove("lblTitulo");
            _controles["lblTitulo_AdminUsuarios"] = lblTitulo;
            _defaults["lblTitulo_AdminUsuarios"]  = lblTitulo.Text;
            _controles[this.Name] = this;
            _defaults[this.Name]  = this.Text;
            SeguridadYServicios.IdiomaManager.getInstance().Registrar(this);
            ActualizarIdioma();
            CargarUsuarios();
            IdiomaUIHelper.AgregarSelector(this);
        }

        private void frmAdminUsuarios_FormClosed(object sender, FormClosedEventArgs e)
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
            ActualizarEncabezados();
        }

        private void ActualizarEncabezados()
        {
            if (dgvUsuarios.Columns.Count == 0) return;
            var mgr = SeguridadYServicios.IdiomaManager.getInstance();
            if (dgvUsuarios.Columns["Id"] != null)
                dgvUsuarios.Columns["Id"].HeaderText      = mgr.Traducir("colhdr_Id")       ?? "ID";
            if (dgvUsuarios.Columns["Usuario"] != null)
                dgvUsuarios.Columns["Usuario"].HeaderText = mgr.Traducir("colhdr_Usuario")   ?? "Usuario";
            if (dgvUsuarios.Columns["Rol"] != null)
                dgvUsuarios.Columns["Rol"].HeaderText     = mgr.Traducir("colhdr_Rol")       ?? "Rol";
            if (dgvUsuarios.Columns["Bloqueado"] != null)
                dgvUsuarios.Columns["Bloqueado"].HeaderText = mgr.Traducir("colhdr_Bloqueado") ?? "Bloqueado";
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

        private void CargarUsuarios()
        {
            dgvUsuarios.DataSource = _bll.ListarTodos();

            if (dgvUsuarios.Columns.Count > 0)
            {
                if (dgvUsuarios.Columns["Contrasena"] != null)       dgvUsuarios.Columns["Contrasena"].Visible       = false;
                if (dgvUsuarios.Columns["IntentosFallidos"] != null) dgvUsuarios.Columns["IntentosFallidos"].Visible = false;
                ActualizarEncabezados();
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            using (frmNuevoUsuario frm = new frmNuevoUsuario())
            {
                if (frm.ShowDialog() != DialogResult.OK) return;

                bool creado = _bll.Crear(frm.NombreUsuario, frm.Contrasena, frm.Rol);

                if (creado)
                {
                    MessageBox.Show("Usuario creado correctamente.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarUsuarios();
                }
                else
                {
                    MessageBox.Show("El nombre de usuario ya existe.", "Atención",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void btnModificarPerfiles_Click(object sender, EventArgs e)
        {
            BE.USUARIO seleccionado = dgvUsuarios.CurrentRow?.DataBoundItem as BE.USUARIO;
            if (seleccionado == null)
            {
                MessageBox.Show("Seleccioná un usuario.", "Atención",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (frmAsignarPerfiles frm = new frmAsignarPerfiles(seleccionado))
                frm.ShowDialog();
        }

        private void btnDesbloquear_Click(object sender, EventArgs e)
        {
            BE.USUARIO seleccionado = dgvUsuarios.CurrentRow?.DataBoundItem as BE.USUARIO;
            if (seleccionado == null)
            {
                MessageBox.Show("Seleccioná un usuario.", "Atención",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!seleccionado.Bloqueado)
            {
                MessageBox.Show("El usuario no está bloqueado.", "Atención",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("¿Desbloquear al usuario '" + seleccionado.Usuario + "'?",
                "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            _bll.Desbloquear(seleccionado.Usuario);
            MessageBox.Show("Usuario desbloqueado.", "Éxito",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            CargarUsuarios();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            BE.USUARIO seleccionado = dgvUsuarios.CurrentRow?.DataBoundItem as BE.USUARIO;
            if (seleccionado == null)
            {
                MessageBox.Show("Seleccioná un usuario.", "Atención",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string usuarioActual = SeguridadYServicios.SessionManager.getInstance().getUsuario().Usuario;
            if (seleccionado.Usuario == usuarioActual)
            {
                MessageBox.Show("No podés eliminar tu propio usuario.", "Atención",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"¿Eliminar al usuario '{seleccionado.Usuario}'? Esta acción no se puede deshacer.",
                "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            _bll.Eliminar(seleccionado.Id);
            CargarUsuarios();
        }

        private void btnHistorial_Click(object sender, EventArgs e)
        {
            BE.USUARIO seleccionado = dgvUsuarios.CurrentRow?.DataBoundItem as BE.USUARIO;
            if (seleccionado == null)
            {
                MessageBox.Show("Seleccioná un usuario.", "Atención",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (frmHistorialUsuario frm = new frmHistorialUsuario(seleccionado))
                frm.ShowDialog();

            CargarUsuarios();
        }

        private void btnRefrescar_Click(object sender, EventArgs e)
        {
            CargarUsuarios();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
