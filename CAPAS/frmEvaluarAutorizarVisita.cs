using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CAPAS
{
    public partial class frmEvaluarAutorizarVisita : FormBase
    {
        private readonly BLL.AlertaRevisionBLL _alertaBLL = new BLL.AlertaRevisionBLL();


        private List<BE.AlertaRevision> _alertas;

        public frmEvaluarAutorizarVisita()
        {
            InitializeComponent();
        }

        private void frmEvaluarAutorizarVisita_Load(object sender, EventArgs e)
        {
            CargarAlertas();

            InicializarFormulario();
        }

        private void CargarAlertas()
        {
            _alertas = _alertaBLL.ListarPendientes();
            dgvAlertas.DataSource = null;
            dgvAlertas.DataSource = _alertas;

            if (dgvAlertas.Columns.Count > 0)
            {
                if (dgvAlertas.Columns["EquipoId"] != null) dgvAlertas.Columns["EquipoId"].Visible = false;
                if (dgvAlertas.Columns["EquipoNombre"] != null) dgvAlertas.Columns["EquipoNombre"].HeaderText = "Equipo";
                if (dgvAlertas.Columns["FechaGeneracion"] != null) dgvAlertas.Columns["FechaGeneracion"].HeaderText = "Fecha";
                if (dgvAlertas.Columns["Estado"] != null) dgvAlertas.Columns["Estado"].HeaderText = "Estado";
            }
        }

        private BE.AlertaRevision ObtenerSeleccionada()
        {
            if (dgvAlertas.CurrentRow?.DataBoundItem is BE.AlertaRevision alerta) return alerta;
            MsgBox.Show(Textos.T("msg_SeleccionaUnaAlerta", "Seleccioná una alerta."), "Atención", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
            return null;
        }

        private void btnAutorizar_Click(object sender, EventArgs e)
        {
            var alerta = ObtenerSeleccionada();
            if (alerta == null) return;

            var equipo = _alertaBLL.ObtenerEquipoDeAlerta(alerta);
            if (MsgBox.Show(Textos.T("msg_AutorizarLaVisitaTecnicaPara", "¿Autorizar la visita técnica para '{0}'?", equipo.Nombre), "Confirmar",
                MsgBox.Botones.SiNo, MsgBox.Icono.Pregunta) != DialogResult.Yes)
                return;

            _alertaBLL.AutorizarVisita(alerta);
            MsgBox.Show(Textos.T("msg_VisitaAutorizada", "Visita autorizada."), "Éxito", MsgBox.Botones.OK, MsgBox.Icono.Exito);
            CargarAlertas();
        }

        private void btnDescartar_Click(object sender, EventArgs e)
        {
            var alerta = ObtenerSeleccionada();
            if (alerta == null) return;

            if (MsgBox.Show(Textos.T("msg_DescartarEstaAlertaComoFalsoPositivo", "¿Descartar esta alerta como falso positivo?"), "Confirmar",
                MsgBox.Botones.SiNo, MsgBox.Icono.Pregunta) != DialogResult.Yes)
                return;

            _alertaBLL.DescartarAlerta(alerta);
            MsgBox.Show(Textos.T("msg_AlertaDescartada", "Alerta descartada."), "Éxito", MsgBox.Botones.OK, MsgBox.Icono.Exito);
            CargarAlertas();
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            ExcelExportHelper.ExportarDataGridView(dgvAlertas, "AlertasRevision");
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}