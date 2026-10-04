using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CAPAS
{
    public partial class frmRealizarRevision : FormBase
    {
        private readonly BLL.VisitaTecnicaBLL _visitaBLL = new BLL.VisitaTecnicaBLL();
        private readonly BLL.AlertaRevisionBLL _alertaBLL = new BLL.AlertaRevisionBLL();
        private readonly BLL.EquipoBLL _equipoBLL = new BLL.EquipoBLL();
        private readonly BLL.InformeMantenimientoBLL _informeBLL = new BLL.InformeMantenimientoBLL();


        private List<BE.VisitaTecnica> _visitas;

        public frmRealizarRevision()
        {
            InitializeComponent();
        }

        private void frmRealizarRevision_Load(object sender, EventArgs e)
        {
            CargarVisitas();

            InicializarFormulario();
        }

        private void CargarVisitas()
        {
            int tecnicoId = SeguridadYServicios.SessionManager.getInstance().getUsuario().Id;
            _visitas = _visitaBLL.ListarPendientesPorTecnico(tecnicoId);

            dgvVisitas.DataSource = null;
            dgvVisitas.DataSource = _visitas;

            if (dgvVisitas.Columns.Count > 0)
            {
                if (dgvVisitas.Columns["TecnicoId"] != null) dgvVisitas.Columns["TecnicoId"].Visible = false;
                ActualizarEncabezados();
            }

            txtResultado.Clear();
            chkPendienteRepuesto.Checked = false;
        }

        private void dgvVisitas_SelectionChanged(object sender, EventArgs e)
        {
            var visita = dgvVisitas.CurrentRow?.DataBoundItem as BE.VisitaTecnica;
            if (visita == null) { lblEquipo.Text = Textos.T("lbl_EquipoSinSeleccion", "Equipo: —"); return; }

            BE.AlertaRevision alerta = _alertaBLL.ObtenerPorId(visita.AlertaId);
            BE.Equipo equipo = alerta != null ? _equipoBLL.ObtenerPorId(alerta.EquipoId) : null;

            lblEquipo.Text = equipo != null
                ? Textos.T("lbl_EquipoConUso", "Equipo: {0}  (uso acumulado: {1})", equipo.Nombre, equipo.UsoAcumulado)
                : Textos.T("lbl_EquipoSinDetalle", "Equipo: (no se pudo obtener el detalle)");
        }
        private void btnRegistrarInforme_Click(object sender, EventArgs e)
        {
            if (!(dgvVisitas.CurrentRow?.DataBoundItem is BE.VisitaTecnica visita))
            {
                MsgBox.Show(Textos.T("msg_SeleccionaUnaVisita", "Seleccioná una visita."), "Atención", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtResultado.Text))
            {
                MsgBox.Show(Textos.T("msg_IngresaElResultadoDeLaRevision", "Ingresá el resultado de la revisión."), "Atención",
                    MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            _informeBLL.RegistrarInforme(visita, visita.AlertaId, txtResultado.Text.Trim(), chkPendienteRepuesto.Checked);

            MsgBox.Show(Textos.T("msg_InformeDeMantenimientoRegistradoSeNotificoAlAdministrador", "Informe de mantenimiento registrado. Se notificó al Administrador."), "Éxito",
                MsgBox.Botones.OK, MsgBox.Icono.Exito);
            CargarVisitas();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ActualizarEncabezados()
        {
            Encabezado(dgvVisitas, "AlertaId", "hdr_Alerta", "Alerta");
            Encabezado(dgvVisitas, "FechaCoordinada", "hdr_FechaCoordinada", "Fecha coordinada");
            Encabezado(dgvVisitas, "Estado", "hdr_Estado", "Estado");
        }

        protected override void ActualizarTextosDinamicos()
        {
            ActualizarEncabezados();
        }
    }
}