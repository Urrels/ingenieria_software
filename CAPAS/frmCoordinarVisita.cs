using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CAPAS
{
    public partial class frmCoordinarVisita : FormBase
    {
        private readonly BLL.AlertaRevisionBLL _alertaBLL = new BLL.AlertaRevisionBLL();
        private readonly BLL.EquipoBLL _equipoBLL = new BLL.EquipoBLL();
        private readonly BLL.VisitaTecnicaBLL _visitaBLL = new BLL.VisitaTecnicaBLL();


        private List<BE.AlertaRevision> _alertas;
        private BE.USUARIO _tecnicoSeleccionado;

        public frmCoordinarVisita()
        {
            InitializeComponent();
        }

        private void frmCoordinarVisita_Load(object sender, EventArgs e)
        {
            dtpFecha.Value = DateTime.Today.AddDays(1);
            CargarAlertas();

            InicializarFormulario();
        }

        private void CargarAlertas()
        {
            _alertas = _alertaBLL.ListarAutorizadas();
            dgvAlertas.DataSource = null;
            dgvAlertas.DataSource = _alertas;

            if (dgvAlertas.Columns.Count > 0)
            {
                if (dgvAlertas.Columns["EquipoId"] != null) dgvAlertas.Columns["EquipoId"].Visible = false;
                if (dgvAlertas.Columns["EquipoNombre"] != null) dgvAlertas.Columns["EquipoNombre"].HeaderText = "Equipo";
                if (dgvAlertas.Columns["FechaGeneracion"] != null) dgvAlertas.Columns["FechaGeneracion"].HeaderText = "Fecha alerta";
                if (dgvAlertas.Columns["Estado"] != null) dgvAlertas.Columns["Estado"].HeaderText = "Estado";
            }

            lblTecnico.Text = "Técnico: (seleccioná una alerta)";
            _tecnicoSeleccionado = null;
        }

        private BE.AlertaRevision ObtenerAlertaSeleccionada()
        {
            if (dgvAlertas.CurrentRow?.DataBoundItem is BE.AlertaRevision alerta) return alerta;
            MsgBox.Show(Textos.T("msg_SeleccionaUnaAlerta", "Seleccioná una alerta."), "Atención", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
            return null;
        }

        private void dgvAlertas_SelectionChanged(object sender, EventArgs e)
        {
            var alerta = dgvAlertas.CurrentRow?.DataBoundItem as BE.AlertaRevision;
            if (alerta == null) return;

            var equipo = _equipoBLL.ObtenerPorId(alerta.EquipoId);
            _tecnicoSeleccionado = _visitaBLL.ObtenerTecnicoHabitual(equipo);

            lblTecnico.Text = _tecnicoSeleccionado != null
                ? $"Técnico habitual: {_tecnicoSeleccionado.Usuario}"
                : "Sin técnico habitual asignado — usá 'Buscar alternativo'.";
        }

        private void btnBuscarAlternativo_Click(object sender, EventArgs e)
        {
            var alternativos = _visitaBLL.ListarTecnicosAlternativos();
            if (alternativos.Count == 0)
            {
                MsgBox.Show(Textos.T("msg_NoHayTecnicosAlternativosRegistradosEnElSistema", "No hay técnicos alternativos registrados en el sistema."), "Atención",
                    MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            using (var frm = new frmSeleccionarReemplazo(alternativos))
            {
                if (frm.ShowDialog() != DialogResult.OK) return;
                _tecnicoSeleccionado = frm.EmpleadoSeleccionado;
                lblTecnico.Text = $"Técnico (alternativo): {_tecnicoSeleccionado.Usuario}";
            }
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            var alerta = ObtenerAlertaSeleccionada();
            if (alerta == null) return;

            if (_tecnicoSeleccionado == null)
            {
                MsgBox.Show(Textos.T("msg_ElegiUnTecnicoHabitualOAlternativoAntesDeConfirmar", "Elegí un técnico (habitual o alternativo) antes de confirmar."), "Atención",
                    MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            _visitaBLL.CoordinarVisita(alerta.Id, _tecnicoSeleccionado.Id, dtpFecha.Value);
            _alertaBLL.MarcarCoordinada(alerta);

            MsgBox.Show(Textos.T("msg_SolicitudDeVisitaConfirmada", "Solicitud de visita confirmada."), "Éxito", MsgBox.Botones.OK, MsgBox.Icono.Exito);
            CargarAlertas();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}