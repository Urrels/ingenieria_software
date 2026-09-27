using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ReaLTaiizor.Forms;
using ReaLTaiizor.Manager;

namespace CAPAS
{
    public partial class frmMisNotificaciones : MaterialForm, SeguridadYServicios.IObservadorIdioma
    {
        private readonly BLL.NotificacionBLL _notificacionBLL = new BLL.NotificacionBLL();

        private readonly Dictionary<string, Control> _controles = new Dictionary<string, Control>();
        private readonly Dictionary<string, string> _defaults = new Dictionary<string, string>();

        public frmMisNotificaciones()
        {
            InitializeComponent();
        }

        private void frmMisNotificaciones_Load(object sender, EventArgs e)
        {
            CargarNotificaciones();
            timerRefresco.Start();

            GuardarDefaults(this.Controls);
            _controles[this.Name] = this;
            _defaults[this.Name] = this.Text;
            SeguridadYServicios.IdiomaManager.getInstance().Registrar(this);
            ActualizarIdioma();
            IdiomaUIHelper.AgregarSelector(this);
            MaterialSkinManager.Instance.AddFormToManage(this);
            AppTheme.AplicarTema(this);
        }

        private void timerRefresco_Tick(object sender, EventArgs e)
        {
            CargarNotificaciones();
        }

        private void CargarNotificaciones()
        {
            BE.USUARIO usuario = SeguridadYServicios.SessionManager.getInstance().getUsuario();
            var notificaciones = _notificacionBLL.ListarPorUsuario(usuario);

            int filaSeleccionada = dgvNotificaciones.CurrentCell?.RowIndex ?? -1;

            dgvNotificaciones.DataSource = null;
            dgvNotificaciones.DataSource = notificaciones;

            if (dgvNotificaciones.Columns.Count > 0)
            {
                dgvNotificaciones.Columns["Id"].Visible = false;
                dgvNotificaciones.Columns["GrillaId"].Visible = false;
                dgvNotificaciones.Columns["UsuarioId"].Visible = false;
                dgvNotificaciones.Columns["Mensaje"].HeaderText = "Mensaje";
                dgvNotificaciones.Columns["Mensaje"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvNotificaciones.Columns["FechaEnvio"].HeaderText = "Fecha";
                dgvNotificaciones.Columns["Estado"].HeaderText = "Estado";
            }

            if (filaSeleccionada >= 0 && filaSeleccionada < dgvNotificaciones.Rows.Count
                && dgvNotificaciones.Columns.Contains("Mensaje"))
            {
                dgvNotificaciones.CurrentCell = dgvNotificaciones.Rows[filaSeleccionada].Cells["Mensaje"];
            }
        }

        private void frmMisNotificaciones_FormClosed(object sender, FormClosedEventArgs e)
        {
            timerRefresco.Stop();
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
                    _defaults[c.Name] = c.Text;
                }
                if (c.HasChildren) GuardarDefaults(c.Controls);
            }
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            ExcelExportHelper.ExportarDataGridView(dgvNotificaciones, "MisNotificaciones");
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}