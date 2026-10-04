using System;
using System.Windows.Forms;

namespace CAPAS
{
    public partial class frmMisNotificaciones : FormBase
    {
        private readonly BLL.NotificacionBLL _notificacionBLL = new BLL.NotificacionBLL();


        public frmMisNotificaciones()
        {
            InitializeComponent();
        }

        private void frmMisNotificaciones_Load(object sender, EventArgs e)
        {
            CargarNotificaciones();
            timerRefresco.Start();

            InicializarFormulario();
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
                dgvNotificaciones.Columns["Mensaje"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                ActualizarEncabezados();
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
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            ExcelExportHelper.ExportarDataGridView(dgvNotificaciones, "MisNotificaciones");
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ActualizarEncabezados()
        {
            Encabezado(dgvNotificaciones, "Mensaje", "hdr_Mensaje", "Mensaje");
            Encabezado(dgvNotificaciones, "FechaEnvio", "hdr_Fecha", "Fecha");
            Encabezado(dgvNotificaciones, "Estado", "hdr_Estado", "Estado");
        }

        protected override void ActualizarTextosDinamicos()
        {
            ActualizarEncabezados();
        }
    }
}