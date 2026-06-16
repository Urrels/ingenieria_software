using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ReaLTaiizor.Forms;
using ReaLTaiizor.Manager;

namespace CAPAS
{
    public partial class frmHistorialUsuario : MaterialForm, SeguridadYServicios.IObservadorIdioma
    {
        private readonly BE.USUARIO _usuario;
        private readonly BLL.UsuarioHistorialBLL _bll = new BLL.UsuarioHistorialBLL();
        private readonly Dictionary<string, Control> _controles = new Dictionary<string, Control>();
        private readonly Dictionary<string, string> _defaults = new Dictionary<string, string>();

        public frmHistorialUsuario(BE.USUARIO usuario)
        {
            _usuario = usuario;
            InitializeComponent();
        }

        private void frmHistorialUsuario_Load(object sender, EventArgs e)
        {
            GuardarDefaults(this.Controls);
            _controles.Remove("lblTitulo");
            _defaults.Remove("lblTitulo");
            _controles["lblTitulo_HistorialUsuarios"] = lblTitulo;
            _defaults["lblTitulo_HistorialUsuarios"] = lblTitulo.Text;
            _controles[this.Name] = this;
            _defaults[this.Name] = this.Text;
            SeguridadYServicios.IdiomaManager.getInstance().Registrar(this);
            ActualizarIdioma();
            CargarHistorial();
            IdiomaUIHelper.AgregarSelector(this);
            MaterialSkinManager.Instance.AddFormToManage(this);
            AppTheme.AplicarTema(this);
        }

        private void frmHistorialUsuario_FormClosed(object sender, FormClosedEventArgs e)
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
            if (dgvHistorial.Columns.Count == 0) return;
            var mgr = SeguridadYServicios.IdiomaManager.getInstance();
            if (dgvHistorial.Columns["FechaCambio"] != null)
                dgvHistorial.Columns["FechaCambio"].HeaderText = mgr.Traducir("colhdr_FechaCambio") ?? "Fecha";
            if (dgvHistorial.Columns["TipoCambio"] != null)
                dgvHistorial.Columns["TipoCambio"].HeaderText = mgr.Traducir("colhdr_TipoCambio") ?? "Tipo";
            if (dgvHistorial.Columns["Rol"] != null)
                dgvHistorial.Columns["Rol"].HeaderText = mgr.Traducir("colhdr_Rol") ?? "Rol";
            if (dgvHistorial.Columns["Bloqueado"] != null)
                dgvHistorial.Columns["Bloqueado"].HeaderText = mgr.Traducir("colhdr_Bloqueado") ?? "Bloqueado";
            if (dgvHistorial.Columns["IntentosFallidos"] != null)
                dgvHistorial.Columns["IntentosFallidos"].HeaderText = mgr.Traducir("colhdr_IntentosFallidos") ?? "Intentos";
            if (dgvHistorial.Columns["Perfiles"] != null)
                dgvHistorial.Columns["Perfiles"].HeaderText = mgr.Traducir("colhdr_Perfiles") ?? "Perfiles";
            if (dgvHistorial.Columns["Nombre"] != null)
                dgvHistorial.Columns["Nombre"].HeaderText = mgr.Traducir("colhdr_NombrePersona") ?? "Nombre";
            if (dgvHistorial.Columns["Apellido"] != null)
                dgvHistorial.Columns["Apellido"].HeaderText = mgr.Traducir("colhdr_Apellido") ?? "Apellido";
            if (dgvHistorial.Columns["RealizadoPor"] != null)
                dgvHistorial.Columns["RealizadoPor"].HeaderText = mgr.Traducir("colhdr_RealizadoPor") ?? "Realizado por";
            if (dgvHistorial.Columns["VersionOrigen"] != null)
                dgvHistorial.Columns["VersionOrigen"].HeaderText = mgr.Traducir("colhdr_VersionOrigen") ?? "Versión origen";
            if (dgvHistorial.Columns["Telefono"] != null)
                dgvHistorial.Columns["Telefono"].HeaderText = mgr.Traducir("colhdr_Telefono") ?? "Teléfono";
            if (dgvHistorial.Columns["Email"] != null)
                dgvHistorial.Columns["Email"].HeaderText = mgr.Traducir("colhdr_Email") ?? "Email";
        }

        private void GuardarDefaults(Control.ControlCollection controles)
        {
            foreach (Control c in controles)
            {
                if (!string.IsNullOrEmpty(c.Name) && !string.IsNullOrEmpty(c.Text))
                {
                    _controles[c.Name] = c;
                    _defaults[c.Name] = c.Text;
                }
                if (c.HasChildren) GuardarDefaults(c.Controls);
            }
        }

        private void CargarHistorial()
        {
            dgvHistorial.DataSource = _bll.ObtenerHistorial(_usuario.Id);

            if (dgvHistorial.Columns.Count > 0)
            {
                if (dgvHistorial.Columns["Id"] != null) dgvHistorial.Columns["Id"].Visible = false;
                if (dgvHistorial.Columns["UsuarioId"] != null) dgvHistorial.Columns["UsuarioId"].Visible = false;
                if (dgvHistorial.Columns["UsuarioLogin"] != null) dgvHistorial.Columns["UsuarioLogin"].Visible = false;
                if (dgvHistorial.Columns["RolId"] != null) dgvHistorial.Columns["RolId"].Visible = false;
                if (dgvHistorial.Columns["FechaCambio"] != null) dgvHistorial.Columns["FechaCambio"].Width = 145;
                if (dgvHistorial.Columns["TipoCambio"] != null) dgvHistorial.Columns["TipoCambio"].Width = 130;
                if (dgvHistorial.Columns["Rol"] != null) dgvHistorial.Columns["Rol"].Width = 80;
                if (dgvHistorial.Columns["Bloqueado"] != null) dgvHistorial.Columns["Bloqueado"].Width = 80;
                if (dgvHistorial.Columns["IntentosFallidos"] != null) dgvHistorial.Columns["IntentosFallidos"].Width = 90;
                if (dgvHistorial.Columns["Perfiles"] != null) dgvHistorial.Columns["Perfiles"].Width = 90;
                if (dgvHistorial.Columns["Nombre"] != null) dgvHistorial.Columns["Nombre"].Width = 100;
                if (dgvHistorial.Columns["Apellido"] != null) dgvHistorial.Columns["Apellido"].Width = 100;
                if (dgvHistorial.Columns["RealizadoPor"] != null) dgvHistorial.Columns["RealizadoPor"].Width = 130;
                if (dgvHistorial.Columns["VersionOrigen"] != null) dgvHistorial.Columns["VersionOrigen"].Width = 100;
                ActualizarEncabezados();
            }

            btnRollback.Enabled = false;
        }

        private void dgvHistorial_SelectionChanged(object sender, EventArgs e)
        {
            BE.UsuarioHistorial sel = dgvHistorial.CurrentRow?.DataBoundItem as BE.UsuarioHistorial;
            btnRollback.Enabled = sel != null && sel.TipoCambio != "BAJA";
        }

        private void btnRollback_Click(object sender, EventArgs e)
        {
            BE.UsuarioHistorial sel = dgvHistorial.CurrentRow?.DataBoundItem as BE.UsuarioHistorial;
            if (sel == null) return;

            // FIX: no permitir rollback de un rollback
            if (sel.TipoCambio == "ROLLBACK")
            {
                MessageBox.Show("No se puede restaurar una versión que ya es un rollback.",
                    "Operación no permitida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string msg = string.Format(
                "¿Restaurar el estado del usuario '{0}' a la versión del {1:dd/MM/yyyy HH:mm}?\n\n" +
                "Tipo de cambio registrado: {2}\n\n" +
                "Nota: la contraseña NO se restaurará (se mantendrá la actual).",
                _usuario.Usuario, sel.FechaCambio, sel.TipoCambio);

            if (MessageBox.Show(msg, "Confirmar rollback",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                string admin = SeguridadYServicios.SessionManager.getInstance().getUsuario().Usuario;
                _bll.Rollback(sel.Id, admin);
                MessageBox.Show("Estado restaurado correctamente.", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarHistorial();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
